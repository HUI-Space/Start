using System;

namespace Start
{
    /// <summary>
    /// 游戏客户端信息
    /// 放置CDN上的游戏客户端信息
    /// </summary>
    [Serializable]
    public class GameClient
    {
        /// <summary>
        /// 登陆地址
        /// </summary>
        public string LoginUrl;
        
        /// <summary>
        /// 游戏版本号
        /// </summary>
        public string GameVersion;
        
        /// <summary>
        /// 强更地址
        /// </summary>
        public string DownloadUrl;
        
        /// <summary>
        /// 资源根节点
        /// </summary>
        public string ResourceRoot;

        /// <summary>
        /// 是否开启热更
        /// </summary>
        public bool EnableHotUpdate;
    }
}