namespace Start
{
    public abstract class StateBase<T>
    {
        public virtual bool TryEnter(MatchEntity matchEntity, T entity)
        {
            return true;
        }
        
        public virtual void OnEnter(MatchEntity matchEntity, T entity)
        {
            
        }
        
        public virtual void OnUpdate(MatchEntity matchEntity, T entity)
        {
            
        }
        
        public virtual void OnLateUpdate(MatchEntity matchEntity, T entity)
        {
            
        }

        
        public virtual void OnExit(MatchEntity matchEntity, T entity)
        {
            
        }
    }
}