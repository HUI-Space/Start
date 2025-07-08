using System;
using System.Collections.Generic;

namespace Start
{
    /// <summary>
    /// 资源清单
    /// </summary>
    [Serializable]
    public class Manifest
    {
        public List<AssetInfo> Assets = new List<AssetInfo>();
        public List<ResourceInfo> Resources = new List<ResourceInfo>();
    }
}