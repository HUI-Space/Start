using System;

namespace Start
{
    /// <summary>
    /// 资产数据 (指Unity中的Asset)
    /// </summary>
    [Serializable]
    public class AssetInfo
    {
        /// <summary>
        /// 可寻址地址
        /// </summary>
        public string Address;
        
        /// <summary>
        /// 资源路径
        /// </summary>
        public string Path;
        
        /// <summary>
        /// 所属资产包ID
        /// </summary>
        public string Resource;
        
        /// <summary>
        /// 是否包含Tag
        /// </summary>
        public string[] Tags;
    }
}