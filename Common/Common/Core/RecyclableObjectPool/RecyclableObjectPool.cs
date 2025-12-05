using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Start
{
    /// <summary>
    /// 可回收对象池（全局统一入口，管理所有类型的可复用对象）
    /// </summary>
    public static class RecyclableObjectPool
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
        public static T Acquire<T>() where T : class, IReusable, new()
        {
            return GetReferenceCollection(typeof(T)).Acquire<T>();
        }

        /// <summary>
        /// 回收对象。
        /// </summary>
        /// <param name="reusable"></param>
        /// <exception cref="Exception"></exception>
        public static void Recycle(IReusable reusable)
        {
            if (reusable == null)
            {
                throw new Exception("Reusable is invalid."); 
            }
            Type type = reusable.GetType();
            if (!type.IsClass || type.IsAbstract)
            {
                throw new Exception("Reusable type is not a non-abstract class type.");
            }

            if (!typeof(IReusable).IsAssignableFrom(type))
            {
                throw new Exception($"Reusable type '{type.FullName}' is invalid.");
            }
            
            GetReferenceCollection(type).Recycle(reusable);
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
            if (_recyclablePoolBuckets.TryGetValue(type,out RecyclablePoolBucket recycleCollection))
            {
                recycleCollection = new RecyclablePoolBucket(type);
                _recyclablePoolBuckets.GetOrAdd(type, recycleCollection);
            }
            return recycleCollection;
        }
    }
}