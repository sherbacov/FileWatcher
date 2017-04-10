using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileWatcher.Sources
{
    public class SourceFolderWatñher : ISourceFolder, IDisposable
    {
        public SourceFolderWatñher(IFileSystemWather watñher)
        {
            Watñher = watñher;

            Watñher.NewFile += WatñherOnNewFile;
        }

        private void WatñherOnNewFile(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            Handler(sender, fileSystemEventArgs);
        }

        protected IFileSystemWather Watñher { get; set; }

        public FileSystemEventHandler Handler { get; set; }

        public IEnumerable<string> GetFiles()
        {
            return new DirectoryInfo(Folder).GetFiles("*.*").Select(s => s.FullName);
        }

        public string Folder {
            get => Watñher?.Folder;
            set => Watñher.Folder = value;
        }

        public void Dispose()
        {
            if (Watñher != null)
                Watñher.NewFile -= WatñherOnNewFile;

        }
    }
}