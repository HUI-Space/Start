namespace Start
{
    public class SceneManager : ManagerBase<SceneManager>
    {
        public override int Priority => 10;
        
        public AsyncOperationHandle<T> LoadScene<T>(string sceneName, bool isAdditive = true)
        {
            return ResourceManager.Instance.LoadScene<T>(sceneName, isAdditive);
        }

        public AsyncOperationHandle<T> LoadSceneAsync<T>(string sceneName, bool isAdditive = true)
        {
            return ResourceManager.Instance.LoadSceneAsync<T>(sceneName, isAdditive);
        }

        public void UnloadScene<T>(AsyncOperationHandle<T> handle)
        {
            ResourceManager.Instance.UnloadScene(handle);
        }
        
        public void UnloadScene(string sceneName)
        {
            ResourceManager.Instance.UnloadScene(sceneName);
        }
    }
}