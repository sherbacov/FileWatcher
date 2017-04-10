using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileWatcher.Sources
{
    public class SourceFolderWat�her : ISourceFolder, IDisposable
    {
        public SourceFolderWat�her(IFileSystemWather wat�her)
        {
            Wat�her = wat�her;

            Wat�her.NewFile += Wat�herOnNewFile;
        }

        private void Wat�herOnNewFile(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            Handler(sender, fileSystemEventArgs);
        }

        protected IFileSystemWather Wat�her { get; set; }

        public FileSystemEventHandler Handler { get; set; }

        public IEnumerable<string> GetFiles()
        {
            return new DirectoryInfo(Folder).GetFiles("*.*").Select(s => s.FullName);
        }

        public string Folder {
            get => Wat�her?.Folder;
            set => Wat�her.Folder = value;
        }

        public void Dispose()
        {
            if (Wat�her != null)
                Wat�her.NewFile -= Wat�herOnNewFile;

        }
    }
}