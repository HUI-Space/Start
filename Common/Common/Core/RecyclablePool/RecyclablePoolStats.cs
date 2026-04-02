using System;

namespace Start
{
    /// <summary>
    /// 可回收对象池的统计信息
    /// </summary>
    public struct RecyclablePoolStats
    {
        /// <summary>
        /// 池管理的对象类型
        /// </summary>
        public Type Type { get; private set; }
        
        /// <summary>
        /// 池内闲置对象数量
        /// </summary>
        public int UnusedCount { get; private set; }
        
        /// <summary>
        /// 正在使用的对象数量
        /// </summary>
        public int UsingCount { get; private set; }
        
        /// <summary>
        /// 获取对象数量
        /// </summary>
        public int AcquireCount { get; private set; }

        /// <summary>
        /// 归还对象数量
        /// </summary>
        public int ReleaseCount { get; private set; }
        
        /// <summary>
        /// 获取增加对象数量。
        /// </summary>
        public int AddCount { get; private set; }

        /// <summary>
        /// 获取移除对象数量。
        /// </summary>
        public int RemoveCount { get; private set; }
        
        /// <summary>
        /// 累计创建的对象总数
        /// </summary>
        public int TotalCreateCount { get; private set; }

        /// <summary>
        /// 历史峰值使用数量
        /// </summary>
        public int PeakUsingCount { get; private set; }
        
        /// <summary>
        /// 最后一次操作（获取/归还）时间
        /// </summary>
        public DateTime LastOperationTime { get; private set; }
        
        /// <summary>
        /// 初始化回收池信息
        /// </summary>
        /// <param name="type">对象类型</param>
        /// <param name="unusedCount">未使用的对象数量</param>
        /// <param name="usingCount">正在使用的对象数量</param>
        /// <param name="acquireCount">获取对象数量</param>
        /// <param name="releaseCount">归还对象数量</param>
        /// <param name="addCount">获取增加对象数量。</param>
        /// <param name="removeCount">获取移除对象数量。</param>
        /// <param name="totalCreateCount">累计创建对象数量</param>
        /// <param name="peakUsingCount">峰值使用对象数量</param>
        /// <param name="lastOperationTime">最后操作时间</param>
        public RecyclablePoolStats(Type type, int unusedCount, int usingCount, int acquireCount, int releaseCount, int addCount, int removeCount, int totalCreateCount, int peakUsingCount, DateTime lastOperationTime)
        {
            Type = type;
            UnusedCount = unusedCount;
            UsingCount = usingCount;
            AcquireCount = acquireCount;
            ReleaseCount = releaseCount;
            AddCount = addCount;
            RemoveCount = removeCount;
            TotalCreateCount = totalCreateCount;
            PeakUsingCount = peakUsingCount;
            LastOperationTime = lastOperationTime;
        }
    }
}