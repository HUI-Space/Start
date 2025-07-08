using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Start
{
    public class SingletonManager : ManagerBase<SingletonManager>
    {
        public override int Priority => 0;

        private readonly Dictionary<Type, ISingleton> _singletonDic = new Dictionary<Type, ISingleton>();

        public T CreateSingleton<T>() where T : ISingleton
        {
            T singleton = Activator.CreateInstance<T>();
            _singletonDic.Add(typeof(T), singleton);
            singleton.Initialize();
            return singleton;
        }

        public override Task DeInitialize()
        {
            foreach (ISingleton singleton in _singletonDic.Values)
            {
                singleton.DeInitialize();
            }
            _singletonDic.Clear();
            return base.DeInitialize();
        }
    }
}