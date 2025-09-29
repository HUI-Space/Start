using System.Diagnostics;

namespace Start
{
    public class LocalBattleFrameEngine : BattleFrameEngineBase
    {
        /// <summary>
        /// 权威帧
        /// </summary>
        public int AuthorityFrame = -1;
        
        public FrameTimeCounter TimeCounter { get; private set; }
        
        /// <summary>
        /// 执行的时间
        /// </summary>
        private Stopwatch _stopWatch;
        
        public LocalBattleFrameEngine()
        {
            TimeCounter = new FrameTimeCounter(0, 0, FrameConst.FrameInterval);
            _stopWatch = new Stopwatch();
            _stopWatch.Start();
        }
        
        protected override void LogicUpdate()
        {
            //当前LogicUpdate执行时间
            long logicUpdateRunningTime = _stopWatch.ElapsedMilliseconds;
            //如果当前LogicUpdate执行时间 大于下一帧时间则执行逻辑 如果小于下一帧时间则返回     这个 while 循环可以实现追帧
            while (logicUpdateRunningTime > TimeCounter.FrameTime(AuthorityFrame + 1))
            {
                AuthorityFrame += 1;
                //1.获取玩家的操作
                FrameInput playerInput = InputController.Instance.GetInput();
                //2.生成一个完整的逻辑帧数据
                FrameData frame = new FrameData(AuthorityFrame,new FrameInput[1]{playerInput});
                //3.将逻辑帧数据保存到重播系统中
                ReplayController.Instance.SaveFrame(frame);
                //4.调用比赛控制器的逻辑更新
                MatchController.Instance.LogicUpdate(frame);
                //5.
                
            }
        }

        protected override void RenderUpdate()
        {
            
            
        }
    }
}