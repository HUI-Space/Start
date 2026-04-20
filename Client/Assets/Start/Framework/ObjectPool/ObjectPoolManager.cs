using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Start
{
    /// <summary>
    /// 对象池管理器（全局管理所有对象池）
    /// </summary>
    public class ObjectPoolManager : ManagerBase<ObjectPoolManager>, IUpdateManager
    {
        public override int Priority => 2;
        
        /// <summary>
        /// 类型-对象池映射字典
        /// </summary>
        private readonly ConcurrentDictionary<Type, IObjectPool> _pools = new ConcurrentDictionary<Type, IObjectPool>();

        /// <summary>
        /// 创建指定类型的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="capacity">容量（0表示无上限）</param>
        /// <returns>对象池实例</returns>
        /// <exception cref="InvalidOperationException">对象池已存在时抛出</exception>
        public ObjectPool<T> CreateObjectPool<T>(int capacity = 0) where T : class, IObject, new()
        {
            Type type = typeof(T);
            if (_pools.ContainsKey(type))
            {
                throw new InvalidOperationException($"类型[{type.Name}]的对象池已存在，无法重复创建");
            }

            ObjectPool<T> pool = ObjectPool<T>.Create(capacity);
            if (_pools.TryAdd(type, pool))
            {
                return pool;
            }
            
            // 添加失败时回收池实例，避免内存泄漏
            RecyclablePool.Recycle(pool);
            throw new InvalidOperationException($"类型[{type.Name}]的对象池创建失败");
        }
        
        /// <summary>
        /// 获取指定类型的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>对象池实例</returns>
        /// <exception cref="InvalidOperationException">对象池不存在时抛出</exception>
        public ObjectPool<T> GetObjectPool<T>() where T : class, IObject, new()
        {
            Type type = typeof(T);
            if (_pools.TryGetValue(type, out var pool))
            {
                return (ObjectPool<T>)pool;
            }
            
            throw new InvalidOperationException($"类型[{type.Name}]的对象池不存在，请先创建");
        } 
        
        /// <summary>
        /// 销毁指定类型的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>是否销毁成功</returns>
        public bool DestroyObjectPool<T>() where T : class, IObject, new()
        {
            Type type = typeof(T);
            if (_pools.TryRemove(type, out var pool))
            {
                RecyclablePool.Recycle(pool);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 更新所有对象池（驱动自动释放）
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝秒数</param>
        /// <param name="realElapseSeconds">真实流逝秒数</param>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            // 遍历副本，避免遍历过程中字典修改导致的异常
            foreach (var item in _pools.ToArray())
            {
                item.Value.Update(elapseSeconds, realElapseSeconds);
            }
        }

        /// <summary>
        /// 反初始化（销毁所有对象池）
        /// </summary>
        /// <returns>任务</returns>
        public override Task DeInitialize()
        {
            foreach (var item in _pools.Values)
            {
                item.Recycle();
                RecyclablePool.Recycle(item);
            }
            _pools.Clear();
            return base.DeInitialize();
        }
    }
}