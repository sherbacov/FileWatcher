using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using NLog;

namespace FileWatcher
{
    public class Config : IConfig
    {
        public string SourceFolder { get; set; }
        public string DestFolder { get; set; }
        public ErrorMode Mode { get; set; }
        public int WaitSecs { get; set; }
        public int RetrySec { get; set; }
        public FileExistMode FileExist { get; set; }
        public bool StartBackupThread { get; set; }
    }

    public enum ErrorMode
    {
        /// <summary>
        /// Ошибка, прерывать обработку
        /// </summary>
        Error = 1,
        /// <summary>
        /// Продолжать копирование при наличии ошибки.
        /// </summary>
        Continue = 2,
        /// <summary>
        /// Ожидать RetrySec и поторить попытку. 
        /// </summary>
        WaitAndRetry = 3
    }

    public enum FileExistMode
    {
        /// <summary>
        /// Перезаписывать файл
        /// </summary>
        Override = 1,
        /// <summary>
        /// Пропускать файл из источника
        /// </summary>
        Ignore = 2
    }

    public interface IConfig
    {
        string SourceFolder { get; set; }

        string DestFolder { get; set; }

        ErrorMode Mode { get; set; }

        FileExistMode FileExist { get; set; }

        int WaitSecs { get; set; }

        int RetrySec { get; set; }

        bool StartBackupThread { get; set; }
    }

    /// <summary>
    /// Внутренний интерфейс обрабатывающий папку
    /// </summary>
    public interface IFileSystemWather
    {
        string Folder { get; set; }
        event FileSystemEventHandler NewFile;

    }

    public class InternalFileSystemWather : IFileSystemWather
    {
        private FileSystemWatcher _watcher;

        private string _folder;

        public string Folder
        {
            get { return _folder; }
            set
            {
                _folder = value;
                _watcher = new FileSystemWatcher(_folder);
                _watcher.NotifyFilter = NotifyFilters.LastWrite;
                _watcher.Filter = "*.*";
                _watcher.Changed += NewFile;
                _watcher.EnableRaisingEvents = true;
            }
        }

        public event FileSystemEventHandler NewFile;
    }


    public class SourceFolder : ISourceFolder
    {
        public IFileSystemWather Wather { get; set; }
    }


    public interface ISourceFolder
    {
        IFileSystemWather Wather { get; set; }
    }


    public class FileWatcher : IFileWather, IDisposable
    {
        private readonly IConfig _config;

        public FileWatcher(IConfig config, IFileSystemWather wather)
        {
            _config = config;

            _sourceFolders = new List<SourceFolder>();
            foreach (var folder in config.SourceFolder.Split(';'))
            {
                _log.Debug("Папка источник: {0}", folder);
                var sf = new SourceFolder();
                sf.Wather = wather;
                sf.Wather.NewFile += new FileSystemEventHandler(WatcherOnChanged);
                sf.Wather.Folder = folder;
                _sourceFolders.Add(sf);
            }

            _destFolders = new List<string>();
            foreach (var dstfolder in config.DestFolder.Split(';'))
            {
                _log.Debug("Папка назначения: {0}", dstfolder);
                _destFolders.Add(dstfolder);
            }
        }

        private readonly List<SourceFolder> _sourceFolders;

        private readonly List<string> _destFolders;

        private bool _stopBackupThead;
        private Thread backupThread;

        public void StartProcessing()
        {
            if (_config.StartBackupThread)
            {
                backupThread = new Thread((ParameterizedThreadStart) (o =>
                {
                    _log.Info("Запуск резервного потока.");
                    while (true)
                    {
                        if (_stopBackupThead)
                        {
                            _log.Info("Остановка резервного потока.");
                            break;
                        }

                        try
                        {
                            foreach (var sourceFolder in _sourceFolders)
                            {
                                foreach (var file in new DirectoryInfo(sourceFolder.Wather.Folder).GetFiles("*.*"))
                                {
                                    _log.Debug("Передача файла из резервного потока: {0}", file.Name);
                                    WatcherOnChanged((object) this, new FileSystemEventArgs(WatcherChangeTypes.Changed, file.Directory.FullName, file.Name));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.Error(ex);
                        }
                        Thread.Sleep(2000);
                    }
                }));
                try
                {
                    backupThread.Start();
                }
                catch (Exception ex)
                {
                    _log.Error(ex);
                }
            }
        }

        protected virtual bool IsFileLocked(FileInfo file)
        {
            FileStream fileStream = null;
            try
            {
                fileStream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException ex)
            {
                _log.Debug(ex, "Файл заблокирован.");
                return true;
            }
            finally
            {
                fileStream?.Close();
            }

            _log.Trace("Файл {0} свободен от блокировки.", file.Name);
            return false;
        }


        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        private void WatcherOnChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                _log.Debug("WatcherOnChanged ({1}) \"{0}\"", e.FullPath, e.ChangeType);

                if (!File.Exists(e.FullPath))
                {
                    _log.Debug("Файл не найден. Возможно уже перенесен. \"{0}\"", e.FullPath);
                }
                else
                {
                    _log.Info("Найден файл. \"{0}\"", e.Name);

                    while (true)
                    {
                        if (IsFileLocked(new FileInfo(e.FullPath)))
                            _log.Warn("Файл занят. \"{0}\"", e.FullPath);
                        else
                            break;

                        if (_config.WaitSecs != 0)
                            Thread.Sleep(_config.WaitSecs * 1000);
                    }
                    foreach (string destFileName in _destFolders.Select(folder => Path.Combine(folder, e.Name)))
                    {
                        _log.Info("Копирую файл \"{0}\" в \"{1}\"", e.Name, destFileName);

                        try
                        {
                            if (File.Exists(destFileName))
                            {
                                _log.Warn("Файл уже существует. \"{0}\"", destFileName);
                                switch (_config.FileExist)
                                {
                                    case FileExistMode.Override:
                                        _log.Warn("Файл будет перезаписан. \"{0}\"", destFileName);
                                        File.Delete(destFileName);
                                        break;
                                    case FileExistMode.Ignore:
                                        _log.Warn("Файл пропущен и не скопирован в \"{0}\"", destFileName);
                                        continue;
                                }
                            }

                            File.Copy(e.FullPath, destFileName);
                        }
                        catch (Exception exception)
                        {
                            _log.Error(exception, "Ошибка при копировании.");
                            if (_config.Mode == ErrorMode.Error)
                                throw;

                            if (_config.Mode == ErrorMode.Continue)
                                continue;

                            if (_config.Mode == ErrorMode.WaitAndRetry)
                            {
                                while (true)
                                {
                                    Thread.Sleep(_config.RetrySec);
                                    File.Copy(e.FullPath, destFileName);
                                }
                            }
                        }
                        
                    }
                    _log.Debug("Удаляю файл \"{0}\"", e.Name);
                    File.Delete(e.FullPath);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "Ошибка");
            }
        }

        public void Dispose()
        {
            if (backupThread != null)
            {
                _stopBackupThead = true;
                backupThread.Join();
            }
        }
    }
}
