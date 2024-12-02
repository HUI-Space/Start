using System.Collections.Generic;

namespace Start.Framework
{
    public static class Log
    {
        private static readonly HashSet<ILogHelper> _logHelpers = new HashSet<ILogHelper>();
        
        public static void SetLog(ILogHelper logHelper)
        {
            _logHelpers.Add(logHelper);
        }

        public static void Clear()
        {
            _logHelpers.Clear();
        }

        /// <summary>
        /// 信息。
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="args"></param>
        public static void Info(string message,params object[] args)
        {
            foreach (ILogHelper logHelper in _logHelpers)
            {
                logHelper?.Log(ELogType.Info,message,args);
            }
        }

        /// <summary>
        ///  警告。
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="args"></param>
        public static void Warning(string message,params object[] args)
        {
            foreach (ILogHelper logHelper in _logHelpers)
            {
                logHelper?.Log(ELogType.Warning,message,args);
            }
        }

        /// <summary>
        /// 错误。
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="args"></param>
        public static void Error(string message,params object[] args)
        {
            foreach (ILogHelper logHelper in _logHelpers)
            {
                logHelper?.Log(ELogType.Error,message,args);
            }
        }

        /// <summary>
        /// 严重错误。
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="args"></param>
        public static void Fatal(string message,params object[] args)
        {
            foreach (ILogHelper logHelper in _logHelpers)
            {
                logHelper?.Log(ELogType.Fatal,message,args);
            }
        }
    }
}