using System.Collections.Generic;

namespace Start
{
    public abstract class DataEntityCollection<TValue> : IDataEntityCollection where TValue : DataEntity, new()
    {
        private TValue _value;

        public virtual void Initialize()
        {
            
        }

        public virtual void DeInitialize()
        {
            _value?.DeInitialize();
            _value = default;
        }

        public bool GetDataEntity(out TValue value)
        {
            if (_value == null)
            {
                _value = new TValue();
                _value.Initialize();
            }

            _value.TryRefresh();
            value = _value;
            return true;
        }

        public virtual void Reset()
        {
            _value?.Reset();
        }
    }

    public abstract class DataEntityCollection<TKey, TValue> : IDataEntityCollection
        where TValue : DataEntity<TKey>, new()
    {
        private readonly Dictionary<TKey, TValue> _dataEntity = new Dictionary<TKey, TValue>();
        private readonly HashSet<TKey> _pool = new HashSet<TKey>();

        public virtual void Initialize()
        {
            Register();
        }

        public virtual void Reset()
        {
            foreach (KeyValuePair<TKey, TValue> pair in _dataEntity)
            {
                pair.Value.Reset();
            }
        }

        public virtual void DeInitialize()
        {
            foreach (KeyValuePair<TKey, TValue> pair in _dataEntity)
            {
                pair.Value.DeInitialize();
            }
            _dataEntity.Clear();
            UnRegister();
        }

        public bool GetDataEntity(TKey key, out TValue value)
        {
            value = default;
            if (HasKey(key) == false)
            {
                return false;
            }

            if (!_dataEntity.TryGetValue(key, out value))
            {
                value = new TValue();
                value.Initialize();
                value.SetKey(key);
                _dataEntity[key] = value;
            }
            value.TryRefresh();
            return true;
        }

        /// <summary>
        /// 检查Key是否合法
        /// </summary>
        /// <returns></returns>
        protected virtual bool HasKey(TKey key)
        {
            return _pool.Contains(key);
        }

        protected virtual void CheckAllKey(uint code, object msg)
        {
            if (code != 0) return;
            _pool.Clear();
            foreach (TKey key in _dataEntity.Keys)
            {
                if (HasKey(key) == false)
                {
                    _pool.Add(key);
                }
            }
            foreach (TKey key in _pool)
            {
                _dataEntity.Remove(key);
            }
        }

        /// <summary>
        /// 注册数据实体集合，使其可以参与数据管理和处理流程
        /// </summary>
        protected virtual void Register()
        {
        }

        /// <summary>
        /// 注销数据实体集合，使其不再参与数据管理和处理流程
        /// </summary>
        protected virtual void UnRegister()
        {
        }

        protected void RegisterNetMessage<T>(uint channelId, uint messageId) where T : class
        {
            SocketManager.Instance.Register<T>(channelId, messageId, CheckAllKey, 1);
        }

        protected void UnRegisterNetMessage<T>(uint channelId, uint messageId) where T : class
        {
            SocketManager.Instance.UnRegister<T>(channelId, messageId, CheckAllKey);
        }
    }
}