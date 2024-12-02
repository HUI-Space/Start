using System;
using System.Threading.Tasks;

namespace Start.Framework
{
    public abstract class AsyncFsmState<T> : IFsmState where T : class
    {
        protected IAsyncFsm<T> Fsm { get; private set; }

        protected internal virtual Task OnInitialize(IAsyncFsm<T> fsm)
        {
            Fsm = fsm;
            return Task.CompletedTask;
        }

        protected internal virtual Task OnEnter()
        {
            return Task.CompletedTask;
        }

        protected internal virtual void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }

        protected internal virtual Task OnExit()
        {
            return Task.CompletedTask;
        }

        protected internal virtual Task OnDeInitialize()
        {
            Fsm = default;
            return Task.CompletedTask;
        }

        protected Task ChangeState<TState>() where TState : AsyncFsmState<T>
        {
            AsyncFsm<T> asyncFsmImplement = (AsyncFsm<T>)Fsm;
            if (asyncFsmImplement == null)
            {
                throw new Exception("AsyncFsm is invalid.");
            }
            return asyncFsmImplement.ChangeState<TState>();
        }
    }
}