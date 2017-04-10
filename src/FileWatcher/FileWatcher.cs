using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using FileWatcher.Configuration;
using NLog;

namespace FileWatcher
{
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


    public class FileWatcher : IFileWather, IDisposable
    {
        private readonly IConfig _config;

        private readonly ISourceFolderManager _sourceFolderManager;

        public FileWatcher(IConfig config, ISourceFolderManager sourceFolderManager)
        {
            _config = config;
            _sourceFolderManager = sourceFolderManager;

            sourceFolderManager.Process(config.SourceFolder, WatcherOnChanged);

            _destFolders = new List<string>();
            foreach (var dstfolder in config.DestFolder.Split(';'))
            {
                _log.Debug("Папка назначения: {0}", dstfolder);
                _destFolders.Add(dstfolder);
            }
        }

        private readonly List<string> _destFolders;

        private bool _stopBackupThead;
        private Thread backupThread;

        private bool _firstStart = true;

        public void StartProcessing()
        {
            if (_firstStart)
            {
                _sourceFolderManager.SearchFiles();
                _firstStart = false;
            }

            if (_config.StartBackupThread)
            {
                backupThread = new Thread(o =>
                {
                    _log.Info("Запуск резервного потока.");
                    while (true)
                    {
                        if (_stopBackupThead)
                        {
                            _log.Info("Остановка резервного потока.");
                            break;
                        }
                        _log.Trace("Обработка цикла резервного потока.");
                        try
                        {
                            foreach (var sourceFolder in _sourceFolderManager.Folders)
                            {
                                foreach (var file in new DirectoryInfo(sourceFolder).GetFiles("*.*"))
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
                        Thread.Sleep(_config.BackupThreadTimer);
                    }
                });
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
                                //while (true)
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
