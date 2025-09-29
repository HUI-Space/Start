using System;
using System.IO;
using Common;

namespace Start
{
    public class ResourceHelper : IResourceHelper
    {
        public static IResourceLoader ResourceLoader { get; private set; }
        public static Type LoadResourceHelper => typeof(LoadResourceHelper);
        public static Type LoadResourceAgentHelper => typeof(LoadResourceAgentHelper);
        
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
            ResourceLoader = default;
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            ResourceLoader?.Update(elapseSeconds, realElapseSeconds);
        }

        public AsyncOperationHandle<T> LoadAsset<T>(string path)
        {
            return ResourceLoader?.LoadAsset<T>(path);
        }

        public AsyncOperationHandle<T> LoadAssetAsync<T>(string path)
        {
            return ResourceLoader?.LoadAssetAsync<T>(path);
        }

        public AsyncOperationHandle<T> LoadScene<T>(string path, bool isAdditive = true)
        {
            return ResourceLoader?.LoadScene<T>(path,isAdditive);
        }

        public AsyncOperationHandle<T> LoadSceneAsync<T>(string path, bool isAdditive = true)
        {
            return ResourceLoader?.LoadSceneAsync<T>(path,isAdditive);
        }

        public void Unload(IAsyncOperationHandle handle)
        {
            ResourceLoader?.Unload(handle);
        }

        public bool HasAsset(string assetName)
        {
            return ResourceLoader.HasAsset(assetName);
        }
    }
    
}