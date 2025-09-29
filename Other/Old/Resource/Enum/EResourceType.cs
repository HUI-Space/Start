namespace Start
{
    /// <summary>
    /// 资源类型
    /// </summary>
    public enum EResourceType
    {
        None,
        
        /// <summary>
        /// 内置资源
        /// 简介：随出包一并出，作为基础资源
        /// 存放位置:StreamingAssets
        /// 额外处理 ：需要收集并记录
        /// </summary>
        BuiltIn,
        
        /// <summary>
        /// 强制下载资源
        /// 简介：主要通过热更的方式，下载更新到本地（判断CDN是否有配置，有则下载，无则不需要下载）
        /// 存放位置:手机 persistentDataPath，电脑 StreamingAssets
        /// 额外处理 ：无
        /// </summary>
        Mandatory,
        
        /// <summary>
        /// 可选下载资源
        /// 简介：没有该资源也可运行游戏，类似于DLC形式，下载后即可游戏特殊玩法，该部分资源需要收集并记录
        /// 存放位置:手机 persistentDataPath，电脑 StreamingAssets
        /// 额外处理 ：需要收集并记录
        /// </summary>
        Optional,
    }
}