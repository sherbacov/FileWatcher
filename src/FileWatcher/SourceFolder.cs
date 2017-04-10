using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileWatcher
{
    public interface ISourceFolder
    {
        IFileSystemWather Wat�her { get; set; }

        string Folder { get; set; }

        IEnumerable<string> GetFiles();
    }
    public class SourceFolder : ISourceFolder
    {
        public SourceFolder(IFileSystemWather wat�her)
        {
            Wat�her = wat�her;
        }
        public IFileSystemWather Wat�her { get; set; }
        public IEnumerable<string> GetFiles()
        {
            return new DirectoryInfo(Folder).GetFiles("*.*").Select(s => s.FullName);
        }

        public string Folder {
            get { return Wat�her?.Folder;}
            set { Wat�her.Folder = value; }
        }
    }
}