namespace Start
{
    public interface IAsyncOperationHandle : IReference
    {
        /// <summary>
        /// 是否完成
        /// </summary>
        bool IsDone { get; }
        
        /// <summary>
        /// 资源进度
        /// </summary>
        float Progress { get; }
        
        /// <summary>
        /// 资产名称
        /// </summary>
        string AssetName { get; }
        
        /// <summary>
        /// 资源名称
        /// </summary>
        string ResourceName { get; }
        
        /// <summary>
        /// 加载资源状态。
        /// </summary>
        EAsyncOperationStatus EAsyncOperationStatus { get; }
        
        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="status"></param>
        void SetStatus(EAsyncOperationStatus status);
        
        /// <summary>
        /// 设置Resource名称
        /// </summary>
        /// <param name="resourceName"></param>
        void SetResourceName(string resourceName);
        
        /// <summary>
        /// 设置游戏资源名称
        /// </summary>
        /// <param name="assetName"></param>
        void SetAssetName(string assetName);
        
        /// <summary>
        /// 设置进度
        /// </summary>
        /// <param name="progress"></param>
        void SetProgress(float progress);
    }
}