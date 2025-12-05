using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Start
{
    public class ObjectPoolManager : ManagerBase<ObjectPoolManager>, IUpdateManger
    {
        public override int Priority => 2;
        public int ObjectPoolCount => _objectPools.Count;

        private List<IObjectPoolBase> _cachedAllObjectPools;
        private Dictionary<Type, IObjectPoolBase> _objectPools;

        public override Task Initialize()
        {
            _cachedAllObjectPools = new List<IObjectPoolBase>();
            _objectPools = new Dictionary<Type, IObjectPoolBase>();
            return Task.CompletedTask;
        }

        public bool HasObjectPool<TObject, TTarget>() 
            where TObject : ObjectBase<TTarget>, new()
            where TTarget : class
        {
            return HasObjectPool(typeof(TObject));
        }

        public IObjectPool<TObject, TTarget> GetObjectPool<TObject, TTarget>() 
            where TObject : ObjectBase<TTarget>, new()
            where TTarget : class
        {
            if (HasObjectPool(typeof(TObject)))
            {
                return (IObjectPool<TObject, TTarget>)_objectPools[typeof(TObject)];
            }

            return null;
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
        public IObjectPool<TObject, TTarget> CreateObjectPool<TObject, TTarget>(
            int capacity = int.MaxValue, 
            bool allowMultiSpawn = false,
            float autoReleaseInterval = float.MaxValue, 
            float expireTime = float.MaxValue, 
            int priority = 0)
            where TObject : ObjectBase<TTarget>, new()
            where TTarget : class
        {
            if (HasObjectPool(typeof(TObject)))
            {
                throw new Exception($"已经存在 ObjectPool '{typeof(TObject)}'.");
            }
            
            ObjectPool<TObject, TTarget> objectPool = RecyclableObjectPool.Acquire<ObjectPool<TObject, TTarget>>();
            objectPool.Initialize(capacity, allowMultiSpawn, autoReleaseInterval, expireTime, priority);
            _objectPools.Add(typeof(TObject), objectPool);
            return objectPool;
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            foreach (var objectPoolBase in _objectPools.Values)
            {
                objectPoolBase.Update(elapseSeconds, realElapseSeconds);
            }
        }


        public bool DestroyObjectPool<TObject, TTarget>() 
            where TObject : ObjectBase<TTarget>, new()
            where TTarget : class
        {
            if (_objectPools.TryGetValue(typeof(TObject), out IObjectPoolBase objectPool))
            {
                objectPool.DeInitialize();
                RecyclableObjectPool.Recycle(objectPool);
                return _objectPools.Remove(typeof(TObject));
            }

            return false;
        }

        /// <summary>
        /// 释放对象池中的可释放对象。
        /// </summary>
        public void Release()
        {
            GetAllObjectPools(true, _cachedAllObjectPools);
            foreach (IObjectPoolBase objectPool in _cachedAllObjectPools)
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
            foreach (IObjectPoolBase objectPool in _cachedAllObjectPools)
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
        private void GetAllObjectPools(bool sort, List<IObjectPoolBase> results)
        {
            if (results == null)
            {
                throw new Exception("结果为空.");
            }

            results.Clear();
            foreach (KeyValuePair<Type, IObjectPoolBase> objectPool in _objectPools)
            {
                results.Add(objectPool.Value);
            }

            if (sort)
            {
                results.Sort(ObjectPoolComparer);
            }
        }

        private static int ObjectPoolComparer(IObjectPoolBase a, IObjectPoolBase b)
        {
            return a.Priority.CompareTo(b.Priority);
        }

        public override Task DeInitialize()
        {
            foreach (KeyValuePair<Type, IObjectPoolBase> objectPool in _objectPools)
            {
                objectPool.Value.DeInitialize();
            }

            _objectPools.Clear();
            _cachedAllObjectPools.Clear();
            _cachedAllObjectPools = null;
            _objectPools = null;
            return base.DeInitialize();
        }
    }
}