using System.Collections.Generic;

namespace Start
{
    public partial class MatchController : SingletonBase<MatchController>
    {
        private MatchStateMachine _matchStateMachine;
        private PlayerStateMachine _playerStateMachine;

        private IObjectPool<MatchEntity> _matchEntityPool;
        private IObjectPool<PlayerEntity> _playerEntityPool;

        public override void Initialize()
        {
            _matchStateMachine = new MatchStateMachine();
            _playerStateMachine = new PlayerStateMachine();
            _matchEntityPool = ObjectPoolManager.Instance.CreateObjectPool<MatchEntity>();
            _playerEntityPool = ObjectPoolManager.Instance.CreateObjectPool<PlayerEntity>();
        }

        public override void DeInitialize()
        {
            base.DeInitialize();
            ObjectPoolManager.Instance.DestroyObjectPool<MatchEntity>();
            ObjectPoolManager.Instance.DestroyObjectPool<PlayerEntity>();
            _matchEntityPool = null;
            _playerEntityPool = null;
            _matchStateMachine = null;
            _playerStateMachine = null;
        }

        /// <summary>
        /// 创建MatchEntity
        /// </summary>
        /// <returns></returns>
        public MatchEntity SpawnMatchEntity()
        {
            return _matchEntityPool.Spawn();
        }

        /// <summary>
        /// 创建MatchEntity 
        /// </summary>
        /// <param name="matchEntity">被复制的对象</param>
        /// <returns>返回创建之后复制完成的对象</returns>
        public MatchEntity SpawnMatchEntity(MatchEntity matchEntity)
        {
            MatchEntity newMatchEntity = _matchEntityPool.Spawn();
            matchEntity.CopyTo(newMatchEntity);
            return newMatchEntity;
        }

        /// <summary>
        /// 回收MatchEntity
        /// </summary>
        /// <param name="matchEntity"></param>
        public void UnSpawnMatchEntity(MatchEntity matchEntity)
        {
            _matchEntityPool.UnSpawn(matchEntity);
        }

        /// <summary>
        /// 创建PlayerEntity
        /// </summary>
        public PlayerEntity SpawnPlayerEntity()
        {
            return _playerEntityPool.Spawn();
        }

        /// <summary>
        /// 创建PlayerEntity
        /// </summary>
        /// <param name="playerEntity">被复制的对象</param>
        /// <returns>返回创建之后复制完成的对象</returns>
        public PlayerEntity SpawnPlayerEntity(PlayerEntity playerEntity)
        {
            PlayerEntity newPlayerEntity = _playerEntityPool.Spawn();
            playerEntity.CopyTo(newPlayerEntity);
            return newPlayerEntity;
        }

        /// <summary>
        /// 回收PlayerEntity
        /// </summary>
        /// <param name="playerEntity"></param>
        public void UnSpawnPlayerEntity(PlayerEntity playerEntity)
        {
            _playerEntityPool.UnSpawn(playerEntity);
        }
    }


    public partial class MatchController
    {
        /// <summary>
        /// 确认帧数据
        /// </summary>
        public MatchEntity AuthorityMatchEntity { get; private set; }

        public void InitializeData(MatchEntity matchEntity)
        {
            AuthorityMatchEntity = matchEntity;
        }

        public void DeInitializeData()
        {
            UnSpawnMatchEntity(AuthorityMatchEntity);
            AuthorityMatchEntity = null;
        }

        /// <summary>
        /// MatchEntity 逻辑更新，主要是各种系统开始生效
        /// </summary>
        /// <param name="matchEntity">实体</param>
        /// <param name="frame">帧数据</param>
        /// <param name="time">时间</param>
        public void LogicUpdate(MatchEntity matchEntity, FrameData frame, FP time)
        {
            // 时间
            matchEntity.Time = time;

            // 拷贝玩家输入
            CopyFrameDataToMatchEntity(matchEntity, frame);

            // 更新玩家状态
            UpdatePlayerState(matchEntity);

            // 更新比赛状态
            UpdateMatchState(matchEntity);
        }


        /// <summary>
        /// 拷贝玩家输入到MatchEntity
        /// </summary>
        /// <param name="matchEntity">实体</param>
        /// <param name="frame">帧数据</param>
        private void CopyFrameDataToMatchEntity(MatchEntity matchEntity, FrameData frame)
        {
            matchEntity.AuthorityFrame = frame.AuthorityFrame;
            List<PlayerEntity> playerList = matchEntity.PlayerList;
            foreach (var playerEntity in playerList)
            {
                if (frame.GetInputByIndex(playerEntity.Id, out FrameInput playerInput))
                {
                    playerEntity.Input.Yaw = playerInput.Yaw;
                    playerEntity.Input.Button = playerInput.Button;
                    playerEntity.Input.GiveUp = playerInput.GiveUp;
                }
            }
        }

        private void UpdatePlayerState(MatchEntity matchEntity)
        {
            List<PlayerEntity> playerList = matchEntity.PlayerList;

            // 更新玩家状态
            foreach (PlayerEntity playerEntity in playerList)
            {
                _playerStateMachine.OnUpdate(matchEntity, playerEntity);
            }

            // 晚更新
            foreach (PlayerEntity playerEntity in playerList)
            {
                _playerStateMachine.OnLateUpdate(matchEntity, playerEntity);
            }

            // 物理系统
            PhysicsSystem.LogicUpdate(matchEntity);
            // 各种系统来咯

            // 状态切换
            foreach (PlayerEntity playerEntity in playerList)
            {
                // 状态切换
                _playerStateMachine.ChangeState(matchEntity, playerEntity);
                // 强制状态切换
                _playerStateMachine.ForceChangeState(matchEntity, playerEntity);
            }
        }

        private void UpdateMatchState(MatchEntity matchEntity)
        {
            // 比赛状态
            MatchSystem.LogicUpdate(matchEntity);
            // 状态切换
            _matchStateMachine.Update(matchEntity);
        }
    }
}