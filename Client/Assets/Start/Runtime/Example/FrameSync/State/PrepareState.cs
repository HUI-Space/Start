using System.Threading.Tasks;

namespace Start
{
    /// <summary>
    /// 准备状态(准备战斗数据状态)
    /// </summary>
    public class PrepareState : AsyncFsmState<BattleManager>
    {
        protected override Task OnEnter()
        {
            BattleData battleData = BattleData.Create();
            battleData.BattleType = EBattleType.Observer;
            battleData.FrameInterval = FrameConst.FrameInterval;
            battleData.SceneName = "Assets/Asset/Scene/FrameEngine/FrameEngineScene.unity";
            battleData.PlayerData.Add(new PlayerData()
            {
                ModelPath = "Assets/Asset/Model/FrameEngine/Player/Player.prefab",
                Id = 0,
                Name = "0"
            });
            Fsm.SetData(nameof(BattleData),battleData);
            ChangeState<LoadingState>();
            return base.OnEnter();
        }
    }
}