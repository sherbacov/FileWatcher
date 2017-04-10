using System.IO;

namespace FileWatcher.Sources
{
    /// <summary>
    /// Внутренний интерфейс обрабатывающий папку
    /// </summary>
    public interface IFileSystemWather
    {
        string Folder { get; set; }
        event FileSystemEventHandler NewFile;

    }
}