using System;
using System.Collections.Generic;

namespace Start
{
    [Serializable]
    public abstract class ConfigBase<T> : IConfig where T : IConfigItem
    {
        public List<T> DataList = new List<T>();
        private Dictionary<long, T> _dataDictionary;

        public virtual void Initialize()
        {
            _dataDictionary = new Dictionary<long, T>();
            foreach (var item in DataList)
            {
                if (item is ConfigItemBase configItem)
                {
                    if (_dataDictionary.ContainsKey(configItem.Id))
                    {
                        continue;
                    }
                    _dataDictionary.Add(configItem.Id, item);
                }
            }
        }

        public bool ContainsId(long id)
        {
            return _dataDictionary.ContainsKey(id);
        }

        public T GetValueById(long id)
        {
            if (!_dataDictionary.TryGetValue(id, out var item))
            {
            }
            return item;
        }

        public virtual void DeInitialize()
        {
            DataList.Clear();
            DataList = default;
            _dataDictionary.Clear();
            _dataDictionary = default;
        }
    }
}