using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Start
{
    public class ConfigManager : ManagerBase<ConfigManager>
    {
        public override int Priority => 6;
        
        private IConfigHelper _configHelper;
        private readonly Dictionary<Type, IConfig> _configs = new Dictionary<Type, IConfig>();

        public override Task Initialize()
        {
            _configHelper = Helper.CreateHelper<IConfigHelper>();
            return base.Initialize();
        }
        
        public override Task DeInitialize()
        {
            _configHelper = default;
            foreach (IConfig config in _configs.Values)
            {
                config.DeInitialize();
            }
            _configs.Clear();
            return base.DeInitialize();
        }
        
        public T GetConfig<T>() where T : class, IConfig
        {
            return (T)GetConfig(typeof(T));
        }
        
        public IConfig GetConfig(Type type)
        {
            if (!_configs.TryGetValue(type,out IConfig config))
            {
                config = _configHelper.LoadConfig(type);
                config.Initialize();
                _configs[type] = config;
            }
            return config;
        }
    }
}