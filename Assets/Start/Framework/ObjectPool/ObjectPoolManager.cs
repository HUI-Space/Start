using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Start.Framework
{
    [Manager]
    public class ObjectPoolManager:ManagerBase<ObjectPoolManager>,IUpdateManger
    {
        public override int Priority => 2;
        public int ObjectPoolCount => _objectPools.Count;
        
        private List<ObjectPoolBase> _cachedAllObjectPools;
        private Dictionary<Type, ObjectPoolBase> _objectPools;
        
        public override Task Initialize()
        {
            _cachedAllObjectPools = new List<ObjectPoolBase>();
            _objectPools = new Dictionary<Type, ObjectPoolBase>();
            return Task.CompletedTask;
        }

        public bool HasObjectPool<T>() where T : ObjectBase
        {
            return HasObjectPool(typeof(T));
        }
        
        public IObjectPool<T> GetObjectPool<T>() where T : ObjectBase
        {
            if (HasObjectPool(typeof(T)))
            {
                return (IObjectPool<T>)_objectPools[typeof(T)];
            }
            return default;
        }
        
        /// <summary>
        /// 创建对象池
        /// </summary>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="allowMultiSpawn">允许多次获取</param>
        /// <param name="autoReleaseInterval">对象池自动释放可释放对象的间隔秒数</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public IObjectPool<T> CreateObjectPool<T>(int capacity = int.MaxValue, bool allowMultiSpawn = false, float autoReleaseInterval = float.MaxValue, float expireTime = float.MaxValue, int priority = 0) where T : ObjectBase
        {
            if (HasObjectPool(typeof(T)))
            {
                throw new Exception($"Already exist ObjectPool '{typeof(T)}'.");
            }

            ObjectPool<T> objectPool = new ObjectPool<T>();
            objectPool.Initialize(capacity, allowMultiSpawn, autoReleaseInterval, expireTime, priority);
            _objectPools.Add(typeof(T),objectPool);
            return objectPool;
        }
        
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            foreach (var objectPoolBase in _objectPools.Values)
            {
                objectPoolBase.Update(elapseSeconds,realElapseSeconds);
            }
        }
        

        public bool DestroyObjectPool<T>() where T : ObjectBase
        {
            if (_objectPools.TryGetValue(typeof(T), out ObjectPoolBase objectPool))
            {
                objectPool.DeInitialize();
                ReferencePool.Release((IObjectPool<T>)objectPool);
                return _objectPools.Remove(typeof(T));
            }
            return default;
        }

        /// <summary>
        /// 释放对象池中的可释放对象。
        /// </summary>
        public void Release()
        {
            GetAllObjectPools(true, _cachedAllObjectPools);
            foreach (ObjectPoolBase objectPool in _cachedAllObjectPools)
            {
                objectPool.Release();
            }
        }

        /// <summary>
        /// 释放对象池中的所有未使用对象。
        /// </summary>
        public void ReleaseAllUnused()
        {
            GetAllObjectPools(true, _cachedAllObjectPools);
            foreach (ObjectPoolBase objectPool in _cachedAllObjectPools)
            {
                objectPool.ReleaseAllUnused();
            }
        }
        private bool HasObjectPool(Type objectType)
        {
            return _objectPools.ContainsKey(objectType);
        }
        
        /// <summary>
        /// 获取所有对象池。
        /// </summary>
        /// <param name="sort">是否根据对象池的优先级排序。</param>
        /// <param name="results">所有对象池。</param>
        private void GetAllObjectPools(bool sort, List<ObjectPoolBase> results)
        {
            if (results == null)
            {
                throw new Exception("Results is invalid.");
            }

            results.Clear();
            foreach (KeyValuePair<Type, ObjectPoolBase> objectPool in _objectPools)
            {
                results.Add(objectPool.Value);
            }

            if (sort)
            {
                results.Sort(ObjectPoolComparer);
            }
        }
        private static int ObjectPoolComparer(ObjectPoolBase a, ObjectPoolBase b)
        {
            return a.Priority.CompareTo(b.Priority);
        }

        public override Task DeInitialize()
        {
            foreach (KeyValuePair<Type, ObjectPoolBase> objectPool in _objectPools)
            {
                objectPool.Value.DeInitialize();
            }
            _objectPools.Clear();
            _cachedAllObjectPools.Clear();
            _cachedAllObjectPools = default;
            _objectPools = default;
            return base.DeInitialize();
        }
    }
    
}