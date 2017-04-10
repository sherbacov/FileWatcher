using System.IO;

namespace FileWatcher.Sources
{
    /// <summary>
    /// ���������� ��������� �������������� �����
    /// </summary>
    public interface IFileSystemWather
    {
        string Folder { get; set; }
        event FileSystemEventHandler NewFile;

    }
}