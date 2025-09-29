namespace Start
{
    public interface IResourceLoader
    {
        void Initialize();
        
        void DeInitialize();
        
        AsyncOperationHandle<T> LoadAsset<T>(string path);
        
        AsyncOperationHandle<T> LoadAssetAsync<T>(string path);
        
        AsyncOperationHandle<T> LoadScene<T>(string path,bool isAdditive = true);
        
        AsyncOperationHandle<T> LoadSceneAsync<T>(string path,bool isAdditive = true);
        
        void Update(float elapseSeconds,float realElapseSeconds);

        bool HasAsset(string assetName);
        
        void Unload(IAsyncOperationHandle handle);
    }
}