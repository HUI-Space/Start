namespace Start
{
    public class LoadResourceTask : LoadResourceBaseTask
    {
        public override bool IsScene => false;
        public string AssetName { get; private set; }

        public static LoadResourceTask Create(string resourceName, string assetName = default, IAsyncOperationHandle asyncOperationHandle = default)
        {
            LoadResourceTask loadResourceTask = ReferencePool.Acquire<LoadResourceTask>();
            loadResourceTask.ResourceName = resourceName;
            loadResourceTask.AsyncOperationHandle = asyncOperationHandle;
            loadResourceTask.AssetName = assetName;
            return loadResourceTask;
        }

        public override void Clear()
        {
            base.Clear();
            AssetName = default;
            ResourceName = default;
            AsyncOperationHandle = default;
        }
    }
}