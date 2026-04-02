namespace Start
{
    [PlayerState(EPlayerState.Idle)]
    public class PlayerIdleState : PlayerStateBase
    {
        
        public override void OnLateUpdate(MatchEntity matchEntity, PlayerEntity playerEntity)
        {
            if (!InputSystem.IsYawStop(playerEntity.Input.Yaw))
            {
                playerEntity.State.NextState = (int)EPlayerState.Move;
            }
        }
    }
}