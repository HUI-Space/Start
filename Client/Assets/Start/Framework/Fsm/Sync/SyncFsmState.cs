using System;

namespace Start
{
    public abstract class SyncFsmState<T> : IFsmState where T : class
    {
        protected ISyncFsm<T> Fsm { get; private set; }
        
        protected internal virtual void OnInitialize(ISyncFsm<T> fsm)
        {
            Fsm = fsm;
        }
        
        protected internal virtual void OnEnter()
        {
            
        }
        
        protected internal virtual void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            
        }

        protected internal virtual void OnExit()
        {
            
        }
        
        protected internal virtual void OnDeInitialize()
        {
            Fsm = default;
        }
        
        protected void ChangeState<TState>() where TState : SyncFsmState<T>
        {
            if (Fsm == null)
            {
                throw new Exception("SyncFsm 为空.");
            }
            Fsm.ChangeState<TState>();
        }
    }
}