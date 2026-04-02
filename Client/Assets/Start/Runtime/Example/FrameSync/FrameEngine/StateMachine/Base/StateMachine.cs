namespace Start
{
    public abstract class StateMachine<T> 
    {
        protected StateBase<T>[] _states;

        public virtual StateBase<T> GetState(int stateId)
        {
            return _states[stateId] as StateBase<T>;
        }

        public virtual void OnUpdate(MatchEntity matchEntity,T t)
        {
          
        }
        
        public virtual void OnLateUpdate(MatchEntity matchEntity,T t)
        {
            
        }
    }
}