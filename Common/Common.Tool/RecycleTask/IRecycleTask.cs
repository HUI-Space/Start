using System.Runtime.CompilerServices;

namespace Start
{
    public interface IRecycleTask : ICriticalNotifyCompletion, IReference
    {
        bool IsCompleted { get; }
    }
}