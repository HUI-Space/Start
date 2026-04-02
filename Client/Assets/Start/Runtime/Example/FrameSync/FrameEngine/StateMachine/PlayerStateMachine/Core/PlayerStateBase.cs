namespace Start
{
    public abstract class PlayerStateBase : StateBase<PlayerEntity>
    {
        public virtual void OnCollision( MatchEntity matchEntity,PlayerEntity source, PlayerEntity target)
        {

        }
    }
}