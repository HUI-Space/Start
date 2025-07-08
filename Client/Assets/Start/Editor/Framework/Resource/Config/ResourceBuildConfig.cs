using System;
using UnityEditor;
using UnityEngine.Serialization;

namespace Start.Editor
{
    [Serializable]
    public class ResourceBuildConfig
    {
        /// <summary>
        /// 本地游戏版本号
        /// </summary>
        public string LocalGameVersion;
        
        /// <summary>
        /// 输出目录
        /// </summary>
        public string OutputDirectory;

        /// <summary>
        /// 资源模式
        /// </summary>
        public EResourceModeType ResourceModeType;

        /// <summary>
        /// 文件命名类型
        /// </summary>
        public NameType NameType;
        
        /// <summary>
        /// 构建的平台
        /// </summary>
        public BuildTarget BuildTarget;

        /// <summary>
        /// 资源包流水线的构建模式
        /// </summary>
        public BuildModeType BuildModeType;
        
        /// <summary>
        /// 对 AssetBundle 应用的压缩算法类型
        /// </summary>
        public CompressionType CompressionType;
        
        /// <summary>
        /// 禁止写入类型树结构（可以降低包体和内存并提高加载效率）
        /// </summary>
        public bool DisableWriteTypeTree = false;

        /// <summary>
        /// 忽略类型树变化
        /// </summary>
        public bool IgnoreTypeTreeChanges = true;

        /// <summary>
        /// 打热更的版本号（打热更的时间戳）
        /// </summary>
        public string ResourceVersion => Timestamp.ToString();
        
        /// <summary>
        /// 工作路径
        /// </summary>
        public string WorkingPath => $"{OutputDirectory}/Working/{BuildTarget.ToString()}/";
        
        /// <summary>
        /// 日志路径
        /// </summary>
        public string ReportPath => $"{OutputDirectory}/Report/" ;
        
        /// <summary>
        /// 输出路径
        /// </summary>
        public string OutputPath => $"{OutputDirectory}/{LocalGameVersion}/{BuildTarget.ToString()}/" ;
        
        /// <summary>
        /// 内置资源输出路径
        /// </summary>
        public string BuiltInPath => $"{OutputPath}/{EResourceType.BuiltIn}/";
        
        /// <summary>
        /// 强制下载资源输出路径
        /// </summary>
        public string MandatoryPath => $"{OutputPath}/{ResourceVersion}/";
        
        /// <summary>
        /// 构建日志路径
        /// </summary>
        public string BuildLog => $"{ReportPath}/{LocalGameVersion}_{BuildTarget}_{nameof(BuildLog)}.log";
        
        /// <summary>
        /// 强制下载资源信息版本日志
        /// </summary>
        public string RemoteResourceVersionLog => $"{ReportPath}/{LocalGameVersion}_{BuildTarget}_{nameof(ResourceVersion)}.log";
        
        /// <summary>
        /// 内嵌资源资源信息日志
        /// </summary>
        public string BuiltInResourceManifest => $"{ReportPath}/{LocalGameVersion}_{BuildTarget}_BuiltIn.log";
        
        /// <summary>
        /// 强制下载资源信息日志 
        /// </summary>
        public string MandatoryResourceManifest => $"{ReportPath}/{LocalGameVersion}_{BuildTarget}_Mandatory.log";
        
        /// <summary>
        /// 可选下载资源配置信息日志 
        /// </summary>
        public string OptionalResourceManifest => $"{ReportPath}/{LocalGameVersion}_{BuildTarget}_Optional.log";
        
        /// <summary>
        /// 打热更的时间戳
        /// </summary>
        [NonSerialized]
        public long Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        
        public BuildAssetBundleOptions GetBuildAssetBundleOptions()
        {
            BuildAssetBundleOptions opt = BuildAssetBundleOptions.None;
            opt |= BuildAssetBundleOptions.StrictMode; //Do not allow the build to succeed if any errors are reporting during it. 如果在构建过程中报告了任何错误，则不允许构建成功。
            if (BuildModeType == BuildModeType.DryRunBuild)
            {
                opt |= BuildAssetBundleOptions.DryRunBuild;
                return opt;
            }
            if (CompressionType == CompressionType.Uncompressed)
                opt |= BuildAssetBundleOptions.UncompressedAssetBundle;
            else if (CompressionType == CompressionType.LZ4)
                opt |= BuildAssetBundleOptions.ChunkBasedCompression;
            if (BuildModeType == BuildModeType.ForceRebuild)
                opt |= BuildAssetBundleOptions.ForceRebuildAssetBundle; //Force rebuild the asset bundles 强制重建资源包
            
            if (DisableWriteTypeTree)
                opt |= BuildAssetBundleOptions.DisableWriteTypeTree; //Do not include type information within the asset bundle (don't write type tree). 不要在资源包中包含类型信息(不要写类型树)。
            if (IgnoreTypeTreeChanges)
                opt |= BuildAssetBundleOptions.IgnoreTypeTreeChanges; //Ignore the type tree changes when doing the incremental build check. 在执行增量构建检查时忽略类型树更改。


            opt |= BuildAssetBundleOptions.DeterministicAssetBundle;//DeterministicAssetBundle 选项的作用是确保每次构建 AssetBundle 时，相同的输入资源会生成相同的二进制输出。也就是说，只要输入的资源和构建设置保持不变，那么多次构建得到的 AssetBundle 文件的二进制内容是完全相同的。
            //opt |= BuildAssetBundleOptions.DisableLoadAssetByFileName; //Disables Asset Bundle LoadAsset by file name.
            //opt |= BuildAssetBundleOptions.DisableLoadAssetByFileNameWithExtension; //Disables Asset Bundle LoadAsset by file name with extension.	
            
            return opt;
        }
    }
}