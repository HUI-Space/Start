using System;
using System.Text;

namespace Start
{
    public class ReportLog : ILogHelper, IReference
    {
        private StringBuilder _stringBuilder;

        public static ReportLog Create()
        {
            ReportLog reportLog = ReferencePool.Acquire<ReportLog>();
            reportLog._stringBuilder = new StringBuilder();
            return reportLog;
        }

        public void AppendLine()
        {
            _stringBuilder.AppendLine();
        }

        public void Info(string message, params object[] args)
        {
            LogInternal(ELogType.Info, message, args);
        }

        public void Warning(string message, params object[] args)
        {
            LogInternal(ELogType.Warning, message, args);
        }

        public void Error(string message, params object[] args)
        {
            LogInternal(ELogType.Error, message, args);
        }

        public void Fatal(string message, params object[] args)
        {
            LogInternal(ELogType.Fatal, message, args);
        }

        public void Save(string name, string path)
        {
            FileUtility.WriteAllBytes(name, path, _stringBuilder.ToString());
        }

        private void LogInternal(ELogType logType, string message, object[] args)
        {
            _stringBuilder.AppendFormat("[{0:yyyy-MM-dd HH:mm:ss.fff}][{1}] ", DateTime.UtcNow.ToLocalTime(),
                logType.ToString());
            _stringBuilder.AppendFormat(message, args);
            _stringBuilder.AppendLine();
        }

        public override string ToString()
        {
            return _stringBuilder.ToString();
        }

        public void Log(ELogType logType, string message, params object[] args)
        {
            LogInternal(logType, message, args);
        }

        public void Clear()
        {
            _stringBuilder.Clear();
            _stringBuilder = default;
        }
    }
}