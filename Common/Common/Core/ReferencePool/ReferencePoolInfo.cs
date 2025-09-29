using System;
using System.Runtime.InteropServices;

namespace Start
{
    [StructLayout(LayoutKind.Auto)]
    public struct ReferencePoolInfo
    {
        /// <summary>
        /// 获取引用池类型。
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// 获取未使用引用数量。
        /// </summary>
        public int UnusedReferenceCount { get; private set; }

        /// <summary>
        /// 获取正在使用引用数量。
        /// </summary>
        public int UsingReferenceCount { get; private set; }

        /// <summary>
        /// 获取获取引用数量。
        /// </summary>
        public int AcquireReferenceCount { get; private set; }

        /// <summary>
        /// 获取归还引用数量。
        /// </summary>
        public int ReleaseReferenceCount { get; private set; }

        /// <summary>
        /// 获取增加引用数量。
        /// </summary>
        public int AddReferenceCount { get; private set; }

        /// <summary>
        /// 获取移除引用数量。
        /// </summary>
        public int RemoveReferenceCount { get; private set; }
        
        /// <summary>
        /// 初始化引用池信息的新实例。
        /// </summary>
        /// <param name="type">引用池类型。</param>
        /// <param name="unusedReferenceCount">未使用引用数量。</param>
        /// <param name="usingReferenceCount">正在使用引用数量。</param>
        /// <param name="acquireReferenceCount">获取引用数量。</param>
        /// <param name="releaseReferenceCount">归还引用数量。</param>
        /// <param name="addReferenceCount">增加引用数量。</param>
        /// <param name="removeReferenceCount">移除引用数量。</param>
        public ReferencePoolInfo(Type type, int unusedReferenceCount, int usingReferenceCount, int acquireReferenceCount, int releaseReferenceCount, int addReferenceCount, int removeReferenceCount)
        {
            Type = type;
            UnusedReferenceCount = unusedReferenceCount;
            UsingReferenceCount = usingReferenceCount;
            AcquireReferenceCount = acquireReferenceCount;
            ReleaseReferenceCount = releaseReferenceCount;
            AddReferenceCount = addReferenceCount;
            RemoveReferenceCount = removeReferenceCount;
        }
    }
}