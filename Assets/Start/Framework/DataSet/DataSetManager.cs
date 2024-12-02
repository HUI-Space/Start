using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Start.Framework
{
    [Manager]
    public class DataSetManager:ManagerBase<DataSetManager>
    {
        public override int Priority => 9;
        private readonly Dictionary<Type,DataSetBase> _dataSets = new Dictionary<Type,DataSetBase>();
        public override Task Initialize()
        {
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
        
        private DataSetBase GetDataSetBase(Type type)
        {
            if (!_dataSets.TryGetValue(type,out DataSetBase dataSet))
            {
                throw new Exception($"DataSetBase not found {type}!");
            }
            return dataSet;
        }
        
        private void Reset()
        {
            foreach (var dataSet in _dataSets)
            {
                dataSet.Value.Reset();
            }
        }
        
        private void Register()
        {
            Type[] types = AssemblyUtility.GetTypes();
            foreach (Type type in types)
            {
                if (type.BaseType == typeof(DataSetBase))
                {
                    _dataSets.Add(type,(DataSetBase) Activator.CreateInstance(type));
                }
            }
            
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