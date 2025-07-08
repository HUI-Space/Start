using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Start
{
    public class DataSetManager : ManagerBase<DataSetManager>
    {
        public override int Priority => 9;
        private readonly Dictionary<Type, DataSetBase> _dataSets = new Dictionary<Type, DataSetBase>();

        public override Task Initialize()
        {
            List<Type> types = AssemblyUtility.GetChildType(typeof(DataSetBase));
            foreach (Type type in types)
            {
                if (Activator.CreateInstance(type) is DataSetBase dataSet)
                {
                    _dataSets.Add(type, dataSet);
                }
            }
            Register();
            return Task.CompletedTask;
        }

        public override Task DeInitialize()
        {
            Reset();
            UnRegister();
            return base.DeInitialize();
        }

        public T GetDataSet<T>() where T : DataSetBase
        {
            return (T)GetDataSetBase(typeof(T));
        }

        public void Reset()
        {
            foreach (var dataSet in _dataSets)
            {
                dataSet.Value.Reset();
            }
        }
        
        public DataSetBase GetDataSetBase(Type type)
        {
            if (!_dataSets.TryGetValue(type, out DataSetBase dataSet))
            {
                throw new Exception($"DataSetBase 没有找到，它的类型为： {type}!");
            }
            return dataSet;
        }
        
        private void Register()
        {
            foreach (var dataSet in _dataSets)
            {
                dataSet.Value.Register();
            }
        }

        private void UnRegister()
        {
            foreach (var dataSet in _dataSets)
            {
                dataSet.Value.UnRegister();
            }
        }
    }
}