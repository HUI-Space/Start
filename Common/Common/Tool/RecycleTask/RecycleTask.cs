using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Start
{
    public enum EAwaiterStatus
    {
        /// <summary>
        /// 尚未完成
        /// </summary>
        Pending = 0,

        /// <summary>
        /// 成功
        /// </summary>
        Succeeded = 1,

        /// <summary>
        /// 失败
        /// </summary>
        Faulted = 2,
        
        Canceled = 3,
    }

    /// <summary>
    /// 可回收的Task Recycle TaskCompletionSource
    /// </summary>
    public partial class RecycleTask : IRecycleTask
    {
        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsCompleted => _awaiterStatus != EAwaiterStatus.Pending;

        /// <summary>
        /// 是否回收
        /// </summary>
        public bool IsRecycle { get; private set; }
        
        /// <summary>
        /// 状态
        /// </summary>
        private EAwaiterStatus _awaiterStatus;

        /// <summary>
        /// 延时timer
        /// </summary>
        private readonly Timer _delayTimer;

        /// <summary>
        /// 异常值
        /// </summary>
        private Exception _exception;

        /// <summary>
        /// 回调
        /// </summary>
        private object _callback;

        /// <summary>
        /// 是否有延时
        /// </summary>
        private bool _hasDelay;

        /// <summary>
        /// 同步上下文 
        /// </summary>
        private SynchronizationContext _synchronizationContext;

        public static RecycleTask Create(bool continueOnCapturedContext = true)
        {
            RecycleTask instance = RecyclablePool.Acquire<RecycleTask>();
            instance.IsRecycle = false;
            instance._awaiterStatus = EAwaiterStatus.Pending;
            instance._synchronizationContext = continueOnCapturedContext ? SynchronizationContext.Current : null;
            return instance;
        }

        public RecycleTask()
        {
            _delayTimer = new Timer(DelayTimerCallback, this, Timeout.Infinite, Timeout.Infinite);
        }

        public void Recycle()
        {
            IsRecycle = true;
            _awaiterStatus = EAwaiterStatus.Pending;
            _synchronizationContext = null;
            _exception = null;
            _callback = null;
            _hasDelay = false;
            _delayTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void OnCompleted(Action action)
        {
            UnsafeOnCompleted(action);
        }

        /// <summary>
        /// 用于在异步操作完成时通知调用者。
        /// 提供了一种更高效的方式来注册回调，但需要注意线程安全问题
        /// 调用者必须确保回调的线程安全性。
        /// 通常情况下，除非你明确知道需要更高的性能并且能够处理潜在的线程安全问题，否则建议使用 OnCompleted 方法。
        /// </summary>
        /// <param name="action"></param>
        public void UnsafeOnCompleted(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            
            if (_awaiterStatus != EAwaiterStatus.Pending)
            {
                ExecuteContinuation(action);
            }
            else
            {
                _callback = action;
            }
        }

        public RecycleTask GetAwaiter()
        {
            return this;
        }

        public void GetResult()
        {
            if (IsRecycle)
            {
                throw new ObjectDisposedException(nameof(RecycleTask), "Task has been recycled");
            }
                
            switch (_awaiterStatus)
            {
                case EAwaiterStatus.Succeeded:
                    RecyclablePool.Recycle(this);
                    break;
                case EAwaiterStatus.Faulted:
                    ExceptionDispatchInfo c = _callback as ExceptionDispatchInfo;
                    RecyclablePool.Recycle(this);
                    c?.Throw();
                    break;
                default:
                    throw new InvalidOperationException("RecycleTask not completed");
            }
        }

        public void SetResult()
        {
            if (IsRecycle)
            {
                throw new ObjectDisposedException(nameof(RecycleTask), "Task has been recycled");
            }
            
            if (_awaiterStatus != EAwaiterStatus.Pending)
            {
                throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
            }

            SignalCompleted(default);
        }

        public void SetException(Exception exception)
        {
            if (IsRecycle)
            {
                throw new ObjectDisposedException(nameof(RecycleTask), "Task has been recycled");
            }
            
            if (_awaiterStatus != EAwaiterStatus.Pending)
            {
                throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
            }

            SignalCompleted(exception ?? throw new ArgumentNullException(nameof(exception)));
        }

        public void SetResultAfter(TimeSpan delay)
        {
            if (IsRecycle)
            {
                throw new ObjectDisposedException(nameof(RecycleTask), "Task has been recycled");
            }
            
            if (_awaiterStatus != EAwaiterStatus.Pending)
            {
                throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
            }

            if (delay < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(delay));
            }

            _exception = null;
            _hasDelay = true;
            _delayTimer.Change(delay, Timeout.InfiniteTimeSpan);
        }

        public void SetExceptionAfter(Exception exception, TimeSpan delay)
        {
            if (IsRecycle)
            {
                throw new ObjectDisposedException(nameof(RecycleTask), "Task has been recycled");
            }
            
            if (_awaiterStatus != EAwaiterStatus.Pending)
            {
                throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
            }

            if (delay < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(delay));
            }

            _hasDelay = true;
            _exception = exception ?? throw new ArgumentNullException(nameof(exception));

            _delayTimer.Change(delay, Timeout.InfiniteTimeSpan);
        }

        private void SignalCompleted(Exception exception)
        {
            if (_hasDelay)
            {
                _hasDelay = false;
                _delayTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }

            Action action = _callback as Action;
            _exception = exception;

            if (exception == null)
            {
                _awaiterStatus = EAwaiterStatus.Succeeded;
            }
            else
            {
                _awaiterStatus = EAwaiterStatus.Faulted;
                _callback = ExceptionDispatchInfo.Capture(exception);
            }

            if (action != null)
            {
                ExecuteContinuation(action);
            }
        }

        /// <summary>
        /// 执行延续任务
        /// </summary>
        /// <param name="action">延续任务</param>
        private void ExecuteContinuation(Action action)
        {
            if (_synchronizationContext == null)
            {
                ThreadPool.UnsafeQueueUserWorkItem(state => ((Action)state)(), action);
            }
            else
            {
                _synchronizationContext.Post(state => ((Action)state)(), action);
            }
        }

        private static void DelayTimerCallback(object state)
        {
            RecycleTask recycleTask = (RecycleTask)state;
            
            // 检查对象是否已被回收
            if (recycleTask.IsRecycle)
                return;
            
            if (recycleTask._awaiterStatus != EAwaiterStatus.Pending)
            {
                throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
            }

            recycleTask._hasDelay = false;
            recycleTask.SignalCompleted(recycleTask._exception);
        }
    }

    public partial class RecycleTask<TResult> : IRecycleTask
    {
        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsCompleted => _awaiterStatus != EAwaiterStatus.Pending;

        /// <summary>
        /// 是否回收
        /// </summary>
        public bool IsRecycle { get; private set; }
        
        /// <summary>
        /// 状态
        /// </summary>
        private EAwaiterStatus _awaiterStatus;

        /// <summary>
        /// 延时timer
        /// </summary>
        private readonly Timer _delayTimer;

        /// <summary>
        /// 异常值
        /// </summary>
        private Exception _exception;

        /// <summary>
        /// 结果值
        /// </summary>
        private TResult _result;

        /// <summary>
        /// 回调
        /// </summary>
        private object _callback;

        /// <summary>
        /// 是否有延时
        /// </summary>
        private bool _hasDelay;

        /// <summary>
        /// 同步上下文 
        /// </summary>
        private SynchronizationContext _synchronizationContext;

        public static RecycleTask<TResult> Create(bool continueOnCapturedContext = true)
        {
            RecycleTask<TResult> instance = RecyclablePool.Acquire<RecycleTask<TResult>>();
            instance.IsRecycle = false;
            instance._awaiterStatus = EAwaiterStatus.Pending;
            instance._synchronizationContext = continueOnCapturedContext ? SynchronizationContext.Current : null;
            return instance;
        }

        public RecycleTask()
        {
            _delayTimer = new Timer(DelayTimerCallback, this, Timeout.Infinite, Timeout.Infinite);
        }

        public void Recycle()
        {
            IsRecycle = true;
            _awaiterStatus = EAwaiterStatus.Pending;
            _synchronizationContext = null;
            _exception = null;
            _callback = null;
            _hasDelay = false;
            _result = default;
            _delayTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void OnCompleted(Action action)
        {
            UnsafeOnCompleted(action);
        }

        /// <summary>
        /// 用于在异步操作完成时通知调用者。
        /// 提供了一种更高效的方式来注册回调，但需要注意线程安全问题
        /// 调用者必须确保回调的线程安全性。
        /// 通常情况下，除非你明确知道需要更高的性能并且能够处理潜在的线程安全问题，否则建议使用 OnCompleted 方法。
        /// </summary>
        /// <param name="action"></param>
        public void UnsafeOnCompleted(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            
            if (_awaiterStatus != EAwaiterStatus.Pending)
            {
                ExecuteContinuation(action);
            }
            else
            {
                _callback = action;
            }
        }

        public RecycleTask<TResult> GetAwaiter()
        {
            return this;
        }

        public TResult GetResult()
        {
            if (IsRecycle)
                throw new ObjectDisposedException(nameof(RecycleTask<TResult>), "Task has been recycled");

            switch (_awaiterStatus)
            {
                case EAwaiterStatus.Succeeded:
                    RecyclablePool.Recycle(this);
                    return _result;
                case EAwaiterStatus.Faulted:
                    ExceptionDispatchInfo c = _callback as ExceptionDispatchInfo;
                    RecyclablePool.Recycle(this);
                    c?.Throw();
                    return default;
                default:
                    throw new InvalidOperationException("RecycleTask not completed");
            }
        }

        public void SetResult(TResult result)
        {
            if (IsRecycle)
                throw new ObjectDisposedException(nameof(RecycleTask<TResult>), "Task has been recycled");
            
            if (_awaiterStatus != EAwaiterStatus.Pending)
            {
                throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
            }

            SignalCompleted(result, default);
        }

        public void SetException(Exception exception)
        {
            if (IsRecycle)
                throw new ObjectDisposedException(nameof(RecycleTask<TResult>), "Task has been recycled");

            if (_awaiterStatus != EAwaiterStatus.Pending)
            {
                throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
            }

            SignalCompleted(default, exception ?? throw new ArgumentNullException(nameof(exception)));
        }

        public void SetResultAfter(TResult result, TimeSpan delay)
        {
            if (IsRecycle)
                throw new ObjectDisposedException(nameof(RecycleTask<TResult>), "Task has been recycled");

            if (_awaiterStatus != EAwaiterStatus.Pending)
            {
                throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
            }

            if (delay < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(delay));
            }

            _result = result;
            _exception = null;
            _hasDelay = true;
            _delayTimer.Change(delay, Timeout.InfiniteTimeSpan);
        }

        public void SetExceptionAfter(Exception exception, TimeSpan delay)
        {
            if (IsRecycle)
                throw new ObjectDisposedException(nameof(RecycleTask<TResult>), "Task has been recycled");

            if (_awaiterStatus != EAwaiterStatus.Pending)
            {
                throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
            }

            if (delay < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(delay));
            }

            _hasDelay = true;
            _result = default;
            _exception = exception ?? throw new ArgumentNullException(nameof(exception));

            _delayTimer.Change(delay, Timeout.InfiniteTimeSpan);
        }

        private void SignalCompleted(TResult result, Exception exception)
        {
            if (_hasDelay)
            {
                _hasDelay = false;
                _delayTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }

            _result = result;
            _exception = exception;

            Action action = _callback as Action;

            if (exception == null)
            {
                _awaiterStatus = EAwaiterStatus.Succeeded;
            }
            else
            {
                _awaiterStatus = EAwaiterStatus.Faulted;
                _callback = ExceptionDispatchInfo.Capture(exception);
            }

            if (action != null)
            {
                ExecuteContinuation(action);
            }
        }

        /// <summary>
        /// 执行延续任务
        /// </summary>
        /// <param name="action">延续任务</param>
        private void ExecuteContinuation(Action action)
        {
            if (_synchronizationContext == null)
            {
                ThreadPool.UnsafeQueueUserWorkItem(state => ((Action)state)(), action);
            }
            else
            {
                _synchronizationContext.Post(state => ((Action)state)(), action);
            }
        }

        private static void DelayTimerCallback(object state)
        {
            RecycleTask<TResult> recycleTask = (RecycleTask<TResult>)state;
            if (recycleTask.IsRecycle)
                return;
            
            if (recycleTask._awaiterStatus != EAwaiterStatus.Pending)
            {
                throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
            }

            recycleTask._hasDelay = false;
            recycleTask.SignalCompleted(recycleTask._result, recycleTask._exception);
        }
    }
    
    public partial class RecycleTask
    {
        public static async Task WaitAny(params RecycleTask[] tasks)
        {
            if (tasks == null)
                throw new ArgumentNullException(nameof(tasks));
            if (tasks.Length == 0)
                throw new ArgumentException("tasks array is empty", nameof(tasks));
            
            RecycleTask completionTask = Create();
            int completedCount = 0;
            
            Action callback = () =>
            {
                if (Interlocked.Increment(ref completedCount) == 1)
                {
                    completionTask.SetResult();
                }
            };
            
            foreach (var task in tasks)
            {
                if (task != null)
                    task.OnCompleted(callback);
            }
            
            await completionTask;
        }

        public static async Task WaitAny(IList<RecycleTask> tasks)
        {
            if (tasks == null)
                throw new ArgumentNullException(nameof(tasks));
            if (tasks.Count == 0)
                throw new ArgumentException("tasks list is empty", nameof(tasks));
            
            RecycleTask completionTask = Create();
            int completedCount = 0;
            
            Action callback = () =>
            {
                if (Interlocked.Increment(ref completedCount) == 1)
                {
                    completionTask.SetResult();
                }
            };
            
            foreach (var task in tasks)
            {
                if (task != null)
                    task.OnCompleted(callback);
            }
            
            await completionTask;
        }
        

        public static async Task WaitAll(params RecycleTask[] tasks)
        {
            if (tasks == null)
                throw new ArgumentNullException(nameof(tasks));
            if (tasks.Length == 0)
                return;
            
            RecycleTask completionTask = Create();
            int remainingCount = tasks.Length;
            
            Action callback = () =>
            {
                if (Interlocked.Decrement(ref remainingCount) == 0)
                {
                    completionTask.SetResult();
                }
            };
            
            foreach (var task in tasks)
            {
                if (task != null)
                    task.OnCompleted(callback);
                else
                    callback(); // null 任务视为已完成
            }
            
            await completionTask;
        }

        public static async Task WaitAll(IList<RecycleTask> tasks)
        {
            if (tasks == null)
                throw new ArgumentNullException(nameof(tasks));
            if (tasks.Count == 0)
                return ;
            
            RecycleTask completionTask = Create();
            int remainingCount = tasks.Count;
            
            Action callback = () =>
            {
                if (Interlocked.Decrement(ref remainingCount) == 0)
                {
                    completionTask.SetResult();
                }
            };
            
            foreach (var task in tasks)
            {
                if (task != null)
                    task.OnCompleted(callback);
                else
                    callback();
            }
            
            await completionTask;
        }
        
    }

    public partial class RecycleTask<TResult>
    {
        public static async Task WaitAny(params RecycleTask<TResult>[] tasks)
        {
            if (tasks == null)
                throw new ArgumentNullException(nameof(tasks));
            if (tasks.Length == 0)
                throw new ArgumentException("tasks array is empty", nameof(tasks));
            
            RecycleTask completionTask = RecycleTask.Create();
            int completedCount = 0;
            
            Action callback = () =>
            {
                if (Interlocked.Increment(ref completedCount) == 1)
                {
                    completionTask.SetResult();
                }
            };
            
            foreach (var task in tasks)
            {
                if (task != null)
                    task.OnCompleted(callback);
            }
            
            await completionTask;
        }
        
        public static async Task WaitAny(IList<RecycleTask<TResult>> tasks)
        {
            if (tasks == null)
                throw new ArgumentNullException(nameof(tasks));
            if (tasks.Count == 0)
                throw new ArgumentException("tasks list is empty", nameof(tasks));
            
            RecycleTask completionTask = RecycleTask.Create();
            int completedCount = 0;
            
            Action callback = () =>
            {
                if (Interlocked.Increment(ref completedCount) == 1)
                {
                    completionTask.SetResult();
                }
            };
            
            foreach (var task in tasks)
            {
                if (task != null)
                    task.OnCompleted(callback);
            }
            
            await completionTask;
        }
        

        public static async Task WaitAll(params RecycleTask<TResult>[] tasks)
        {
            if (tasks == null)
                throw new ArgumentNullException(nameof(tasks));
            if (tasks.Length == 0)
                return;
            
            RecycleTask completionTask = RecycleTask.Create();
            int remainingCount = tasks.Length;
            
            Action callback = () =>
            {
                if (Interlocked.Decrement(ref remainingCount) == 0)
                {
                    completionTask.SetResult();
                }
            };
            
            foreach (var task in tasks)
            {
                if (task != null)
                    task.OnCompleted(callback);
                else
                    callback();
            }
            
            await completionTask;
        }
        
        public static async Task WaitAll(IList<RecycleTask<TResult>> tasks)
        {
            if (tasks == null)
                throw new ArgumentNullException(nameof(tasks));
            if (tasks.Count == 0)
                return;
            
            RecycleTask completionTask = RecycleTask.Create();
            int remainingCount = tasks.Count;
            
            Action callback = () =>
            {
                if (Interlocked.Decrement(ref remainingCount) == 0)
                {
                    completionTask.SetResult();
                }
            };
            
            foreach (var task in tasks)
            {
                if (task != null)
                    task.OnCompleted(callback);
                else
                    callback();
            }
            
            await completionTask;
        }
    }
}