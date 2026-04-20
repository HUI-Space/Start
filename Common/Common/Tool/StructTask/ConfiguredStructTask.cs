using System;
using System.Runtime.CompilerServices;

namespace Start
{
    /// <summary>
    /// 配置后的 StructTask 等待器
    /// </summary>
    public readonly struct ConfiguredStructTask : ICriticalNotifyCompletion
    {
        private readonly StructTask _task;
        private readonly bool _continueOnCapturedContext;

        public ConfiguredStructTask(StructTask task, bool continueOnCapturedContext)
        {
            _task = task;
            _continueOnCapturedContext = continueOnCapturedContext;
        }

        public bool IsCompleted => _task.IsCompleted;

        public ConfiguredStructTask GetAwaiter()
        {
            return this;
        }

        public void GetResult()
        {
            _task.GetResult();
        }

        public void OnCompleted(Action continuation)
        {
            _task.OnCompleted(continuation, allowDuplicate: true, _continueOnCapturedContext);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            _task.OnCompleted(continuation, allowDuplicate: true, _continueOnCapturedContext);
        }
    }

    /// <summary>
    /// 配置后的泛型 StructTask 等待器
    /// </summary>
    public readonly struct ConfiguredStructTask<T> : ICriticalNotifyCompletion
    {
        private readonly StructTask<T> _task;
        private readonly bool _continueOnCapturedContext;

        public ConfiguredStructTask(StructTask<T> task, bool continueOnCapturedContext)
        {
            _task = task;
            _continueOnCapturedContext = continueOnCapturedContext;
        }

        public bool IsCompleted => _task.IsCompleted;

        public ConfiguredStructTask<T> GetAwaiter()
        {
            return this;
        }

        public T GetResult()
        {
            return _task.GetResult();
        }

        public void OnCompleted(Action continuation)
        {
            _task.OnCompleted(continuation, allowDuplicate: true, _continueOnCapturedContext);
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            _task.OnCompleted(continuation, allowDuplicate: true, _continueOnCapturedContext);
        }
    }
}