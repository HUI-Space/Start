using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Start
{
    /// <summary>
    /// 可回收对象池（全局统一入口，管理所有类型的可复用对象）
    /// </summary>
    public static class RecyclablePool
    {
        /// <summary>
        /// 获取对象池的数量。
        /// </summary>
        public static int Count => _recyclablePoolBuckets.Count;
        
        private static readonly ConcurrentDictionary<Type,RecyclablePoolBucket> _recyclablePoolBuckets = new ConcurrentDictionary<Type, RecyclablePoolBucket>();
        
        /// <summary>
        /// 获取所有池的信息。
        /// </summary>
        /// <returns></returns>
        public static RecyclablePoolStats[] GetAllPoolInfos()
        {
            var infos = new List<RecyclablePoolStats>();
            foreach (var kv in _recyclablePoolBuckets)
            {
                var info = kv.Value.GetRecyclablePoolStats();
                infos.Add(info);
            }
            return infos.ToArray();
        }
        
        /// <summary>
        /// 清空所有对象池。
        /// </summary>
        public static void ClearAll()
        {
            foreach (var kv in _recyclablePoolBuckets)
            {
                kv.Value.Clear();
            }
            _recyclablePoolBuckets.Clear();
        }
        
        /// <summary>
        /// 从对象池获取对象。
        /// </summary>
        /// <typeparam name="T">引用类型。</typeparam>
        /// <returns>引用。</returns>
        public static T Acquire<T>() where T : class, IRecycle, new()
        {
            return GetReferenceCollection(typeof(T)).Acquire<T>();
        }

        /// <summary>
        /// 回收对象。
        /// </summary>
        /// <param name="recycle"></param>
        /// <exception cref="Exception"></exception>
        public static void Recycle(IRecycle recycle)
        {
            if (recycle == null)
                throw new ArgumentNullException(nameof(recycle)); 
    
            Type type = recycle.GetType();
            if (!type.IsClass || type.IsAbstract)
                throw new InvalidOperationException($"Type '{type}' must be a non-abstract class.");  
    
            if (!typeof(IRecycle).IsAssignableFrom(type))
                throw new InvalidCastException($"Type '{type}' does not implement IRecycle.");
    
            GetReferenceCollection(type).Recycle(recycle);
        }
        
        /// <summary>
        /// 添加对象。
        /// </summary>
        /// <param name="count"></param>
        /// <typeparam name="T"></typeparam>
        public static void Add<T>(int count)
        {
            GetReferenceCollection(typeof(T)).Add(count);
        }
        
        /// <summary>
        /// 移除对象。
        /// </summary>
        /// <param name="count"></param>
        /// <typeparam name="T"></typeparam>
        public static void Remove<T>(int count)
        {
            GetReferenceCollection(typeof(T)).Remove(count);
        }
        
        private static RecyclablePoolBucket GetReferenceCollection(Type type)
        {
            if (_recyclablePoolBuckets.TryGetValue(type, out var bucket))
            {
                return bucket;
            }
    
            return _recyclablePoolBuckets.GetOrAdd(type, new RecyclablePoolBucket(type));
        }
    }
}