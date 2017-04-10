namespace FileWatcher.Configuration
{
    public enum FileExistMode
    {
        /// <summary>
        /// Перезаписывать файл
        /// </summary>
        Override = 1,
        /// <summary>
        /// Пропускать файл из источника
        /// </summary>
        Ignore = 2
    }
}