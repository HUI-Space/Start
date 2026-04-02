namespace Start
{
    public static class MoveSystem
    {
        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="playerEntity">玩家实体</param>
        /// <param name="deltaTime">时间</param>
        public static void UpdatePosition(PlayerEntity playerEntity,FP deltaTime)
        {
            playerEntity.Move.Direction = InputSystem.YawToTSVector(playerEntity.Input.Yaw);
            playerEntity.Transform.Position = (playerEntity.Transform.Position +
                                               playerEntity.Move.Direction * playerEntity.Move.Speed * deltaTime);
        }
        
        public static void UpdateRotation(PlayerEntity playerEntity,FP deltaTime)
        {
            
        }
    }
}