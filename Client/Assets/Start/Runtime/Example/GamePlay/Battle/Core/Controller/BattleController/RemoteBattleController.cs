using System.Collections.Generic;
using System.Diagnostics;


namespace Start
{
    public class RemoteBattleController : SingletonBase<RemoteBattleController>,IBattleController
    {
        public bool Paused { get; private set; }
        
        /// <summary>
        /// 权威帧
        /// </summary>
        public int AuthorityFrame = -1;
        
        /// <summary>
        /// 预测帧
        /// </summary>
        public int PredictionFrame = -1;
        
        public FrameBuffer FrameBuffer { get; private set;}
        
        public TimeCounter TimeCounter { get; private set; }
        
        /// <summary>
        /// 执行的时间
        /// </summary>
        private Stopwatch _stopWatch;
        
        /// <summary>
        /// 权威帧数据
        /// </summary>
        private readonly Queue<Frame> _authorityFrameQueue = new Queue<Frame>();
        
        /// <summary>
        /// 预测帧数据
        /// </summary>
        private readonly Queue<Frame> _predictionFrameQueue = new Queue<Frame>();
        
        /// <summary>
        /// 权威实体
        /// </summary>
        public MatchEntity AuthorityMatchEntity { get; private set; }

        /// <summary>
        /// 预测实体
        /// </summary>
        public MatchEntity PredictionMatchEntity { get; private set; }
        
        /// <summary>
        /// 预测实体队列
        /// </summary>
        private readonly Queue<MatchEntity> _predictionMatchEntityQueue = new Queue<MatchEntity>();
        
        public void StartBattle(BattleData battleData)
        {
            _stopWatch = new Stopwatch();
            FrameBuffer = new FrameBuffer();
            TimeCounter = new TimeCounter(0, 0, BattleConst.FrameInterval);
            _stopWatch.Start();
        }
        
        public void LogicUpdate()
        {
            //当前时间
            long timeNow = _stopWatch.ElapsedMilliseconds;
            //如果当前时间小于下一帧时间 则返回
            
            //先预测
            while (true)
            {
                //如果当前是小于预测帧下一帧时间 则返回
                if (timeNow < TimeCounter.FrameTime(PredictionFrame + 1))
                {
                    break;
                }
                
                // 最多只预测5帧
                if (PredictionFrame - AuthorityFrame > BattleConst.MaxPredictionFrame)
                {
                    break;
                }
                
                //预测帧 + 1
                ++PredictionFrame;
                
                //预测帧数据
                Frame predictionFrame = FrameBuffer.GetPredictionFrame(PredictionFrame);
                
                //添加预测帧数据
                _predictionFrameQueue.Enqueue(predictionFrame);
                
                //预测更新数据
                Prediction(predictionFrame);
                
                //更新表现层

            }
            
            //这样的做的目的，获取权威帧
            for (int i = 0; i < BattleConst.MaxPredictionFrame; i++)
            {
                if (FrameBuffer.TryGetAuthorityFrame(AuthorityFrame + i, out Frame frame))
                {
                    _authorityFrameQueue.Enqueue(frame);
                }
                else
                {
                    break;
                }
            }
            
            while (_authorityFrameQueue.Count > 0)
            {
                //确认帧 + 1
                AuthorityFrame++;
                
                //权威帧数据
                Frame authorityFrame = _authorityFrameQueue.Dequeue();
                
                //预测帧数据
                Frame predictionFrame = _predictionFrameQueue.Dequeue();
                
                // 权威帧和预测帧是否一致
                if (predictionFrame.Equals(authorityFrame))
                {
                    //权威帧和预测帧一致 权威更新数据
                    //预测帧队列出队
                    MatchEntity predictionMatchEntity = _predictionMatchEntityQueue.Dequeue();
                    MatchEntity lastAuthorityMatchEntity = AuthorityMatchEntity;
                    AuthorityMatchEntity = predictionMatchEntity;
                    ReferencePool.Release(lastAuthorityMatchEntity);
                }
                else
                {
                    //权威帧和预测帧不一致 回滚
                    EntityController.UpdateMatchEntity(AuthorityMatchEntity,authorityFrame);
                    EntityController.LogicUpdateState(AuthorityMatchEntity);
                    
                    //预测帧队列出队回收
                    while (_predictionMatchEntityQueue.Count > 0)
                    {
                        MatchEntity predictionMatchEntity = _predictionMatchEntityQueue.Dequeue();
                        ReferencePool.Release(predictionMatchEntity);
                    }
                    _predictionFrameQueue.Clear();
                    
                    //将权威帧数据拷贝给预测帧，然后再回收
                    MatchEntity lastPredictionMatchEntity = PredictionMatchEntity;
                    PredictionMatchEntity = MatchEntity.Copy(AuthorityMatchEntity);
                    ReferencePool.Release(lastPredictionMatchEntity);
                    
                    // 重新执行预测的帧(这样做的是为了防止网络卡顿追帧太快卡住)
                    int predictionFrameCount = AuthorityFrame + 1 + _authorityFrameQueue.Count;
                    for (int i = AuthorityFrame + 1; i <= predictionFrameCount; ++i)
                    {
                        Frame prediction = FrameBuffer.GetPredictionFrame(i);
                        _predictionFrameQueue.Enqueue(prediction);
                        Prediction(prediction);
                    }
                }
            }
        }

        public void NetworkUpdate()
        {
            
        }

        public void RenderUpdate()
        {
            
        }

        public void Pause()
        {
            Paused = true;
        }

        public void Resume()
        {
            Paused = false;
        }


        private void Prediction(Frame predictionFrame)
        {
            EntityController.UpdateMatchEntity(PredictionMatchEntity,predictionFrame);
            EntityController.LogicUpdateState(PredictionMatchEntity);
            MatchEntity matchEntity = MatchEntity.Copy(PredictionMatchEntity);
            _predictionMatchEntityQueue.Enqueue(matchEntity);
        }
    }
}