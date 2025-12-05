using System;
using System.Collections.Generic;

namespace Start
{
    public class ObjectPool<TObject, TTarget> : IObjectPool<TObject, TTarget>
        where TObject : ObjectBase<TTarget>, new() 
        where TTarget : class
    {
        public Type ObjectType => typeof(TObject);
        public int Capacity { get; private set; }
        
        public int CanReleaseCount
        {
            get
            {
                GetCanReleaseObjects(_cachedCanReleaseObjects);
                return _cachedCanReleaseObjects.Count;
            }
        }
        public bool AllowMultiSpawn { get; private set; }
        public float AutoReleaseInterval { get; private set; }
        public float ExpireTime { get; private set; }
        public int Priority { get; set; }
        public int Count => _objectMap.Count;

        private float _autoReleaseTime;
        private readonly List<TObject> _cachedCanReleaseObjects = new();
        private readonly List<TObject> _cachedToReleaseObjects = new();
        private readonly Dictionary<string, Object> _objects = new();
        private readonly Dictionary<TTarget, Object> _objectMap = new(); // 使用TTarget作为键

        public void Initialize(int capacity, bool allowMultiSpawn, float autoReleaseInterval, float expireTime, int priority)
        {
            Capacity = capacity;
            AllowMultiSpawn = allowMultiSpawn;
            AutoReleaseInterval = autoReleaseInterval;
            ExpireTime = expireTime;
            Priority = priority;
        }
        
        public void Register(TObject obj, bool spawned)
        {
            if (obj == null)
            {
                throw new Exception("Object 为空.");
            }

            Object internalObject = Object.Create(obj, spawned);
            _objects.Add(obj.Name, internalObject);
            _objectMap.Add(obj.Target, internalObject);

            if (Count > Capacity)
            {
                Release();
            }
        }

        public bool CanSpawn(string name)
        {
            if (name == null)
            {
                throw new Exception("Name 为空.");
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

        public TObject Spawn()
        {
            return Spawn(string.Empty);
        }

        public TObject Spawn(string name)
        {
            if (name == null)
            {
                throw new Exception("Name 为空.");
            }

            if (_objects.TryGetValue(name, out Object objectRange))
            {
                if (AllowMultiSpawn || !objectRange.IsInUse)
                {
                    return objectRange.Spawn();
                }
            }
            return default;
        }

        public void UnSpawn(TObject obj)
        {
            if (obj == null)
            {
                throw new Exception("Object 为空.");
            }
            UnSpawn(obj.Target);
        }

        public void UnSpawn(TTarget target)
        {
            if (target == null)
            {
                throw new Exception("target 为空.");
            }
            Object internalObject = GetObject(target);
            if (internalObject != null)
            {
                internalObject.UnSpawn();
                if (Count > Capacity && internalObject.SpawnCount <= 0)
                {
                    Release();
                }
            }
        }

        public void SetLocked(TObject obj, bool locked)
        {
            if (obj == null)
            {
                throw new Exception("Object 为空.");
            }
            if (obj.Target == null)
            {
                throw new Exception("Target 为空.");
            }

            Object internalObject = GetObject(obj.Target);
            if (internalObject != null)
            {
                obj.Locked = locked;
            }
        }

        public void SetPriority(TObject obj, int priority)
        {
            if (obj == null)
            {
                throw new Exception("Object 为空.");
            }
            if (obj.Target == null)
            {
                throw new Exception("Target 为空.");
            }

            Object internalObject = GetObject(obj.Target);
            if (internalObject != null)
            {
                obj.Priority = priority;
            }
        }

        public bool ReleaseObject(TObject obj)
        {
            if (obj == null)
            {
                throw new Exception("Object 为空.");
            }

            return ReleaseObject(obj.Target);
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            _autoReleaseTime += realElapseSeconds;
            if (_autoReleaseTime < AutoReleaseInterval)
            {
                return;
            }

            Release();
        }

        public void Release()
        {
            Release(Count - Capacity, DefaultReleaseObjectFilterCallback);
        }

        public void Release(int toReleaseCount, ReleaseObjectFilterCallback<TObject,TTarget> releaseObjectFilterCallback)
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
            List<TObject> toReleaseObjects = releaseObjectFilterCallback(_cachedCanReleaseObjects, toReleaseCount, expireTime);
            if (toReleaseObjects == null || toReleaseObjects.Count <= 0)
            {
                return;
            }
            foreach (TObject toReleaseObject in toReleaseObjects)
            {
                ReleaseObject(toReleaseObject);
            }
        }

        

        public void ReleaseAllUnused()
        {
            _autoReleaseTime = 0f;
            GetCanReleaseObjects(_cachedCanReleaseObjects);
            foreach (TObject toReleaseObject in _cachedCanReleaseObjects)
            {
                ReleaseObject(toReleaseObject);
            }
        }

        public void DeInitialize()
        {
            foreach (KeyValuePair<TTarget, Object> objectInMap in _objectMap)
            {
                objectInMap.Value.DeInitialize(true);
                RecyclableObjectPool.Recycle(objectInMap.Value);
            }
            _objects.Clear();
            _objectMap.Clear();
            _cachedCanReleaseObjects.Clear();
            _cachedToReleaseObjects.Clear();
        }

        public void Reset()
        {
            Capacity = 0;
            AllowMultiSpawn = false;
            AutoReleaseInterval = 0;
            ExpireTime = 0;
            Priority = 0;
            _autoReleaseTime = 0;
            _cachedCanReleaseObjects.Clear();
            _cachedToReleaseObjects.Clear();
            _objectMap.Clear();
        }
        
        #region 私有方法获取相关信息的接口

        private void GetCanReleaseObjects(List<TObject> results)
        {
            _cachedCanReleaseObjects.Clear();
            foreach (KeyValuePair<TTarget, Object> objectInMap in _objectMap)
            {
                Object internalObject = objectInMap.Value;
                if (internalObject.IsInUse || internalObject.Locked)
                {
                    continue;
                }

                results.Add(internalObject.Peek());
            }
        }
        private Object GetObject(TTarget target)
        {
            if (target == null)
            {
                throw new Exception("Target 为空.");
            }
            return _objectMap.GetValueOrDefault(target);
        }

        
        private bool ReleaseObject(TTarget target)
        {
            if (target == null)
            {
                throw new Exception("Target 为空.");
            }
            Object internalObject = GetObject(target);
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
            RecyclableObjectPool.Recycle(internalObject);
            return true;
        }
        
        /// <summary>
        /// 默认筛选条件筛选函数。
        /// </summary>
        /// <param name="candidateObjects"></param>
        /// <param name="toReleaseCount"></param>
        /// <param name="expireTime"></param>
        /// <returns></returns>
        private List<TObject> DefaultReleaseObjectFilterCallback(List<TObject> candidateObjects, int toReleaseCount, DateTime expireTime)
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
        
        
        private sealed class Object : IReusable
        {
            public TObject Target { get; private set; }
            public int SpawnCount { get; private set; }
            public string Name => Target.Name;
            public bool Locked => Target.Locked;
            public int Priority => Target.Priority;
            public DateTime LastUseTime => Target.LastUseTime;
            public bool CustomCanReleaseFlag => Target.CustomCanReleaseFlag;
            public bool IsInUse => SpawnCount > 0;

            public static Object Create(TObject obj, bool spawned)
            {
                var pooled = RecyclableObjectPool.Acquire<Object>();
                pooled.Target = obj;
                pooled.SpawnCount = spawned ? 1 : 0;
                if (spawned) obj.OnSpawn();
                return pooled;
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
                    throw new InvalidOperationException($"Object '{Name}' spawn count is less than 0.");
            }

            public void DeInitialize(bool isShutdown)
            {
                Target.DeInitialize(isShutdown);
                RecyclableObjectPool.Recycle(Target);
            }

            public void Reset() => (Target, SpawnCount) = (default, default);
        }
    }
}