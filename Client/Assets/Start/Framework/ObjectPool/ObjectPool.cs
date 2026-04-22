using System;
using System.Collections.Generic;

namespace Start
{
    /// <summary>
    /// 泛型对象池实现类
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    public class ObjectPool<T> : IObjectPool<T> where T : class, IObject, new()
    {
        public Type ObjectType => typeof(T);
        
        public int Capacity { get; private set;}
        
        /// <summary>
        /// 对象过期时间（秒）
        /// </summary>
        public float ExpireTime { get; private set; }
        
        /// <summary>
        /// 自动释放间隔（秒）
        /// </summary>
        public float AutoReleaseInterval { get; private set; }
        
        public int Count => _inUseObjects.Count + _freeStack.Count;
        
        /// <summary>
        /// 可释放对象数（空闲+未锁定+自定义可释放）
        /// </summary>
        public int CanReleaseCount
        { 
            get
            {
                // 遍历空闲栈，统计可释放对象
                int count = 0;
                foreach (var t in _freeStack)
                {
                    if (_objectStatus.TryGetValue(t, out var status) 
                        && !status.Locked 
                        && status.CustomCanReleaseFlag)
                    {
                        count++;
                    }
                }
                return count;
            }
        }
        
        private float _autoReleaseTime;
        
        /// <summary>
        /// 缓存可释放对象列表（避免频繁创建集合）
        /// </summary>
        private readonly List<ObjectStatus<T>> _cachedCanReleaseObjects = new();
        
        /// <summary>
        /// 缓存待释放对象列表
        /// </summary>
        private readonly List<ObjectStatus<T>> _cachedToReleaseObjects = new();
        
        /// <summary>
        /// 使用中对象集合（快速判断对象归属）
        /// </summary>
        private readonly HashSet<T> _inUseObjects = new HashSet<T>();
        
        /// <summary>
        /// 空闲对象栈（栈结构：后进先出，提高缓存命中率）
        /// </summary>
        private readonly Stack<T> _freeStack = new Stack<T>();
        
        /// <summary>
        /// 对象状态字典
        /// </summary>
        private readonly Dictionary<T, ObjectStatus<T>> _objectStatus = new Dictionary<T, ObjectStatus<T>>();
        
        /// <summary>
        /// 创建对象池实例
        /// </summary>
        /// <param name="capacity">容量（0表示无上限）</param>
        /// <param name="autoReleaseInterval">自动释放间隔（秒）</param>
        /// <param name="expireTime">对象过期时间（秒）</param>
        /// <returns>对象池实例</returns>
        public static ObjectPool<T> Create(int capacity = Int32.MaxValue, float autoReleaseInterval = float.MaxValue, float expireTime = float.MaxValue)
        {
            ObjectPool<T> pool = RecyclablePool.Acquire<ObjectPool<T>>();
            pool.Capacity = capacity;
            pool.AutoReleaseInterval = autoReleaseInterval;
            pool.ExpireTime = expireTime;
            return pool;
        }
        
        /// <summary>
        /// 从对象池获取对象
        /// </summary>
        /// <returns>对象实例</returns>
        public T Spawn()
        {
            T t = null;
            // 1. 优先从空闲栈获取
            if (_freeStack.TryPop(out t))
            {
                _inUseObjects.Add(t);
                // 更新上次使用时间
                if (_objectStatus.TryGetValue(t, out var status))
                {
                    status.LastUseTime = DateTime.UtcNow;
                }
            }
            // 2. 无空闲对象时创建新实例（未超容量）
            else if (Capacity == 0 || Count < Capacity)
            {
                t = new T();
                ObjectStatus<T> status = ObjectStatus<T>.Create(t);
                _inUseObjects.Add(t);
                _objectStatus.TryAdd(t, status);
            }

            if (t == null)
            {
                throw new InvalidOperationException($"对象池已达容量上限：{Capacity}，无法创建新对象");
            }
            
            t.OnSpawn();
            return t;
        }

        /// <summary>
        /// 获取对象状态
        /// </summary>
        /// <param name="t">对象实例</param>
        /// <returns>对象状态</returns>
        public ObjectStatus<T> GetObjectStatus(T t)
        {
            if (t == null)
            {
                throw new ArgumentNullException(nameof(t), "对象不能为空");
            }
            if (!_objectStatus.TryGetValue(t, out ObjectStatus<T> status))
            {
                throw new InvalidOperationException($"对象[{t.GetType().Name}]未从当前池获取，无法获取对象状态");
            }
            return status;
        }

        /// <summary>
        /// 回收对象到对象池
        /// </summary>
        /// <param name="t">对象实例</param>
        public void UnSpawn(T t)
        {
            if (t == null)
            {
                throw new ArgumentNullException(nameof(t), "回收的对象不能为空");
            }
            
            // 验证对象归属
            if (!_inUseObjects.Remove(t))
            {
                throw new InvalidOperationException($"对象[{t.GetType().Name}]未从当前池获取，无法回收");
            }

            // 1. 超容量时直接释放，不回池
            if (Capacity > 0 && Count >= Capacity)
            {
                ReleaseObjectInternal(t);
            }
            else
            {
                // 2. 重置对象状态后回空闲栈
                t.OnUnSpawn();
                // 更新上次使用时间
                if (_objectStatus.TryGetValue(t, out var status))
                {
                    status.LastUseTime = DateTime.UtcNow;
                }
                _freeStack.Push(t);
            }
        }

        /// <summary>
        /// 设置对象锁定状态
        /// </summary>
        /// <param name="t">对象实例</param>
        /// <param name="locked">是否锁定</param>
        public void SetLocked(T t, bool locked)
        {
            if (t == null)
            {
                throw new ArgumentNullException(nameof(t), "对象不能为空");
            }
            
            if (!_objectStatus.TryGetValue(t, out var status))
            {
                throw new InvalidOperationException($"对象[{t.GetType().Name}]未从当前池获取，无法设置锁定状态");
            }
            
            status.Locked = locked;
        }

        /// <summary>
        /// 设置对象优先级
        /// </summary>
        /// <param name="t">对象实例</param>
        /// <param name="priority">优先级</param>
        public void SetPriority(T t, int priority)
        {
            if (t == null)
            {
                throw new ArgumentNullException(nameof(t), "对象不能为空");
            }
            
            if (!_objectStatus.TryGetValue(t, out var status))
            {
                throw new InvalidOperationException($"对象[{t.GetType().Name}]未从当前池获取，无法设置优先级");
            }
            
            status.Priority = priority;
        }

        /// <summary>
        /// 释放单个对象（外部调用）
        /// </summary>
        /// <param name="t">对象实例</param>
        /// <returns>是否释放成功</returns>
        public bool ReleaseObject(T t)
        {
            if (t == null)
            {
                throw new ArgumentNullException(nameof(t), "对象不能为空");
            }
            
            return ReleaseObjectInternal(t);
        }
        
        /// <summary>
        /// 内部释放对象逻辑（无锁，需外部加锁）
        /// </summary>
        /// <param name="t">对象实例</param>
        /// <returns>是否释放成功</returns>
        private bool ReleaseObjectInternal(T t)
        {
            // 检查对象是否可释放
            if (!_objectStatus.TryGetValue(t, out var status) 
                || status.Locked 
                || !status.CustomCanReleaseFlag)
            {
                return false;
            }

            // 从所有容器移除
            _inUseObjects.Remove(t);
            _objectStatus.Remove(t, out _);
            
            // 从Stack移除
            _freeStack.TryRemove(t);

            // 回收状态对象+释放对象
            RecyclablePool.Recycle(status);
            t.OnRelease();
            return true;
        }
        
        /// <summary>
        /// 更新对象池（驱动自动释放）
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝秒数</param>
        /// <param name="realElapseSeconds">真实流逝秒数</param>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            _autoReleaseTime += realElapseSeconds;
            if (_autoReleaseTime < AutoReleaseInterval)
            {
                return;
            }

            // 重置计时+执行释放
            _autoReleaseTime = 0;
            Release();
        }
        
        /// <summary>
        /// 自动释放超出容量的对象
        /// </summary>
        public void Release()
        {
            // 计算需要释放的数量：当前总数 - 容量（最小为0）
            int toReleaseCount = Math.Max(0, Count - Capacity);
            if (toReleaseCount <= 0) return;
            
            Release(toReleaseCount, DefaultReleaseObjectFilterCallback);
        }
        
        /// <summary>
        /// 自定义释放指定数量的对象
        /// </summary>
        /// <param name="toReleaseCount">期望释放数量</param>
        /// <param name="releaseObjectFilterCallback">筛选函数</param>
        public void Release(int toReleaseCount, ReleaseObjectFilterCallback<T> releaseObjectFilterCallback)
        {
            if (toReleaseCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(toReleaseCount), "释放数量不能为负数");
            }
            if (releaseObjectFilterCallback == null)
            {
                throw new ArgumentNullException(nameof(releaseObjectFilterCallback), "筛选函数不能为空");
            }
            if (toReleaseCount == 0 || CanReleaseCount == 0)
            {
                return; // 无需要释放的对象，直接返回
            }

            // 1. 收集所有可释放的空闲对象
            _cachedCanReleaseObjects.Clear();
            foreach (T t in _freeStack)
            {
                if (_objectStatus.TryGetValue(t, out var status) 
                    && !status.Locked 
                    && status.CustomCanReleaseFlag)
                {
                    _cachedCanReleaseObjects.Add(status);
                }
            }

            if (_cachedCanReleaseObjects.Count == 0) return;

            // 2. 应用筛选函数获取待释放列表
            DateTime currentTime = DateTime.UtcNow;
            List<ObjectStatus<T>> toReleaseObjects = releaseObjectFilterCallback(_cachedCanReleaseObjects, toReleaseCount, currentTime);

            // 3. 执行释放
            int releasedCount = 0;
            foreach (var status in toReleaseObjects)
            {
                if (releasedCount >= toReleaseCount) break;
                if (ReleaseObjectInternal(status.Target))
                {
                    releasedCount++;
                }
            }
        }

        /// <summary>
        /// 回收对象池（清空所有对象）
        /// </summary>
        public void Recycle()
        {
            // 释放所有空闲对象
            while (_freeStack.TryPop(out var t))
            {
                if (_objectStatus.Remove(t, out var status))
                {
                    RecyclablePool.Recycle(status);
                }
                t.OnRelease();
            }
                
            // 清理使用中对象（触发释放回调）
            foreach (var t in _inUseObjects)
            {
                if (_objectStatus.Remove(t, out var status))
                {
                    RecyclablePool.Recycle(status);
                }
                t.OnRelease();
            }
                
            // 重置所有状态
            _inUseObjects.Clear();
            _objectStatus.Clear();
            Capacity = 0;
            _autoReleaseTime = 0;
            _cachedCanReleaseObjects.Clear();
            _cachedToReleaseObjects.Clear();
        }
        
        /// <summary>
        /// 默认释放筛选逻辑：先释放过期对象，再按优先级+最后使用时间排序释放
        /// </summary>
        /// <param name="candidateObjects">候选对象列表</param>
        /// <param name="toReleaseCount">期望释放数量</param>
        /// <param name="expireTime">当前UTC时间</param>
        /// <returns>待释放对象列表</returns>
        private List<ObjectStatus<T>> DefaultReleaseObjectFilterCallback(List<ObjectStatus<T>> candidateObjects, int toReleaseCount, DateTime expireTime)
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
    }
}