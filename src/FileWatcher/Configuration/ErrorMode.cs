namespace FileWatcher.Configuration
{
    public enum ErrorMode
    {
        /// <summary>
        /// ������, ��������� ���������
        /// </summary>
        Error = 1,
        /// <summary>
        /// ���������� ����������� ��� ������� ������.
        /// </summary>
        Continue = 2,
        /// <summary>
        /// ������� RetrySec � �������� �������. 
        /// </summary>
        WaitAndRetry = 3
    }
}