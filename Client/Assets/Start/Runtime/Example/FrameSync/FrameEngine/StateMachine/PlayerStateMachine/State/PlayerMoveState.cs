namespace Start
{
    [PlayerState(EPlayerState.Move)]
    public class PlayerMoveState : PlayerStateBase
    {
        public override void OnUpdate(MatchEntity matchEntity, PlayerEntity playerEntity)
        {
            MoveSystem.UpdatePosition(playerEntity, matchEntity.DeltaTime);
        }

        public override void OnLateUpdate(MatchEntity matchEntity, PlayerEntity playerEntity)
        {
            if (InputSystem.IsYawStop(playerEntity.Input.Yaw))
            {
                playerEntity.State.NextState = (int)EPlayerState.Idle;
            }
        }
    }
}