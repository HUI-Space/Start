using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Start
{
    public class SingletonMonoBehaviourController : SingletonBase<SingletonMonoBehaviourController>
    {
        private readonly Dictionary<Type, ISingleton> _singletonDic = new Dictionary<Type, ISingleton>();
        
        public T CreateSingletonMonoBehaviour<T>() where T : SingletonMonoBehaviourBase<T>
        {
            T t = new GameObject(nameof(T)).AddComponent<T>();
            _singletonDic.Add(typeof(T), t);
            t.gameObject.name = t.Name;
            t.transform.SetParent(t.Parent);
            t.Initialize();
            return t;
        }

        public void RegisterSingleton(ISingleton t)
        {
            _singletonDic.Add(t.GetType(), t);
            t.Initialize();
        }

        public override void DeInitialize()
        {
            foreach (ISingleton singleton in _singletonDic.Values)
            {
                singleton.DeInitialize();
                if (singleton is MonoBehaviour go)
                {
                    Object.Destroy(go);
                }
            }
            _singletonDic.Clear();
            base.DeInitialize();
        }
    }
}