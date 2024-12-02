using System.Collections.Generic;

namespace Start.Framework
{
    public abstract class DataEntityCollection<TValue> : DataEntityCollectionBase where TValue : DataEntity, new()
    {
        private TValue _value;

        public override void Initialize()
        {
            DataEntityManager.Instance.AddDataEntityCollection(typeof(TValue),this);
        }
        
        public override void DeInitialize()
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
        
        public override void Reset()
        {
            _value?.Reset();
        }
    }
    
    public abstract class DataEntityCollection<TKey, TValue> :DataEntityCollectionBase where TValue : DataEntity<TKey>, new()
    {
        private readonly Dictionary<TKey, TValue> _dataEntity = new Dictionary<TKey, TValue>();
        private readonly HashSet<TKey> _pool = new HashSet<TKey>();
        public override void Initialize()
        {
            DataEntityManager.Instance.AddDataEntityCollection(typeof(TValue),this);
        }
        
        public override void DeInitialize()
        {
            foreach (KeyValuePair<TKey, TValue> pair in _dataEntity)
            {
                pair.Value.DeInitialize();
            }
            _dataEntity.Clear();
        }
        
        public bool GetDataEntity(TKey key,out TValue value)
        {
            value = default;
            if (HasKey(key) == false)
            {
                return false;
            }

            if (!_dataEntity.TryGetValue(key,out value))
            {
                value = new TValue();
                value.Initialize();
                value.SetKey(key);
                _dataEntity[key] = value;
            }
            value.TryRefresh();
            return true;
        }

        protected override void CheckAllKey(uint code, object msg)
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
        /// 检查Key是否合法
        /// </summary>
        /// <returns></returns>
        protected abstract bool HasKey(TKey key);
        
        public override void Reset()
        {
            foreach (KeyValuePair<TKey, TValue> pair in _dataEntity)
            {
                pair.Value.Reset();
            }
        }
    }
}