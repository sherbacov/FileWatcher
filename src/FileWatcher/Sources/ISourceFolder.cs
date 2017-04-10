using System.Collections.Generic;
using System.IO;

namespace FileWatcher.Sources
{
    public interface ISourceFolder
    {
        string Folder { get; set; }

        FileSystemEventHandler Handler { get; set; }

        IEnumerable<string> GetFiles();
    }
}