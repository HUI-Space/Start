using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Start.Framework
{
    [Manager]
    public class ConfigManager : ManagerBase<ConfigManager>
    {
        private readonly Dictionary<Type, IConfig> _configs = new Dictionary<Type, IConfig>();
        private IConfigHelper iConfigHelper;
        public override int Priority => 6;

        public override Task DeInitialize()
        {
            iConfigHelper = default;
            foreach (IConfig config in _configs.Values)
            {
                config.DeInitialize();
            }
            _configs.Clear();
            return base.DeInitialize();
        }
        
        /// <summary>
        /// 该接口的主要作用是传入对应对象的接口到对应的Manager中
        /// </summary>
        /// <param name="configHelper"></param>
        public static void SetHelper(IConfigHelper configHelper)
        {
            Instance.iConfigHelper = configHelper;;
        }
        
        public T GetConfig<T>() where T : class, IConfig
        {
            return (T)GetConfig<T>(typeof(T));
        }
        
        private IConfig GetConfig<T>(Type type)where T : class, IConfig
        {
            if (!_configs.TryGetValue(type,out IConfig config))
            {
                config = iConfigHelper.LoadConfig(type);
                config.Initialize();
                _configs[type] = config;
            }
            return config;
        }
    }
}