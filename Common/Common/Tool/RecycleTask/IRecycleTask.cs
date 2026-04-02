using System;
using System.Runtime.CompilerServices;

namespace Start
{
    public interface IRecycleTask : ICriticalNotifyCompletion, IRecycle
    {
        bool IsCompleted { get; }
        
        bool IsRecycle { get; }
    }
}