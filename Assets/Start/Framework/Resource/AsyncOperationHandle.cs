using System;

namespace Start.Framework
{
    public class AsyncOperationHandle<TObject> : IAsyncOperationHandle
    {
        private RecycleTaskCompletionSource<TObject> _recycleTaskCompletionSource;

        public Action<AsyncOperationHandle<TObject>> Completed;

        public TObject Result { get; private set; }

        public RecycleTaskCompletionSource<TObject> Task
        {
            get
            {
                if (_recycleTaskCompletionSource == null)
                {
                    _recycleTaskCompletionSource = RecycleTaskCompletionSource<TObject>.Create();
                    if (IsDone)
                        _recycleTaskCompletionSource.SetResult(Result);
                }

                return _recycleTaskCompletionSource;
            }
        }

        public bool IsDone { get; private set; }
        public float Progress { get; private set; }
        public string AssetName { get; private set; }
        public string ResourceName { get; private set; }
        
        public EAsyncOperationStatus EAsyncOperationStatus { get; private set; }
        public event Action<IAsyncOperationHandle> OnComplete;

        public void SetResult(string assetName, object o)
        {
            if (o == null) return;
            Result = (TObject)o;
            Progress = 1f;
            IsDone = true;
            Completed?.Invoke(this);
            _recycleTaskCompletionSource?.SetResult(Result);
        }

        public void SetResourceName(string resourceName)
        {
            ResourceName = resourceName;
        }

        public void SetStatus(EAsyncOperationStatus status)
        {
            EAsyncOperationStatus = status;
        }

        public void SetProgress(float progress)
        {
            Progress = progress;
        }

        public void Clear()
        {
            IsDone = default;
            Progress = default;
            Result = default;
            AssetName = default;
            ResourceName = default;
            EAsyncOperationStatus = default;
            if (_recycleTaskCompletionSource != null) ReferencePool.Release(_recycleTaskCompletionSource);
            _recycleTaskCompletionSource = default;
        }

        public static AsyncOperationHandle<TObject> Create()
        {
            var asyncOperationHandle = ReferencePool.Acquire<AsyncOperationHandle<TObject>>();
            return asyncOperationHandle;
        }
    }
}