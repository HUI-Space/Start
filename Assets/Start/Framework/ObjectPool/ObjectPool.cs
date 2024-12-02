using System;
using System.Collections.Generic;
using UnityEngine;

namespace Start.Framework
{
    /// <summary>
    /// 释放对象筛选函数。
    /// </summary>
    /// <typeparam name="T">对象类型。</typeparam>
    /// <param name="candidateObjects">要筛选的对象集合。</param>
    /// <param name="toReleaseCount">需要释放的对象数量。</param>
    /// <param name="expireTime">对象过期参考时间。</param>
    /// <returns>经筛选需要释放的对象集合。</returns>
    public delegate List<T> ReleaseObjectFilterCallback<T>(List<T> candidateObjects, int toReleaseCount, DateTime expireTime) where T : ObjectBase;
    public sealed class ObjectPool<T> : ObjectPoolBase, IObjectPool<T> where T : ObjectBase
    {
        public Type ObjectType => typeof(T);
        public int Count => _objectMap.Count;
        
        public int CanReleaseCount
        {
            get
            {
                GetCanReleaseObjects(_cachedCanReleaseObjects);
                return _cachedCanReleaseObjects.Count;
            }
        }
        public int Capacity { get; private set; }
        
        public bool AllowMultiSpawn { get; private set; }
        
        public float AutoReleaseInterval { get; private set; }
        
        public float ExpireTime { get; private set; }
        
        public override int Priority { get; set; }

        /// <summary>
        /// 自动释放时间秒。
        /// </summary>
        private float _autoReleaseTime;
        private readonly List<T> _cachedCanReleaseObjects = new List<T>();
        private readonly List<T> _cachedToReleaseObjects = new List<T>();
        private readonly Dictionary<string, Object<T>> _objects = new Dictionary<string, Object<T>>();
        private readonly Dictionary<object, Object<T>> _objectMap = new Dictionary<object, Object<T>>();
        
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
        public void Initialize(int capacity, bool allowMultiSpawn, float autoReleaseInterval, float expireTime, int priority)
        {
            Capacity = capacity;
            AllowMultiSpawn = allowMultiSpawn;
            AutoReleaseInterval = autoReleaseInterval;
            ExpireTime = expireTime;
            Priority = priority;
        }
        
        /// <summary>
        /// 创建对象。
        /// </summary>
        /// <param name="obj">对象。</param>
        /// <param name="spawned">对象是否已被获取。</param>
        public void Register(T obj, bool spawned)
        {
            if (obj == null)
            {
                throw new Exception("Object is invalid.");
            }

            Object<T> internalObject = Object<T>.Create(obj, spawned);
            _objects.Add(obj.Name, internalObject);
            _objectMap.Add(obj.Target, internalObject);

            if (Count > Capacity)
            {
                Release();
            }
        }

        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            _autoReleaseTime += realElapseSeconds;
            if (_autoReleaseTime < AutoReleaseInterval)
            {
                return;
            }

            Release();
        }
        /// <summary>
        /// 检查对象。
        /// </summary>
        /// <param name="name">对象名称。</param>
        /// <returns>要检查的对象是否存在。</returns>
        public bool CanSpawn(string name)
        {
            if (name == null)
            {
                throw new Exception("Name is invalid.");
            }
            
            if (_objects.TryGetValue(name, out var objectRange))
            {
                if (AllowMultiSpawn || !objectRange.IsInUse)
                {
                    return true;
                }
            }

            return false;
        }
        
        public T Spawn()
        {
            return Spawn(string.Empty);
        }

        public T Spawn(string name)
        {
            if (name == null)
            {
                throw new Exception("Name is invalid.");
            }

            if (_objects.TryGetValue(name, out var objectRange))
            {
                if (AllowMultiSpawn || !objectRange.IsInUse)
                {
                    return objectRange.Spawn();
                }
            }
            return default;
        }
        
        public void UnSpawn(T obj)
        {
            if (obj == null)
            {
                throw new Exception("Object is invalid.");
            }
            UnSpawn(obj.Target);
        }
        
        public void UnSpawn(object target)
        {
            if (target == null)
            {
                throw new Exception("target is invalid.");
            }
            Object<T> internalObject = GetObject(target);
            if (internalObject != null)
            {
                internalObject.UnSpawn();
                if (Count > Capacity && internalObject.SpawnCount <= 0)
                {
                    Release();
                }
            }
        }

        public void SetLocked(T obj, bool locked)
        {
            if (obj == null)
            {
                throw new Exception("Object is invalid.");
            }
            if (obj.Target == null)
            {
                throw new Exception("Target is invalid.");
            }

            Object<T> internalObject = GetObject(obj.Target);
            if (internalObject != null)
            {
                obj.Locked = locked;
            }
        }

        public void SetPriority(T obj, int priority)
        {
            if (obj == null)
            {
                throw new Exception("Object is invalid.");
            }
            if (obj.Target == null)
            {
                throw new Exception("Target is invalid.");
            }

            Object<T> internalObject = GetObject(obj.Target);
            if (internalObject != null)
            {
                obj.Priority = priority;
            }
        }

        public bool ReleaseObject(T obj)
        {
            if (obj == null)
            {
                throw new Exception("Object is invalid.");
            }

            return ReleaseObject(obj.Target);
        }

        public override void Release()
        {
            Release(Count - Capacity, DefaultReleaseObjectFilterCallback);
        }

        public override void ReleaseAllUnused()
        {
            _autoReleaseTime = 0f;
            GetCanReleaseObjects(_cachedCanReleaseObjects);
            foreach (T toReleaseObject in _cachedCanReleaseObjects)
            {
                ReleaseObject(toReleaseObject);
            }
        }
        
        public void Release(int toReleaseCount,ReleaseObjectFilterCallback<T> releaseObjectFilterCallback)
        {
            if (toReleaseCount < 0)
            {
                toReleaseCount = 0;
            }
            DateTime expireTime = DateTime.MinValue;
            if (ExpireTime < float.MaxValue)
            {
                expireTime = DateTime.UtcNow.AddSeconds(-ExpireTime);
            }
            _autoReleaseTime = 0f;
            GetCanReleaseObjects(_cachedCanReleaseObjects);
            List<T> toReleaseObjects = releaseObjectFilterCallback(_cachedCanReleaseObjects, toReleaseCount, expireTime);
            if (toReleaseObjects == null || toReleaseObjects.Count <= 0)
            {
                return;
            }
            foreach (T toReleaseObject in toReleaseObjects)
            {
                ReleaseObject(toReleaseObject);
            }
        }

        #region 私有方法获取相关信息的接口

        private void GetCanReleaseObjects(List<T> results)
        {
            _cachedCanReleaseObjects.Clear();
            foreach (KeyValuePair<object, Object<T>> objectInMap in _objectMap)
            {
                Object<T> internalObject = objectInMap.Value;
                if (internalObject.IsInUse || internalObject.Locked)
                {
                    continue;
                }

                results.Add(internalObject.Peek());
            }
        }
        private Object<T> GetObject(object target)
        {
            if (target == null)
            {
                throw new Exception("Target is invalid.");
            }
            return _objectMap.TryGetValue(target, out Object<T> internalObject) ? internalObject : null;
        }

        
        private bool ReleaseObject(object target)
        {
            if (target == null)
            {
                throw new Exception("Target is invalid.");
            }
            Object<T> internalObject = GetObject(target);
            if (internalObject == null)
            {
                return false;
            }
            
            if (internalObject.IsInUse || internalObject.Locked || !internalObject.CustomCanReleaseFlag)
            {
                return false;
            }

            _objects.Remove(internalObject.Name);
            _objectMap.Remove(target);
            internalObject.DeInitialize(false);
            ReferencePool.Release(internalObject);
            return true;
        }
        
        /// <summary>
        /// 默认筛选条件筛选函数。
        /// </summary>
        /// <param name="candidateObjects"></param>
        /// <param name="toReleaseCount"></param>
        /// <param name="expireTime"></param>
        /// <returns></returns>
        private List<T> DefaultReleaseObjectFilterCallback(List<T> candidateObjects, int toReleaseCount, DateTime expireTime)
        {
            _cachedToReleaseObjects.Clear();

            if (expireTime > DateTime.MinValue)
            {
                for (int i = candidateObjects.Count - 1; i >= 0; i--)
                {
                    if (candidateObjects[i].LastUseTime <= expireTime)
                    {
                        _cachedToReleaseObjects.Add(candidateObjects[i]);
                        candidateObjects.RemoveAt(i);
                    }
                }

                toReleaseCount -= _cachedToReleaseObjects.Count;
            }

            for (int i = 0; toReleaseCount > 0 && i < candidateObjects.Count; i++)
            {
                for (int j = i + 1; j < candidateObjects.Count; j++)
                {
                    if (candidateObjects[i].Priority > candidateObjects[j].Priority
                        || candidateObjects[i].Priority == candidateObjects[j].Priority && candidateObjects[i].LastUseTime > candidateObjects[j].LastUseTime)
                    {
                        (candidateObjects[i], candidateObjects[j]) = (candidateObjects[j], candidateObjects[i]);
                    }
                }

                _cachedToReleaseObjects.Add(candidateObjects[i]);
                toReleaseCount--;
            }

            return _cachedToReleaseObjects;
        }
        
        #endregion
        
        public void Clear()
        {
            Capacity = default;
            AllowMultiSpawn = default;
            AutoReleaseInterval = default;
            ExpireTime = default;
            Priority = default;
            _autoReleaseTime = default;
            _cachedCanReleaseObjects.Clear();
            _cachedToReleaseObjects.Clear();
            _objectMap.Clear();
        }

        public override void DeInitialize()
        {
            foreach (KeyValuePair<object, Object<T>> objectInMap in _objectMap)
            {
                objectInMap.Value.DeInitialize(true);
                ReferencePool.Release(objectInMap.Value);
            }
            _objects.Clear();
            _objectMap.Clear();
            _cachedCanReleaseObjects.Clear();
            _cachedToReleaseObjects.Clear();
        }
        private sealed class Object<TObject> : IReference where TObject : ObjectBase
        {
            public string Name => Target.Name;
            public bool Locked => Target.Locked;
            public int Priority => Target.Priority;
            public DateTime LastUseTime => Target.LastUseTime;
            public bool CustomCanReleaseFlag => Target.CustomCanReleaseFlag;
            public bool IsInUse => SpawnCount > 0;
            public TObject Target { get; private set; }
            /// <summary>
            /// 获取对象的获取计数。
            /// </summary>
            public int SpawnCount { get; private set; }

            /// <summary>
            /// 创建内部对象。
            /// </summary>
            /// <param name="obj">对象。</param>
            /// <param name="spawned">对象是否已被获取。</param>
            /// <returns>创建的内部对象。</returns>
            public static Object<TObject> Create(TObject obj, bool spawned)
            {
                if (obj == null)
                {
                    throw new Exception("Object is invalid.");
                }
                Object<TObject> o = ReferencePool.Acquire<Object<TObject>>();
                o.Target = obj;
                o.SpawnCount = spawned ? 1 : 0;
                if (spawned)
                {
                    obj.OnSpawn();
                }

                return o;
            }
            /// <summary>
            /// 查看对象。
            /// </summary>
            /// <returns>对象。</returns>
            public TObject Peek()
            {
                return Target;
            }
            
            /// <summary>
            /// 获取对象。
            /// </summary>
            /// <returns>对象。</returns>
            public TObject Spawn()
            {
                SpawnCount++;
                Target.LastUseTime = DateTime.UtcNow;
                Target.OnSpawn();
                return Target;
            }

            /// <summary>
            /// 回收对象。
            /// </summary>
            public void UnSpawn()
            {
                Target.OnUnSpawn();
                Target.LastUseTime = DateTime.UtcNow;
                SpawnCount--;
                if (SpawnCount < 0)
                {
                    throw new Exception($"Object '{Name}' spawn count is less than 0.");
                }
            }
            
            /// <summary>
            /// 释放对象。
            /// </summary>
            /// <param name="isShutdown">是否是关闭对象池时触发。</param>
            public void DeInitialize(bool isShutdown)
            {
                Target.DeInitialize(isShutdown);
                ReferencePool.Release(Target);
            }
            
            public void Clear()
            {
                Target = default;
                SpawnCount = default;
            }
        }
    }
}