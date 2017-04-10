using System.IO;

namespace FileWatcher.Sources
{
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
}