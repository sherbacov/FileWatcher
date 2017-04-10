namespace FileWatcher.Configuration
{
    public class Config : IConfig
    {
        public string SourceFolder { get; set; }
        public string DestFolder { get; set; }
        public ErrorMode Mode { get; set; }
        public int WaitSecs { get; set; }
        public int RetrySec { get; set; }
        public FileExistMode FileExist { get; set; }
        public bool StartBackupThread { get; set; }
    }
}