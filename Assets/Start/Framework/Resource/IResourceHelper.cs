namespace Start.Framework
{
    public interface IResourceHelper
    {
        void Initialize();
        void DeInitialize();
        void Update(float elapseSeconds, float realElapseSeconds);
        AsyncOperationHandle<T> LoadAsset<T>(string path);
        AsyncOperationHandle<T> LoadAssetAsync<T>(string path);
        AsyncOperationHandle<T> LoadScene<T>(string path,bool isAdditive = true);
        AsyncOperationHandle<T> LoadSceneAsync<T>(string path,bool isAdditive = true);
        void UnloadAsset(IAsyncOperationHandle handle);
        bool HasAsset(string assetName);
        bool HasResource(string resourceName);
    }
}