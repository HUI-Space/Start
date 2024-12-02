namespace Start.Framework
{
    [Manager]
    public class SceneManager:ManagerBase<SceneManager>
    {
        public override int Priority => 10;
        
        public AsyncOperationHandle<T> LoadScene<T>(string sceneName,bool isAdditive = true)
        {
            return ResourceManager.Instance.LoadScene<T>(sceneName,isAdditive);
        }
        
        public AsyncOperationHandle<T> LoadSceneAsync<T>(string sceneName,bool isAdditive = true) 
        {
            return ResourceManager.Instance.LoadSceneAsync<T>(sceneName,isAdditive);
        }
        
        public void UnloadScene(IAsyncOperationHandle handle)
        {
            ResourceManager.Instance.UnloadAsset(handle);
        }
    }
}