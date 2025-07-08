namespace Start
{
    public enum EMessageId
    {
        None = 0,
        
        
        /// <summary>
        /// 获取本地版本
        /// </summary>
        GetLocalVersionFailure       = 100000,
        /// <summary>
        /// 重新获取本地版本
        /// </summary>
        ReGetLocalVersion     = 100001,
        
        /// <summary>
        /// 获取远程 GameClient
        /// </summary>
        GetRemoteGameClientFailure   = 100002,
        /// <summary>
        /// 重新获取远程 GameClient
        /// </summary>
        ReGetRemoteGameClient = 100003,
        
        /// <summary>
        /// 游戏版本号已更新
        /// </summary>
        GameVersionUpdated    = 100004,
        
        /// <summary>
        /// 获取内嵌资源Manifest
        /// </summary>
        GetBuiltInManifestFailure    = 100005,
        /// <summary>
        /// 重新获取内嵌资源Manifest
        /// </summary>
        ReGetBuiltInManifest  = 100006,
        
        /// <summary>
        /// 获取远程资源版本失败
        /// </summary>
        GetRemoteResourceVersionFailure   = 100007,
        /// <summary>
        /// 重新获取远程资源版本
        /// </summary>
        ReGetRemoteResourceVersion = 100008,
        
        /// <summary>
        /// 获取内嵌资源
        /// </summary>
        GetBuiltInResourceFailure     = 100009,
        /// <summary>
        /// 重新获取内嵌资源
        /// </summary>
        ReGetBuiltInResource   = 100010,
        
        /// <summary>
        /// 获取远程强制资源依赖
        /// </summary>
        GetRemoteMandatoryManifestFailure   = 100011,
        /// <summary>
        /// 重新获取远程强制资源依赖
        /// </summary>
        ReGetRemoteMandatoryManifest = 100012,
        /// <summary>
        /// 获取远程可选资源依赖
        /// </summary>
        GetRemoteOptionalManifestFailure = 100013,
        /// <summary>
        /// 重新获取远程可选资源依赖
        /// </summary>
        ReGetRemoteOptionalManifest = 100014,
        
        /// <summary>
        /// 显示更新资源信息
        /// </summary>
        DisplayUpdateResourceInfo = 100015,
        /// <summary>
        /// 确认更新
        /// </summary>
        ConfirmUpdateResource = 100016,
        
        
        /// <summary>
        /// 音频结束
        /// </summary>
        AudioEnd = 200001,
        
    }
}