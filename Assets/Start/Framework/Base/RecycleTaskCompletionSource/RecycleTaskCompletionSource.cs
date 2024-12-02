using System;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace Start.Framework
{
    public enum AwaiterStatus
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
    }

    public class RecycleTaskCompletionSource : IRecycleTaskCompletionSource
    {
        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsCompleted => _awaiterStatus != AwaiterStatus.Pending;

        /// <summary>
        /// 状态
        /// </summary>
        private AwaiterStatus _awaiterStatus;

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

        public static RecycleTaskCompletionSource Create(bool continueOnCapturedContext = true)
        {
            RecycleTaskCompletionSource instance = ReferencePool.Acquire<RecycleTaskCompletionSource>();
            instance._synchronizationContext = continueOnCapturedContext ? SynchronizationContext.Current : null;
            return instance;
        }

        public RecycleTaskCompletionSource()
        {
            _delayTimer = new Timer(DelayTimerCallback, this, Timeout.Infinite, Timeout.Infinite);
        }

        public void Clear()
        {
            _awaiterStatus = AwaiterStatus.Pending;
            _synchronizationContext = default;
            _exception = default;
            _callback = default;
            _hasDelay = default;
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
            if (_awaiterStatus != AwaiterStatus.Pending)
            {
                ExecuteContinuation(action);
            }
            else
            {
                _callback = action;
            }
        }

        public RecycleTaskCompletionSource GetAwaiter()
        {
            return this;
        }

        public void GetResult()
        {
            switch (_awaiterStatus)
            {
                case AwaiterStatus.Succeeded:
                    ReferencePool.Release(this);
                    break;
                case AwaiterStatus.Faulted:
                    ExceptionDispatchInfo c = _callback as ExceptionDispatchInfo;
                    ReferencePool.Release(this);
                    c?.Throw();
                    break;
                default:
                    throw new Exception("RecycleTask not completed");
            }
        }

        public void SetResult()
        {
            if (_awaiterStatus != AwaiterStatus.Pending)
            {
                throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
            }

            SignalCompleted(default);
        }

        public void SetException(Exception exception)
        {
            if (_awaiterStatus != AwaiterStatus.Pending)
            {
                throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
            }

            SignalCompleted(exception ?? throw new ArgumentNullException(nameof(exception)));
        }

        public void SetResultAfter(TimeSpan delay)
        {
            if (_awaiterStatus != AwaiterStatus.Pending)
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
            if (_awaiterStatus != AwaiterStatus.Pending)
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

        private bool SignalCompleted(Exception exception)
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
                _awaiterStatus = AwaiterStatus.Succeeded;
            }
            else
            {
                _awaiterStatus = AwaiterStatus.Faulted;
                _callback = ExceptionDispatchInfo.Capture(exception);
            }

            if (action != null)
            {
                ExecuteContinuation(action);
            }
            return true;
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
            RecycleTaskCompletionSource taskCompletionSource = (RecycleTaskCompletionSource)state;
            if (taskCompletionSource._awaiterStatus != AwaiterStatus.Pending)
            {
                throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
            }

            taskCompletionSource._hasDelay = false;
            taskCompletionSource.SignalCompleted(taskCompletionSource._exception);
        }
    }

    public class RecycleTaskCompletionSource<TResult> : IRecycleTaskCompletionSource
    {
        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsCompleted => _awaiterStatus != AwaiterStatus.Pending;

        /// <summary>
        /// 状态
        /// </summary>
        private AwaiterStatus _awaiterStatus;

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

        public static RecycleTaskCompletionSource<TResult> Create(bool continueOnCapturedContext = true)
        {
            RecycleTaskCompletionSource<TResult> instance = ReferencePool.Acquire<RecycleTaskCompletionSource<TResult>>();
            instance._synchronizationContext = continueOnCapturedContext ? SynchronizationContext.Current : null;
            return instance;
        }

        public RecycleTaskCompletionSource()
        {
            _delayTimer = new Timer(DelayTimerCallback, this, Timeout.Infinite, Timeout.Infinite);
        }

        public void Clear()
        {
            _awaiterStatus = AwaiterStatus.Pending;
            _synchronizationContext = default;
            _exception = default;
            _callback = default;
            _hasDelay = default;
            _result = default;
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
            if (_awaiterStatus != AwaiterStatus.Pending)
            {
                ExecuteContinuation(action);
            }
            else
            {
                _callback = action;
            }
        }

        public RecycleTaskCompletionSource<TResult> GetAwaiter()
        {
            return this;
        }

        public TResult GetResult()
        {
            switch (_awaiterStatus)
            {
                case AwaiterStatus.Succeeded:
                    ReferencePool.Release(this);
                    return _result;
                case AwaiterStatus.Faulted:
                    ExceptionDispatchInfo c = _callback as ExceptionDispatchInfo;
                    ReferencePool.Release(this);
                    c?.Throw();
                    return default;
                default:
                    throw new Exception("RecycleTask not completed");
            }
        }

        public void SetResult(TResult result)
        {
            if (_awaiterStatus != AwaiterStatus.Pending)
            {
                throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
            }

            SignalCompleted(result, default);
        }

        public void SetException(Exception exception)
        {
            if (_awaiterStatus != AwaiterStatus.Pending)
            {
                throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
            }

            SignalCompleted(default, exception ?? throw new ArgumentNullException(nameof(exception)));
        }

        public void SetResultAfter(TResult result, TimeSpan delay)
        {
            if (_awaiterStatus != AwaiterStatus.Pending)
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
            if (_awaiterStatus != AwaiterStatus.Pending)
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
                _awaiterStatus = AwaiterStatus.Succeeded;
            }
            else
            {
                _awaiterStatus = AwaiterStatus.Faulted;
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
            RecycleTaskCompletionSource<TResult> taskCompletionSource = (RecycleTaskCompletionSource<TResult>)state;
            if (taskCompletionSource._awaiterStatus != AwaiterStatus.Pending)
            {
                throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
            }

            taskCompletionSource._hasDelay = false;
            taskCompletionSource.SignalCompleted(taskCompletionSource._result, taskCompletionSource._exception);
        }
    }
}