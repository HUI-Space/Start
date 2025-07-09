using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Start.Editor
{
    /// <summary>
    /// 后续需要分工程打包时候再进行开发
    /// </summary>
    /*[Serializable]
    public class ResourceGroupConfig
    {
        public List<ResourcePackage> Packages = new List<ResourcePackage>();
    }


    [Serializable]
    public class ResourcePackage
    {
        public List<ResourceGroup> Groups = new List<ResourceGroup>();
    }*/
    
    [Serializable]
    public class ResourceGroupConfig
    {
        public List<ResourceGroup> Groups = new List<ResourceGroup>();
    }
    
    [Serializable]
    public class ResourceGroup
    {
        /// <summary>
        ///  是否激活
        /// </summary>
        public bool Enable;

        /// <summary>
        /// 名称
        /// </summary>
        public string Name;

        /// <summary>
        /// 标签 （用于标记）后续可作为例如分包使用
        /// </summary>
        public string Tags;
        
        /// <summary>
        /// 描述
        /// </summary>
        public string Desc;
        
        /// <summary>
        /// GroupItems
        /// </summary>
        public List<ResourceGroupItem> GroupItems = new List<ResourceGroupItem>();
    }
    
    [Serializable]
    public class ResourceGroupItem
    {
        /// <summary>
        /// 资源路径
        /// </summary>
        public string Path;
        
        /// <summary>
        /// 收集器类型
        /// </summary>
        public CollectorType CollectorType;
        
        /// <summary>
        /// 构建类型
        /// </summary>
        public PackType PackType;
        
        /// <summary>
        /// 可寻址
        /// </summary>
        public AddressType AddressType;
    }
}