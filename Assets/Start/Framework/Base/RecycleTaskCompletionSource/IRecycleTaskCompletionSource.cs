using System.Runtime.CompilerServices;

namespace Start.Framework
{
    public interface IRecycleTaskCompletionSource:ICriticalNotifyCompletion,IReference
    {
        bool IsCompleted { get; }
    }
}