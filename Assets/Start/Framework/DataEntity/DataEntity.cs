using System;
using System.Collections.Generic;

namespace Start.Framework
{
    
    public abstract class DataEntity<TKey> : DataEntity
    {
        protected TKey Key;

        public void SetKey(TKey key)
        {
            Key = key;
        }
    }
    
    public abstract class DataEntity: IDataEntity
    {
        private readonly HashSet<Action> _netUpdate = new HashSet<Action>();
        private readonly Dictionary<DataSetBase, int> _recordVersionIndex = new Dictionary<DataSetBase, int>();
        private readonly List<int>                    _recordVersion      = new List<int>();
        
        public abstract void Initialize();
        

        public void RegisterNetUpdate<TNet>(TNet net, Action<TNet> callback) where TNet : DataSetBase
        {
            _recordVersionIndex[net] = _recordVersion.Count;
            _recordVersion.Add(0);

            _netUpdate.Add(CheckUpdate);

            void CheckUpdate()
            {
                if (_recordVersion[_recordVersionIndex[net]] != net.Version)
                {
                    try
                    {
                        _recordVersion[_recordVersionIndex[net]] = net.Version;
                        callback(net);
                    }
                    catch (Exception e)
                    {
                        /*Debug.LogError($"二级数据{GetType()}更新失败,DataSet:{net.GetType()},错误信息:{e}");*/
                    }
                }
            }
        }

        public abstract void ResetTempData();
        
        public void TryRefresh()
        {
            ResetTempData();
            foreach (Action action in _netUpdate)
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    /*Debug.LogError(e);*/
                }
            }
        }

        public void Reset()
        {
            for (int i = 0; i < _recordVersion.Count; i++)
            {
                _recordVersion[i] = 0;
            }
        }

        public abstract void DeInitialize();
    }
}