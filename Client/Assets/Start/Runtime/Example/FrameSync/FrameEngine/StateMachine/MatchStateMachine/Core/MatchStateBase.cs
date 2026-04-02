namespace Start
{
    public abstract class MatchStateBase
    {
        public virtual bool TryEnter(MatchEntity matchEntity)
        {
            return true;
        }
        
        public virtual void OnEnter(MatchEntity matchEntity)
        {
            
        }
        
        public virtual void OnUpdate(MatchEntity matchEntity)
        {
            
        }
        
        public virtual void OnExit(MatchEntity matchEntity)
        {
            
        }
    }
}