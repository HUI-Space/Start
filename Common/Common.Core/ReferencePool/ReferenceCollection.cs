using System;
using System.Collections.Generic;

namespace Start
{
    public class ReferenceCollection
    {
        /// <summary>
        /// 当前引用池未使用的数量
        /// </summary>
        public int UnusedReferenceCount => _references.Count;

        /// <summary>
        /// 正在使用引用数量
        /// </summary>
        public int UsingReferenceCount { get; private set; }

        /// <summary>
        /// 获取引用数量
        /// </summary>
        public int AcquireReferenceCount { get; private set; }

        /// <summary>
        /// 归还引用数量
        /// </summary>
        public int ReleaseReferenceCount { get; private set; }

        /// <summary>
        /// 增加引用数量
        /// </summary>
        public int AddReferenceCount { get; private set; }

        /// <summary>
        /// 移除引用数量
        /// </summary>
        public int RemoveReferenceCount { get; private set; }

        /// <summary>
        /// 存储对象的队列
        /// </summary>
        private readonly Queue<IReference> _references;

        /// <summary>
        /// 当前引用池的对象类型
        /// </summary>
        private readonly Type _referenceType;

        public ReferenceCollection(Type referenceType)
        {
            _references = new Queue<IReference>();
            _referenceType = referenceType;
            AcquireReferenceCount = 0;
            ReleaseReferenceCount = 0;
            AddReferenceCount = 0;
            RemoveReferenceCount = 0;
        }

        public T Aquire<T>() where T : class, IReference, new()
        {
            if (typeof(T) != _referenceType)
            {
                throw new Exception("Type is invalid.");
            }

            UsingReferenceCount++;
            AcquireReferenceCount++;
            lock (_references)
            {
                if (_references.Count > 0)
                {
                    return (T)_references.Dequeue();
                }
            }

            AddReferenceCount++;
            return new T();
        }

        public IReference Aquire()
        {
            UsingReferenceCount++;
            AcquireReferenceCount++;
            lock (_references)
            {
                if (_references.Count > 0)
                {
                    return _references.Dequeue();
                }
            }

            AddReferenceCount++;
            return (IReference)Activator.CreateInstance(_referenceType);
        }

        public void Release(IReference reference)
        {
            reference.Clear();
            lock (_references)
            {
                if (_references.Contains(reference))
                {
                    throw new Exception("The reference has been released.");
                }

                _references.Enqueue(reference);
            }

            ReleaseReferenceCount++;
            UsingReferenceCount--;
        }

        public void Add<T>(int count) where T : class, IReference, new()
        {
            if (typeof(T) != _referenceType)
            {
                throw new Exception("Type is invalid.");
            }

            lock (_references)
            {
                AddReferenceCount += count;
                while (count-- > 0)
                {
                    _references.Enqueue(new T());
                }
            }
        }

        public void Add(int count)
        {
            lock (_references)
            {
                AddReferenceCount += count;
                while (count-- > 0)
                {
                    _references.Enqueue((IReference)Activator.CreateInstance(_referenceType));
                }
            }
        }

        public void Remove(int count)
        {
            lock (_references)
            {
                if (_references.Count < count)
                {
                    count = _references.Count;
                }

                ReleaseReferenceCount += count;
                while (count-- > 0)
                {
                    _references.Dequeue();
                }
            }
        }

        public void RemoveAll()
        {
            lock (_references)
            {
                RemoveReferenceCount += _references.Count;
                _references.Clear();
            }
        }
    }
}