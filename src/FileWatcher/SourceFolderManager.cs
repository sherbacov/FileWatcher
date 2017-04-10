using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autofac;
using FileWatcher.Sources;
using NLog;

namespace FileWatcher
{
    public interface ISourceFolderManager
    {
        void Configure(string src, FileSystemEventHandler handler);
        IEnumerable<string> Folders { get; }

        void SearchFiles();
    }

    public class SourceFolderManager : ISourceFolderManager
    {
        public SourceFolderManager(ILifetimeScope container)
        {
            _container = container;
        }

        private readonly List<ISourceFolder> _sourceFolders = new List<ISourceFolder>();

        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly ILifetimeScope _container;

        private FileSystemEventHandler _handler;

        public void Configure(string src, FileSystemEventHandler handler)
        {
            _sourceFolders.Clear();
            _handler = handler;

            foreach (var folder in src.Split(';'))
            {
                _log.Debug("Папка источник: {0}", folder);
                var sf = _container.Resolve<ISourceFolder>();
                sf.Handler += handler;
                sf.Folder = folder;
                _sourceFolders.Add(sf);
            }
        }

        public void SearchFiles()
        {
            // Обработка файлов уже в папке
            foreach (var sourceFolder in _sourceFolders)
            {
                var files = sourceFolder.GetFiles();

                foreach (var file in files)
                {
                    _log.Debug($"Найден файл {file} при поиске по маске.");

                    _handler(this, new FileSystemEventArgs(WatcherChangeTypes.Changed,
                        Path.GetDirectoryName(file),
                        Path.GetFileName(file)
                    ));
                }
            }
        }

        public IEnumerable<string> Folders
        {
            get
            {
                return _sourceFolders.Select(sf => sf.Folder);
            }
        }
    }
}