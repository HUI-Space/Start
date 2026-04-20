using System;

namespace Start
{
    /// <summary>
    /// StructTask 诊断信息
    /// </summary>
    public static class StructTaskDiagnostics
    {
        /// <summary>
        /// 未处理异常事件
        /// </summary>
        public static event Action<Exception> OnUnhandledException;

        /// <summary>
        /// 报告未处理异常
        /// </summary>
        /// <param name="ex">异常对象</param>
        internal static void ReportException(Exception ex)
        {
            OnUnhandledException?.Invoke(ex);
        }
    }
}
