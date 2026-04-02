namespace Start
{
    public class LocalFrameEngine : FrameEngineBase
    {
        
        protected override void LogicUpdate()
        {
            MatchEntity matchEntity = MatchController.Instance.AuthorityMatchEntity;
            //当前LogicUpdate执行时间
            long logicUpdateRunningTime = _stopWatch.ElapsedMilliseconds;
            //当前帧
            int authorityFrame = matchEntity.AuthorityFrame;
            //追赶帧数
            int catchUpCount = 0;
            //如果当前LogicUpdate执行时间 大于下一帧时间则执行逻辑 如果小于下一帧时间则返回     这个 while 循环可以实现追帧 （最大追赶帧数）
            while (logicUpdateRunningTime >= _timeCounter.FrameTime(authorityFrame + 1) && catchUpCount < FrameConst.MaxCatchUpFrame)
            {
                authorityFrame ++;
                catchUpCount ++;
                //1获取本地玩家帧数据
                FrameData frameData = InputController.Instance.GetLocalBattleFrameData(authorityFrame);
                //2.将逻辑帧数据保存到重播系统中
                ReplayController.Instance.LogicUpdate(frameData);
                //3.调用比赛控制器的逻辑更新
                MatchController.Instance.LogicUpdate(matchEntity,frameData,_timeCounter.FrameTime(authorityFrame));
                //4.渲染实体逻辑更新
                RenderController.Instance.LogicUpdate(matchEntity);
                UnityEngine.Debug.Log($"逻辑更新时间：{logicUpdateRunningTime} 时间:{matchEntity.Time} 玩家输入:{frameData}");
            }
        }

        protected override void RenderUpdate()
        {
            RenderController.Instance.RenderUpdate();
        }
    }
}