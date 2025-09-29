using System;
using System.Collections.Generic;

namespace Start
{
    /// <summary>
    /// 对象回收池
    /// </summary>
    public static class RecyclePool
    {
        private static readonly Dictionary<Type,RecycleContainer> _recycleContainers = new Dictionary<Type, RecycleContainer>();

        
        public static T Get<T>() where T : class, IRecycle, new()
        {
            return GetRecycleContainer(typeof(T)).Get<T>();
        }
        
        public static IRecycle Get(Type type)
        {
            return GetRecycleContainer(type).Get();
        }

        public static void Add<T>(int count) where T : class, IRecycle, new()
        {
            
        }

        public static void Remove<T>(int count) where T : class, IRecycle, new()
        {
            
        }

        public static void RemoveAll<T>() where T : class, IRecycle, new()
        {
            
        }
        
        /// <summary>
        /// 执行回收操作的方法。
        /// </summary>
        /// <param name="recycle">一个实现了IRecycle接口的实例，用于执行回收操作。</param>
        public static void Recycle(IRecycle recycle)
        {
            
        }

        public static void ClearAll()
        {
            
        }
        
        private static RecycleContainer GetRecycleContainer(Type type)
        {
            RecycleContainer recycleContainer;
            lock (_recycleContainers)
            {
                if (!_recycleContainers.TryGetValue(type, out recycleContainer))
                {
                    recycleContainer = new RecycleContainer(type);
                    _recycleContainers.Add(type, recycleContainer);
                }
            }
            return recycleContainer;
        }
    }
}