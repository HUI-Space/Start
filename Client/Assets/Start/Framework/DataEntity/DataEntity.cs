using System;
using System.Collections.Generic;

namespace Start
{
    public abstract class DataEntity<TKey> : DataEntity
    {
        public TKey Key;

        public void SetKey(TKey key)
        {
            Key = key;
        }
    }

    public abstract class DataEntity : IDataEntity
    {
        private readonly Dictionary<DataSetBase, int> _recordVersionIndex = new Dictionary<DataSetBase, int>();
        private readonly HashSet<Action> _netUpdate = new HashSet<Action>();
        private readonly List<int> _recordVersion = new List<int>();

        public virtual void Initialize()
        {
            
        }

        public virtual void ResetTempData()
        {
            
        }

        public virtual void DeInitialize()
        {
            _recordVersionIndex.Clear();
            _recordVersion.Clear();
            _netUpdate.Clear();
        }
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
                    Logger.Error(e.ToString());
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

        protected void RegisterNetUpdate<TNet>(TNet net, Action<TNet> callback) where TNet : DataSetBase
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
                        Logger.Error($"二级数据{GetType()}更新失败,DataSet:{net.GetType()},错误信息:{e}");
                    }
                }
            }
        }
    }
}