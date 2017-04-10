using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileWatcher
{
    public interface ISourceFolder
    {
        IFileSystemWather Watñher { get; set; }

        string Folder { get; set; }

        IEnumerable<string> GetFiles();
    }
    public class SourceFolder : ISourceFolder
    {
        public SourceFolder(IFileSystemWather watñher)
        {
            Watñher = watñher;
        }
        public IFileSystemWather Watñher { get; set; }
        public IEnumerable<string> GetFiles()
        {
            return new DirectoryInfo(Folder).GetFiles("*.*").Select(s => s.FullName);
        }

        public string Folder {
            get { return Watñher?.Folder;}
            set { Watñher.Folder = value; }
        }
    }
}