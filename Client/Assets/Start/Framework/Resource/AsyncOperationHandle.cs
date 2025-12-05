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
        
        public RecycleTask<T> Task
        {
            get
            {
                if (_recycleTask == null)
                {
                    _recycleTask = RecycleTask<T>.Create();
                    if (IsDone)
                        _recycleTask.SetResult(Result);
                }

                return _recycleTask;
            }
        }
        
        private RecycleTask<T> _recycleTask;
        
        public event Action<AsyncOperationHandle<T>> OnComplete;
        
        public event Action<AsyncOperationHandle<T>> OnProgress;
        
        public static AsyncOperationHandle<T> Create()
        {
            var asyncOperationHandle = RecyclableObjectPool.Acquire<AsyncOperationHandle<T>>();
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
            _recycleTask?.SetResult(Result);
        }
        
        public void Reset()
        {
            IsDone = false;
            Progress = 0;
            Result = default;
            AssetName = null;
            ResourceName = null;
            EAsyncOperationStatus = default;
            _recycleTask = null;
        }
    }
}