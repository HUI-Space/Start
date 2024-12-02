using System;

namespace Start.Framework
{
    public interface IAsyncOperationHandle:IReference
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

        event Action<IAsyncOperationHandle> OnComplete;
        void SetResult(string assetName ,object o);
        void SetResourceName(string resourceName);
        void SetStatus(EAsyncOperationStatus status);
        void SetProgress(float progress);
    }
}