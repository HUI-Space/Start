using System;
using System.Threading.Tasks;

namespace Start
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
            Fsm = null;
            return Task.CompletedTask;
        }
        
        protected void ChangeState<TState>() where TState : AsyncFsmState<T>
        {
            if (Fsm == null)
            {
                throw new Exception("AsyncFsm 为空.");
            }
            Fsm.ChangeState<TState>();
        }
    }
}