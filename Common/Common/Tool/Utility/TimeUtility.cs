using System;
using System.Diagnostics;

namespace Start
{
    /// <summary>
    /// 时间工具
    /// </summary>
    public static class TimeUtility
    {
        /// <summary>
        /// 当前时间戳
        /// </summary>
        /// <returns>时间戳</returns>
        public static long TimeStamp()
        {
            DateTime DateTime1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (DateTime.UtcNow.Ticks - DateTime1970.Ticks) / 10000;
        }
        
        /// <summary>
        /// DateTime 转换为时间戳
        /// </summary>
        /// <param name="dt">DateTime</param>
        /// <returns>时间戳</returns>
        public static long ConvertToTimeStamp(DateTime dt)
        {
            DateTime DateTime1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(dt - DateTime1970).TotalMilliseconds;
        }

        /// <summary>
        /// 时间戳转换为 DateTime
        /// </summary>
        /// <param name="timeStamp">时间戳</param>
        /// <returns>DateTime</returns>
        public static DateTime ConvertFromTimeStamp(long timeStamp)
        {
            DateTime DateTime1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return DateTime1970.AddSeconds(timeStamp);
        }
    }
}