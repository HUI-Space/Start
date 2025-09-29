namespace Start
{
    public interface ILogHelper
    {
        void Log(ELogType logType, string message, params object[] args);
    }
}