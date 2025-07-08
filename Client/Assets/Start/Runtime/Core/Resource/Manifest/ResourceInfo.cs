using System;

namespace Start
{
    /// <summary>
    /// 资产包数据
    /// 打出所有AssetBundle的信息
    /// </summary>
    [Serializable]
    public class ResourceInfo
    {
        /// <summary>
        /// AssetBundle名称
        /// </summary>
        public string Name;
        
        /// <summary>
        /// MD5
        /// </summary>
        public string MD5;

        /// <summary>
        /// 文件大小
        /// </summary>
        public long Size;

        /// <summary>
        /// CRC
        /// </summary>
        public uint CRC;
        
        /// <summary>
        /// 偏移
        /// </summary>
        public ulong Offset;
        
        /// <summary>
        /// 资源类型
        /// </summary>
        public EResourceType ResourceType;
        
        /// <summary>
        /// 依赖的资源名
        /// </summary>
        public string[] Depends;
    }
}