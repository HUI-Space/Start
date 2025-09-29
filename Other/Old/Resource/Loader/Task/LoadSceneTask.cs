namespace Start
{
    public class LoadSceneTask : LoadResourceBaseTask
    {
        public override bool IsScene => true;
        public string SceneName { get; private set; }
        public bool IsAdditive { get; private set; }

        public static LoadSceneTask Create(string resourceName, string sceneName, bool isAdditive, IAsyncOperationHandle asyncOperationHandle)
        {
            LoadSceneTask loadResourceTask = ReferencePool.Acquire<LoadSceneTask>();
            loadResourceTask.ResourceName = resourceName;
            loadResourceTask.SceneName = sceneName;
            loadResourceTask.IsAdditive = isAdditive;
            loadResourceTask.AsyncOperationHandle = asyncOperationHandle;
            return loadResourceTask;
        }

        public override void Clear()
        {
            base.Clear();
            SceneName = default;
            ResourceName = default;
            AsyncOperationHandle = default;
        }
    }
}