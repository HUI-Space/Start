using System;
using System.Collections.Generic;

namespace Start
{
    public static class Logger
    {
        private static readonly HashSet<ILogHelper> _logHelpers = new HashSet<ILogHelper>();

        static Logger()
        {
            List<Type> types = AssemblyUtility.GetChildType(typeof(ILogHelper));
            Type attribute = typeof(LogAttribute);
            foreach (Type type in types)
            {
                if (type.IsDefined(attribute, false))
                {
                    _logHelpers.Add((ILogHelper)Activator.CreateInstance(type));
                }
            }
        }

        public static void AddLogHelper(ILogHelper logHelper)
        {
            _logHelpers.Add(logHelper);
        }

        public static void RemoveLogHelper(ILogHelper logHelper)
        {
            _logHelpers.Remove(logHelper);
        }

        public static void Clear()
        {
            _logHelpers.Clear();
        }
        
        /// <summary>
        /// Log
        /// </summary>
        /// <param name="logType">日志类型</param>
        /// <param name="message">日志</param>
        /// <param name="args"></param>
        public static void Log(ELogType logType, string message, params object[] args)
        {
            foreach (ILogHelper logHelper in _logHelpers)
            {
                logHelper?.Log(logType, message, args);
            }
        }

        /// <summary>
        /// 信息。
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="args"></param>
        public static void Info(string message, params object[] args)
        {
            foreach (ILogHelper logHelper in _logHelpers)
            {
                logHelper?.Log(ELogType.Info, message, args);
            }
        }
        
        /// <summary>
        ///  警告。
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="args"></param>
        public static void Warning(string message, params object[] args)
        {
            foreach (ILogHelper logHelper in _logHelpers)
            {
                logHelper?.Log(ELogType.Warning, message, args);
            }
        }

        /// <summary>
        /// 错误。
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="args"></param>
        public static void Error(string message, params object[] args)
        {
            foreach (ILogHelper logHelper in _logHelpers)
            {
                logHelper?.Log(ELogType.Error, message, args);
            }
        }

        /// <summary>
        /// 严重错误。
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="args"></param>
        public static void Fatal(string message, params object[] args)
        {
            foreach (ILogHelper logHelper in _logHelpers)
            {
                logHelper?.Log(ELogType.Fatal, message, args);
            }
        }
    }
}