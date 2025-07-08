using System;
using System.Collections.Generic;

namespace Start
{
    /// <summary>
    /// 回收对象的容器
    /// </summary>
    public class RecycleContainer
    {
        /// <summary>
        /// 当前未使用的数量
        /// </summary>
        public int UnusedCount => _recycles.Count;

        /// <summary>
        /// 正在使用数量
        /// </summary>
        public int UsingCount { get; private set; }

        /// <summary>
        /// 获取数量
        /// </summary>
        public int GetCount { get; private set; }

        /// <summary>
        /// 归还数量
        /// </summary>
        public int RecycleCount { get; private set; }

        /// <summary>
        /// 增加数量
        /// </summary>
        public int AddCount { get; private set; }

        /// <summary>
        /// 移除数量
        /// </summary>
        public int RemoveCount { get; private set; }

        /// <summary>
        /// 存储对象的队列
        /// </summary>
        private readonly Queue<IRecycle> _recycles;

        /// <summary>
        /// 当前回收池的对象类型
        /// </summary>
        private readonly Type _recycleType;

        public RecycleContainer(Type recycleType)
        {
            _recycles = new Queue<IRecycle>();
            _recycleType = recycleType;
            GetCount = 0;
            AddCount = 0;
            RemoveCount = 0;
            RecycleCount = 0;
        }

        public T Get<T>() where T : class, IRecycle, new()
        {
            if (typeof(T) != _recycleType)
            {
                throw new Exception("Type is invalid.");
            }

            UsingCount++;
            GetCount++;
            lock (_recycles)
            {
                if (_recycles.Count > 0)
                {
                    return (T)_recycles.Dequeue();
                }
            }

            AddCount++;
            return new T();
        }

        public IRecycle Get()
        {
            UsingCount++;
            GetCount++;
            lock (_recycles)
            {
                if (_recycles.Count > 0)
                {
                    return _recycles.Dequeue();
                }
            }

            AddCount++;
            return (IRecycle)Activator.CreateInstance(_recycleType);
        }

        public void Release(IRecycle reference)
        {
            _recycles.Clear();
            lock (_recycles)
            {
                if (_recycles.Contains(reference))
                {
                    throw new Exception("The reference has been released.");
                }

                _recycles.Enqueue(reference);
            }

            RecycleCount++;
            UsingCount--;
        }

        public void Add<T>(int count) where T : class, IRecycle, new()
        {
            if (typeof(T) != _recycleType)
            {
                throw new Exception("Type is invalid.");
            }

            lock (_recycles)
            {
                AddCount += count;
                while (count-- > 0)
                {
                    _recycles.Enqueue(new T());
                }
            }
        }

        public void Add(int count)
        {
            lock (_recycles)
            {
                AddCount += count;
                while (count-- > 0)
                {
                    _recycles.Enqueue((IRecycle)Activator.CreateInstance(_recycleType));
                }
            }
        }

        public void Remove(int count)
        {
            lock (_recycles)
            {
                if (_recycles.Count < count)
                {
                    count = _recycles.Count;
                }

                RecycleCount += count;
                while (count-- > 0)
                {
                    _recycles.Dequeue();
                }
            }
        }

        public void RemoveAll()
        {
            lock (_recycles)
            {
                RemoveCount += _recycles.Count;
                _recycles.Clear();
            }
        }
    }
}