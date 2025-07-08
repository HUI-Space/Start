using UnityEngine;

namespace Start
{
    public abstract class SingletonMonoBehaviourBase<T> : MonoBehaviour ,ISingleton where T : SingletonMonoBehaviourBase<T>
    {
        public static T Instance 
        {
            get
            {
                if (_instance == null)
                {
                    _instance = SingletonMonoBehaviourController.Instance.CreateSingletonMonoBehaviour<T>();
                }
                return _instance;
            }
        }
        
        private static T _instance;
        public virtual string Name => typeof(T).Name;
        public virtual GameObject Parent => Main.Root != null ? Main.Root.gameObject : null;


        public virtual void Initialize()
        {
            
        }

        public virtual void DeInitialize()
        {
            
        }
    }
}