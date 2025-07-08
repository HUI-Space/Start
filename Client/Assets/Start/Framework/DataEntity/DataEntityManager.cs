using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Start
{
    public class DataEntityManager : ManagerBase<DataEntityManager>
    {
        public override int Priority => 8;

        private readonly Dictionary<Type, IDataEntityCollection> _dataEntityCollections =
            new Dictionary<Type, IDataEntityCollection>();

        public override Task Initialize()
        {
            List<Type> types = AssemblyUtility.GetChildType(typeof(IDataEntityCollection));
            foreach (Type type in types)
            {
                if (!type.IsAbstract && type.BaseType is {IsGenericType: true } &&
                    Activator.CreateInstance(type) is IDataEntityCollection dataEntityCollection)
                {
                    Type baseType = type.BaseType;
                    Type genericTypeDefinition = baseType.GetGenericTypeDefinition();
                    Type[] genericArguments = baseType.GetGenericArguments();
                    if (genericArguments.Length == 1)
                    {
                        dataEntityCollection.Initialize();
                        _dataEntityCollections.Add(genericArguments[0], dataEntityCollection);
                    }
                    if (genericArguments.Length == 2)
                    {
                        dataEntityCollection.Initialize();
                        _dataEntityCollections.Add(genericArguments[1], dataEntityCollection);
                    }
                }
            }
            return Task.CompletedTask;
        }

        public override Task DeInitialize()
        {
            foreach (KeyValuePair<Type, IDataEntityCollection> item in _dataEntityCollections)
            {
                item.Value.DeInitialize();
            }
            _dataEntityCollections.Clear();
            return base.DeInitialize();
        }

        public TValue GetDataEntity<TValue>() where TValue : DataEntity, new()
        {
            if (_dataEntityCollections.TryGetValue(typeof(TValue), out IDataEntityCollection dataEntityCollection))
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

        public TValue GetDataEntity<TKey, TValue>(TKey key) where TValue : DataEntity<TKey>, new()
        {
            if (_dataEntityCollections.TryGetValue(typeof(TValue),
                out IDataEntityCollection dataEntityCollection))
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
        
        public void ResetAllCollection()
        {
            foreach (IDataEntityCollection collection in _dataEntityCollections.Values)
            {
                collection.Reset();
            }
        }
    }
}