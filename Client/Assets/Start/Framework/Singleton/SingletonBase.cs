namespace Start
{
    public abstract class SingletonBase<T> : ISingleton where T : SingletonBase<T>
    {
        public static T Instance => _instance ?? (_instance = SingletonManager.Instance.CreateSingleton<T>());
        private static T _instance;
        
        public virtual void Initialize()
        {
            
        }

        public virtual void DeInitialize()
        {
            
        }
    }
}