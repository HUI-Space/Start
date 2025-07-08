namespace Start
{
    /// <summary>
    /// 加载资源状态。
    /// </summary>
    public enum EAsyncOperationStatus
    {
        None = 0,
        /// <summary>
        /// 加载资源完成。
        /// </summary>
        Succeeded ,

        /// <summary>
        /// 加载中。
        /// </summary>
        Processing,
        
        /// <summary>
        /// 加载资源错误。
        /// </summary>
        Failed
    }
}