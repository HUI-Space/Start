namespace Start
{
    public class ResourceHelper : IResourceHelper
    {
        public static IResourceLoader ResourceLoader { get; private set; }
        
        public void Initialize()
        {
#if UNITY_EDITOR
            if (GameConfig.EnableAssetbundle == false)
            {
                ResourceLoader = new EditorResourceLoader();
            }
            else
#endif
            {
                ResourceLoader = new ResourceLoader();
            }
            ResourceLoader.Initialize();
        }

        public void DeInitialize()
        {
            ResourceLoader.DeInitialize();
            ResourceLoader = null;
        }

        public AsyncOperationHandle<T> LoadAsset<T>(string path)
        {
            return ResourceLoader?.LoadAsset<T>(path);
        }

        public AsyncOperationHandle<T> LoadAssetAsync<T>(string path)
        {
            return ResourceLoader?.LoadAssetAsync<T>(path);
        }

        public AsyncOperationHandle<T> LoadScene<T>(string sceneName, bool isAdditive = true)
        {
            return ResourceLoader?.LoadScene<T>(sceneName,isAdditive);
        }

        public AsyncOperationHandle<T> LoadSceneAsync<T>(string path, bool isAdditive = true)
        {
            return ResourceLoader?.LoadSceneAsync<T>(path,isAdditive);
        }
        
        public void UnloadScene(IAsyncOperationHandle handle)
        {
            ResourceLoader?.Unload(handle);
        }

        public void UnloadScene(string sceneName)
        {
            ResourceLoader?.UnloadScene(sceneName);
        }
        
        public void Unload(IAsyncOperationHandle handle)
        {
            ResourceLoader?.Unload(handle);
        }

        public void Unload(string assetName)
        {
            ResourceLoader?.Unload(assetName);
        }
        public bool HasAsset(string assetName)
        {
            return ResourceLoader.HasAsset(assetName);
        }
    }
}