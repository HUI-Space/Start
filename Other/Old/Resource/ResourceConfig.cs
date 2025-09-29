using System.IO;
using Common;
using UnityEngine;

namespace Start
{
    public static class ResourceConfig
    {
        public const string AssetBundle = nameof(AssetBundle);
        
        /// <summary>
        /// 资源路径
        /// 包含：强制下载资源、可选下载资源
        /// </summary>
#if UNITY_STANDALONE_WIN
        public static readonly string ResourcePath = Path.Combine(Application.streamingAssetsPath, AssetBundle).RegularPath();
#else
        public static readonly string ResourcePath = Path.Combine(Application.persistentDataPath, AssetBundle).RegularPath();
#endif
        /// <summary>
        /// 内置资源名称
        /// 位置：包内
        /// </summary>
        public const string LocalBuiltInResource = "LocalBuiltInResource.s";
        
        /// <summary>
        /// 内置资源配置信息
        /// 位置：包内
        /// </summary>
        public const string LocalBuiltInManifest = "LocalBuiltInManifest.b";
        
        /// <summary>
        /// CDN上的资源版本信息
        /// 内容：资源版本号（打包的时候的时间戳）
        /// 位置：CDN
        /// </summary>
        public const string RemoteResourceVersion = "RemoteResourceVersion.b";
        
        /// <summary>
        /// 强制下载资源配置信息
        /// 位置：CDN
        /// </summary>
        public const string RemoteMandatoryManifest = "RemoteMandatoryManifest.b";
        
        /// <summary>
        /// 可选下载资源配置信息
        /// 位置：CDN
        /// </summary>
        public const string RemoteOptionalManifest = "RemoteOptionalManifest.b";
        
        /// <summary>
        /// 本地所有游戏资源的版本信息
        /// 主要流程包含：写入 BuiltIn 资源包，Mandatory 资源包，Optional 资源包
        /// 第一次运行游戏时，将 BuiltIn.b 写入 （即ResourceType.BuiltIn）
        /// 有新的热更资源时，将新的资源写入 Mandatory.b（即 ResourceType.Mandatory）
        /// 指定模块资源更新，将新的资源写入 Optional.b（即 ResourceType.Optional）
        /// 位置：本地（手机 persistentDataPath，电脑 StreamingAssets）
        /// </summary>
        public const string LocalManifest = "LocalManifest.b";
        
        
        /// <summary>
        /// 本地资源版本信息路径
        /// 内容：资源版本号（打资源的时候的时间戳）
        /// 位置：本地（手机 persistentDataPath，电脑 StreamingAssets）
        /// </summary>
        public static readonly string LocalResourceVersionPath = 
            Path.Combine(ResourcePath, "LocalResourceVersion.b").RegularPath();
        
        /// <summary>
        /// 本地已经下载可选资源信息路径
        /// 内容：已经下载可选资源模块信息
        /// 位置：本地（手机 persistentDataPath，电脑 StreamingAssets）
        /// </summary>
        public static readonly string LocalOptionalResourceInfoPath = 
            Path.Combine(ResourcePath, "LocalOptionalResourceInfo.b").RegularPath();
        
        /// <summary>
        /// 本地所有游戏资源的版本信息路径
        /// </summary>
        public static readonly string LocalManifestPath = 
            Path.Combine(ResourcePath, LocalManifest).RegularPath();
        
        /// <summary>
        /// 本地游戏版本路径
        /// </summary>
        public static readonly string LocalGameVersionPath = 
            Path.Combine(Application.streamingAssetsPath,"LocalGameVersion.b").RegularPath();
        
        /// <summary>
        /// 内置资源路径
        /// </summary>
        public static readonly string BuiltInResourcePath = 
            Path.Combine(Application.streamingAssetsPath, AssetBundle).RegularPath();
        
        /// <summary>
        /// 内置资源路径
        /// </summary>
        public static readonly string LocalBuiltInResourcePath = 
            Path.Combine(BuiltInResourcePath,LocalBuiltInResource).RegularPath();
        
        /// <summary>
        /// 内置资源配置信息路径
        /// </summary>
        public static readonly string LocalBuiltInManifestPath = 
            Path.Combine(BuiltInResourcePath,LocalBuiltInManifest).RegularPath();
        
        
        /// <summary>
        /// 根据资源类型和资源名称获取资源路径
        /// </summary>
        /// <param name="resourceType">资源类型</param>
        /// <param name="resourceName">资源名称</param>
        /// <returns></returns>
        public static string GetResourcePath(EResourceType resourceType, string resourceName)
        {
            if (resourceType == EResourceType.BuiltIn)
            {
                return Path.Combine(BuiltInResourcePath,LocalBuiltInResource);
            }
            return Path.Combine(GetResourcePath(resourceType), resourceName).RegularPath();
        }

        /// <summary>
        /// 根据资源类型获取资源路径
        /// </summary>
        /// <param name="resourceType"></param>
        /// <returns></returns>
        public static string GetResourcePath(EResourceType resourceType)
        {
            switch (resourceType)
            {
                case EResourceType.BuiltIn:
                    return BuiltInResourcePath;
                default:
                    return ResourcePath;
            }
        }
    }
}