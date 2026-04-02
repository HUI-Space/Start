using System;

namespace Start
{
    /// <summary>
    /// 资源加载操作句柄，可实现通过同步加载以及异步加载
    /// 需要注意的是，如果该句柄被某处使用时候，释放该句柄后需要置空
    /// 如果没有使用该句柄，既可以通过资源管理器通过资源名称进行释放句柄，无需管理对应的句柄操作
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncOperationHandle<T> : IAsyncOperationHandle
    {
        public T Result { get; private set; }
        
        public bool IsDone { get; private set; }
        
        public float Progress { get; private set; }
        
        public string AssetName { get; private set; }
        
        public string ResourceName { get; private set; }
        
        public EAsyncOperationStatus EAsyncOperationStatus { get; private set; }
        
        public StructTask<T> Task
        {
            get
            {
                if (!_structTask.IsValid)
                {
                    _structTask = StructTask<T>.Create();
                    if (IsDone)
                        _structTask.SetResult(Result);
                }

                return _structTask;
            }
        }
        
        private StructTask<T> _structTask;
        
        public event Action<AsyncOperationHandle<T>> OnComplete;
        
        public event Action<AsyncOperationHandle<T>> OnProgress;
        
        public static AsyncOperationHandle<T> Create()
        {
            AsyncOperationHandle<T> asyncOperationHandle = RecyclablePool.Acquire<AsyncOperationHandle<T>>();
            return asyncOperationHandle;
        }
        
        public void SetStatus(EAsyncOperationStatus status)
        {
            EAsyncOperationStatus = status;
        }

        public void SetResourceName(string resourceName)
        {
            ResourceName = resourceName;
        }

        public void SetAssetName(string assetName)
        {
            AssetName = assetName;
        }

        public void SetProgress(float progress)
        {
            Progress = progress;
            OnProgress?.Invoke(this);
        }

        public void SetResult(T t)
        {
            if (t == null)
            {
                throw new Exception($"加载资源失败:{AssetName}");
            }
            Result = t;
            IsDone = true;
            Progress = 1f;
            OnComplete?.Invoke(this);
            if (_structTask.IsValid)
            {
                _structTask.SetResult(Result);
            }
        }
        
        public void Recycle()
        {
            IsDone = false;
            Progress = 0;
            Result = default;
            AssetName = null;
            ResourceName = null;
            EAsyncOperationStatus = default;
            _structTask = default;
            OnComplete = null;
            OnProgress = null;
        }
    }
}
