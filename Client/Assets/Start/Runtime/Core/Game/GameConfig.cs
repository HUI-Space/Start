using System.IO;
using Common;

namespace Start
{
    /// <summary>
    /// 游戏配置(游戏的杂项配置都在这里)
    /// </summary>
    public static class GameConfig
    {
        #region 运行基础配置

        /// <summary>
        /// 开启日志
        /// </summary>
        public static bool EnableLog = false;
        
        /// <summary>
        /// 显示FPS
        /// </summary>
        public static bool EnableFPS = false;
        
        /// <summary>
        /// 读取AssetBundle
        /// </summary>
        public static bool EnableAssetbundle = true;
        
        /// <summary>
        /// 游戏资源模式
        /// </summary>
        public static EResourceModeType ResourceModeType = EResourceModeType.Standalone;

        /// <summary>
        /// CDN地址
        /// </summary>
        public static readonly string CDNUrl = "http://192.168.16.194";
        
        #endregion


        #region 本地需要读取的相关配置

        /// <summary>
        /// 本地渠道Id
        /// </summary>
        public static int LocalChannelId = 1;
        
        /// <summary>
        /// 本地游戏版本
        /// </summary>
        public static string LocalGameVersion;

        #endregion
        
        
        #region 远程CDN 配置，路径，相关
        
        public static GameClient GameClient;
        
        /// <summary>
        /// 远程资源版本
        /// </summary>
        public static string RemoteResourceVersion;
        
        /// <summary>
        /// 是否存在远程可选资源
        /// </summary>
        public static bool ExistRemoteOptionalResource;
        
        /// <summary>
        /// 远程GameClient地址
        /// </summary>
        public static string RemoteGameClientPath => 
            Path.Combine(CDNUrl, LocalGameVersion, $"{nameof(GameClient)}_{LocalChannelId}.txt").RegularPath();

        /// <summary>
        /// 远程资源根目录
        /// </summary>
        public static string RemoteResourcePath => 
            Path.Combine(CDNUrl, LocalGameVersion, GameClient.ResourceRoot);
        
        /// <summary>
        /// CDN上的资源版本信息路径
        /// 内容：资源版本号（打资源的时候的时间戳）
        /// 位置：CDN
        /// </summary>
        public static string RemoteResourceVersionPath => 
            Path.Combine(RemoteResourcePath, ResourceConfig.RemoteResourceVersion).RegularPath();

        /// <summary>
        /// 热更新下载地址
        /// </summary>
        public static string RemoteDownloadResourcePath => 
            Path.Combine(RemoteResourcePath,RemoteResourceVersion).RegularPath();
        
        /// <summary>
        /// 强制下载资源配置信息地址
        /// 远程地址
        /// Mandatory.b
        /// </summary>
        public static string RemoteMandatoryManifestPath => 
            Path.Combine(RemoteDownloadResourcePath, ResourceConfig.RemoteMandatoryManifest).RegularPath();

        /// <summary>
        /// 可选下载资源配置信息地址
        /// 远程地址
        /// Optional.b
        /// </summary>
        public static string RemoteOptionalManifestPath => 
            Path.Combine(RemoteDownloadResourcePath, ResourceConfig.RemoteOptionalManifest).RegularPath();
        
        #endregion
    }
}