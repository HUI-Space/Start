using System.Runtime.CompilerServices;

namespace Start
{
    public interface IRecycleTask : ICriticalNotifyCompletion, IReusable
    {
        bool IsCompleted { get; }
    }
}