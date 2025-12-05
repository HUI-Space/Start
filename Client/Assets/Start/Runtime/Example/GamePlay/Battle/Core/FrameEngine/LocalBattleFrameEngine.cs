using System.Collections.Generic;

namespace Start
{
    public class LocalBattleFrameEngine : BattleFrameEngineBase
    {
        /// <summary>
        /// 权威实体
        /// </summary>
        public MatchEntity AuthorityMatchEntity { get; private set; }
        
        /// <summary>
        /// 渲染实体
        /// </summary>
        public MatchEntity RenderMatchEntity { get; private set; }
        

        protected override void StartEngine(BattleData battleData)
        {
            RecyclableObjectPool.Add<MatchEntity>(10);
            RecyclableObjectPool.Add<PlayerEntity>(10);
            AuthorityMatchEntity = RecyclableObjectPool.Acquire<MatchEntity>();
            AuthorityMatchEntity.AuthorityFrame = -1;
            AuthorityMatchEntity.DeltaTime = new FP(battleData.FrameInterval) / new FP(1000);
            AuthorityMatchEntity.TimeScale = FP.One;
            AuthorityMatchEntity.PlayerList = new List<PlayerEntity>(battleData.Player.Count);
            for (int i = 0; i < battleData.Player.Count; i++)
            {
                AuthorityMatchEntity.PlayerList.Add(RecyclableObjectPool.Acquire<PlayerEntity>());
            }
        }

        protected override void LogicUpdate()
        {
            //当前LogicUpdate执行时间
            long logicUpdateRunningTime = _stopWatch.ElapsedMilliseconds;
            //当前帧
            int authorityFrame = AuthorityMatchEntity.AuthorityFrame;
            //最大追赶帧数
            int maxCatchUpFrame = 3;
            //追赶帧数
            int catchUpCount = 0;
            //如果当前LogicUpdate执行时间 大于下一帧时间则执行逻辑 如果小于下一帧时间则返回     这个 while 循环可以实现追帧
            //TODO 单机模式看是否需要变为定点数进行判断逻辑更新
            while (logicUpdateRunningTime >= _timeCounter.FrameTime(authorityFrame + 1) && catchUpCount < maxCatchUpFrame)
            { 
                authorityFrame ++;
                catchUpCount ++;
                //帧数
                AuthorityMatchEntity.AuthorityFrame = authorityFrame;
                //1.获取玩家的操作
                FrameInput playerInput = InputController.Instance.GetInput();
                //2.生成一个完整的逻辑帧数据
                FrameData frame = new FrameData(authorityFrame,new FrameInput[1]{playerInput});
                //3.将逻辑帧数据保存到重播系统中
                ReplayController.Instance.SaveFrame(frame);
                //4.拷贝玩家输入到MatchEntity
                MatchController.Instance.CopyFrameDataToMatchEntity(AuthorityMatchEntity,frame);
                //5.调用比赛控制器的逻辑更新
                MatchController.Instance.LogicUpdateState(AuthorityMatchEntity);
                //6.其他系统..
                //7.拷贝到渲染实体
                if (RenderMatchEntity != null)
                {
                    RecyclableObjectPool.Recycle(RenderMatchEntity);
                }
                RenderMatchEntity = MatchEntity.Copy(AuthorityMatchEntity);
                
                UnityEngine.Debug.Log($"逻辑帧:{authorityFrame} 时间:{logicUpdateRunningTime} 玩家输入:{playerInput}  ");
            }
        }

        protected override void RenderUpdate()
        {
            //开始渲染
        }
    }
}