namespace Start
{
    [PlayerState(EPlayerState.Idle)]
    public class PlayerIdleState : PlayerStateBase
    {
        public override void OnEnter(MatchEntity matchEntity, PlayerEntity entity)
        {
            base.OnEnter(matchEntity, entity);
        }
        public override void OnUpdate(MatchEntity matchEntity, PlayerEntity entity)
        {
            base.OnUpdate(matchEntity, entity);
        }
        public override void OnExit(MatchEntity matchEntity, PlayerEntity entity)
        {
            base.OnExit(matchEntity, entity);
        }
    }
}