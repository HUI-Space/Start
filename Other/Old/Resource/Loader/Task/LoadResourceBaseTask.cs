namespace Start
{
    public abstract class LoadResourceBaseTask : TaskBase
    {
        public abstract bool IsScene { get; }
        public string ResourceName { get; protected set; }
        protected IAsyncOperationHandle AsyncOperationHandle { get; set; }
        
        public void OnLoadAssetComplete(string assetName,object asset)
        {
            AsyncOperationHandle?.SetResult(asset);
        }
        
        public void OnLoadResourceComplete()
        {
            AsyncOperationHandle?.SetResourceName(ResourceName);
        }
        
        public void OnLoadResourceProgress(float progress)
        {
            AsyncOperationHandle?.SetProgress(progress);
        }
        
        public void OnLoadResourceStatusType(EAsyncOperationStatus asyncOperationStatus)
        {
            AsyncOperationHandle?.SetStatus(asyncOperationStatus);
        }
    }
}