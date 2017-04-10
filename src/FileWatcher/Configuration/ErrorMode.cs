namespace FileWatcher.Configuration
{
    public enum ErrorMode
    {
        /// <summary>
        /// Ошибка, прерывать обработку
        /// </summary>
        Error = 1,
        /// <summary>
        /// Продолжать копирование при наличии ошибки.
        /// </summary>
        Continue = 2,
        /// <summary>
        /// Ожидать RetrySec и поторить попытку. 
        /// </summary>
        WaitAndRetry = 3
    }
}