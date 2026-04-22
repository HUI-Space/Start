using System;

namespace Start
{
    /// <summary>
    /// 时间工具类
    /// 提供时间戳与 DateTime 之间的转换功能
    /// </summary>
    public static class TimeUtility
    {
        private static readonly DateTime DateTime1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// 获取当前时间戳（毫秒）
        /// </summary>
        /// <returns>当前时间戳（毫秒）</returns>
        public static long TimeStamp()
        {
            return (DateTime.UtcNow.Ticks - DateTime1970.Ticks) / 10000;
        }

        /// <summary>
        /// 获取当前时间戳（秒）
        /// </summary>
        /// <returns>当前时间戳（秒）</returns>
        public static long TimeStampSeconds()
        {
            return (DateTime.UtcNow.Ticks - DateTime1970.Ticks) / 10000000;
        }

        /// <summary>
        /// 将 DateTime 转换为时间戳（毫秒）
        /// </summary>
        /// <param name="dt">要转换的 DateTime</param>
        /// <returns>时间戳（毫秒）</returns>
        public static long ConvertToTimeStamp(DateTime dt)
        {
            return (long)(dt.ToUniversalTime() - DateTime1970).TotalMilliseconds;
        }

        /// <summary>
        /// 将时间戳（毫秒）转换为 DateTime
        /// </summary>
        /// <param name="timeStamp">时间戳（毫秒）</param>
        /// <returns>DateTime（UTC）</returns>
        public static DateTime ConvertFromTimeStamp(long timeStamp)
        {
            return DateTime1970.AddMilliseconds(timeStamp);
        }

        /// <summary>
        /// 将时间戳（秒）转换为 DateTime
        /// </summary>
        /// <param name="timeStamp">时间戳（秒）</param>
        /// <returns>DateTime（UTC）</returns>
        public static DateTime ConvertFromTimeStampSeconds(long timeStamp)
        {
            return DateTime1970.AddSeconds(timeStamp);
        }
    }
}
