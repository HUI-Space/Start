using System;

namespace Start
{
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
        
        public event Action<AsyncOperationHandle<T>> OnComplete;

        public event Action<AsyncOperationHandle<T>> OnProgress;
        
        private RecycleTask<T> _recycleTask;
        public static AsyncOperationHandle<T> Create()
        {
            var asyncOperationHandle = ReferencePool.Acquire<AsyncOperationHandle<T>>();
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

        public void SetResult(object t)
        {
            if (t == null)
            {
                throw new Exception($"加载资源失败:{AssetName}");
            }

            if (t is T result)
            {
                Result = result;
                IsDone = true;
                Progress = 1f;
                OnComplete?.Invoke(this);
                _recycleTask?.SetResult(Result);
            }
            else
            {
                throw new Exception($"资源类型转换错误:{AssetName}");
            }
        }
        
        public void Clear()
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