using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace Start
{
    /// <summary>
    /// 可回收对象池的单类型存储桶（管理某一种类型的可复用对象）
    /// </summary>
    public class RecyclablePoolBucket
    {
        private Type _type;
        private int _unusedCount;
        private int _usingCount;
        private int _acquireCount;
        private int _releaseCount;
        private int _addCount;
        private int _removeCount;
        private int _totalCreateCount;
        private int _peakUsingCount;

        private readonly ConcurrentQueue<IReusable> _unused = new ConcurrentQueue<IReusable>();

        public RecyclablePoolBucket(Type type)
        {
            _type = type;
        }

        /// <summary>
        /// 获取对象池统计信息
        /// </summary>
        /// <returns></returns>
        public RecyclablePoolStats GetRecyclablePoolStats()
        {
            return new RecyclablePoolStats(_type, _unusedCount, _usingCount, _acquireCount, _releaseCount, _addCount, _removeCount, _totalCreateCount, _peakUsingCount, DateTime.Now);
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <typeparam name="T">要获取的对象类型</typeparam>
        /// <returns></returns>
        public T Acquire<T>() where T : IReusable, new()
        {
            if (_unused.TryDequeue(out IReusable item))
            {
                Interlocked.Increment(ref _usingCount);
                Interlocked.Increment(ref _acquireCount);
                UpdatePeakCount();
                return (T)item;
            }
            
            Interlocked.Increment(ref _usingCount);
            Interlocked.Increment(ref _acquireCount);
            Interlocked.Increment(ref _totalCreateCount);
            UpdatePeakCount();
            return (T)Activator.CreateInstance(_type);
        }
        
        /// <summary>
        /// 归还对象到池（自动重置状态）
        /// </summary>
        /// <param name="reusable"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public void Recycle(IReusable reusable)
        {
            if (reusable == null)
            {
                throw new ArgumentNullException(nameof(reusable));
            }
            
            reusable.Reset();
            if (_unused.Contains(reusable))
            {
                throw new Exception("The recycle has been released.");
            }
            _unused.Enqueue(reusable);
            Interlocked.Decrement(ref _usingCount);
            Interlocked.Increment(ref _releaseCount);
        }
        
        /// <summary>
        /// 添加对象到池
        /// </summary>
        /// <param name="count"></param>
        public void Add(int count)
        {
            if (count <= 0) return;
            for (int i = 0; i < count; i++)
            {
                _unused.Enqueue(Activator.CreateInstance(_type) as IReusable);
                Interlocked.Increment(ref _addCount);
                Interlocked.Increment(ref _unusedCount);
                Interlocked.Increment(ref _totalCreateCount);
            }
        }
        
        /// <summary>
        /// 移除对象
        /// </summary>
        /// <param name="count"></param>
        public void Remove(int count)
        {
            if (count <= 0) return;
            for (int i = 0; i < count; i++)
            {
                if (_unused.TryDequeue(out IReusable item))
                {
                    Interlocked.Increment(ref _removeCount);
                    Interlocked.Decrement(ref _unusedCount);
                }
            }
        }
        
        /// <summary>
        /// 清空当前桶的闲置对象
        /// </summary>
        public void Clear()
        {
            while (_unused.TryDequeue(out _))
            {
                // 循环出队，清空队列
            }
            // 修正：重置计数器
            Interlocked.Exchange(ref _unusedCount, 0);
            Interlocked.Exchange(ref _usingCount, 0);
            Interlocked.Exchange(ref _acquireCount, 0);
            Interlocked.Exchange(ref _releaseCount, 0);
            Interlocked.Exchange(ref _addCount, 0);
            Interlocked.Exchange(ref _removeCount, 0);
            Interlocked.Exchange(ref _totalCreateCount, 0);
            Interlocked.Exchange(ref _peakUsingCount, 0);
        }
        
        /// <summary>
        /// 更新峰值使用量
        /// </summary>
        private void UpdatePeakCount()
        {
            int currentUsing = Volatile.Read(ref _usingCount);
            int currentPeak = Volatile.Read(ref _peakUsingCount);
        
            // 修正：CAS更新峰值，避免多线程覆盖
            while (currentUsing > currentPeak)
            {
                if (Interlocked.CompareExchange(ref _peakUsingCount, currentUsing, currentPeak) == currentPeak)
                {
                    break;
                }
                currentPeak = Volatile.Read(ref _peakUsingCount);
            }
        }
    }
}