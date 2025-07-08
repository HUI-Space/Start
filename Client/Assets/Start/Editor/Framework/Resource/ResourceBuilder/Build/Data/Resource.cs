using System;
using System.Collections.Generic;
using UnityEditor;

namespace Start.Editor
{
    /// <summary>
    /// 资源数据
    /// MainResource 为配置的收集的资源数据 例如某个 ResourceGroupItem 中 ResourcePath 路径下的所有有效资源
    /// Resource 为运行时收集的所有有效资源数据
    /// </summary>
    public class Resource
    {
        /// <summary>
        /// 资源路径
        /// </summary>
        public string Path { get; private set; }
        
        /// <summary>
        /// 资源组Group 名称
        /// </summary>
        public string ResourceGroupName { get; private set; }

        /// <summary>
        /// 资源包名称
        /// </summary>
        public string ResourcePackageName { get; private set; }

        /// <summary>
        /// 资源GUID
        /// </summary>
        public string GUID
        {
            get
            {
                if (string.IsNullOrEmpty(_guid))
                {
                    _guid = AssetDatabase.AssetPathToGUID(Path);
                }
                return _guid;
            }
        }

        /// <summary>
        /// 文件格式
        /// </summary>
        public string Extension
        {
            get
            {
                if (string.IsNullOrEmpty(_guid))
                {
                    _extension = System.IO.Path.GetExtension(Path);
                }
                return _extension;
            }
        }
        
        /// <summary>
        /// 资源类型
        /// </summary>
        public Type Type => _type ?? (_type = AssetDatabase.GetMainAssetTypeAtPath(Path));
        
        /// <summary>
        /// 是否是Shader
        /// </summary>
        public bool IsShader => Type == typeof(UnityEngine.Shader) || Type == typeof(UnityEngine.ShaderVariantCollection);
        
        /// <summary>
        /// 直接依赖
        /// </summary>
        public HashSet<string> DirectDependencies => _directDependencies ?? (_directDependencies = Utility.GetDependencies(Path, false));

        /// <summary>
        /// 所有依赖
        /// 包括直接依赖和间接依赖
        /// </summary>
        public HashSet<string> AllDependencies => _allDependencies ?? (_allDependencies = Utility.GetDependencies(Path));
        
        private string _guid;
        private Type _type;
        private string _extension;
        private HashSet<string> _directDependencies;
        private HashSet<string> _allDependencies;
        
        public Resource(string path,string resourceGroupName,string resourcePackageName = null)
        {
            Path = path;
            ResourceGroupName = resourceGroupName;
            ResourcePackageName = resourcePackageName;
        }
    }
}