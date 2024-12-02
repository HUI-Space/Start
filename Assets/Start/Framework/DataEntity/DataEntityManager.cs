using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Start.Framework
{
    public class DataEntityManager:ManagerBase<DataEntityManager>
    {
        public override int Priority => 8;
        
        private readonly Dictionary<Type, IDataEntityCollection> _dataEntityCollections = new Dictionary<Type, IDataEntityCollection>();

        public override Task Initialize()
        {
            Type[] types = AssemblyUtility.GetTypes();
            foreach (Type type in types)
            {
                if (type.IsClass && !type.IsAbstract && type.IsAssignableFrom(typeof(IDataEntityCollection)))
                {
                    IDataEntityCollection dataEntityCollection = Activator.CreateInstance(type) as IDataEntityCollection;
                    if (dataEntityCollection !=null)
                    {
                        dataEntityCollection.Initialize();
                        dataEntityCollection.Register();
                    }
                }
            }
            return Task.CompletedTask;
        }

        public override Task DeInitialize()
        {
            foreach (KeyValuePair<Type, IDataEntityCollection> item in _dataEntityCollections)
            {
                item.Value.UnRegister();
                item.Value.DeInitialize();
            }
            _dataEntityCollections.Clear();
            return base.DeInitialize();
        }
        
        public TValue GetDataEntity<TValue>() where TValue : DataEntity, new()
        {
            if (Instance._dataEntityCollections.TryGetValue(typeof(TValue), out IDataEntityCollection dataEntityCollection))
            {
                if (dataEntityCollection is DataEntityCollection<TValue> dataEntityCollection1)
                {
                    if (dataEntityCollection1.GetDataEntity(out TValue value))
                    {
                        return value;
                    }
                }
            }
            return default;
        }

        public TValue GetDataEntity<TKey, TValue>(TKey key) where  TValue : DataEntity<TKey>, new()
        {
            
            if (Instance._dataEntityCollections.TryGetValue(typeof(TValue), out IDataEntityCollection dataEntityCollection))
            {
                if (dataEntityCollection is DataEntityCollection<TKey, TValue> dataEntityCollection1)
                {
                    if (dataEntityCollection1.GetDataEntity(key, out TValue value))
                    {
                        return value;
                    }
                }
            }
            return default;
        }

        public void AddDataEntityCollection(Type type,IDataEntityCollection dataEntityCollection)
        {
            Instance._dataEntityCollections[type] = dataEntityCollection;
        }
        
        public void ResetAllCollection()
        {
            foreach (IDataEntityCollection collection in _dataEntityCollections.Values)
            {
                collection.Reset();
            }
        }
    }
}