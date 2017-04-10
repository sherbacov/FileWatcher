namespace FileWatcher.Configuration
{
    public interface IConfig
    {
        string SourceFolder { get; set; }

        string DestFolder { get; set; }

        ErrorMode Mode { get; set; }

        FileExistMode FileExist { get; set; }

        int WaitSecs { get; set; }

        int RetrySec { get; set; }

        bool StartBackupThread { get; set; }
    }
}