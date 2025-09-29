﻿using System.Collections.Generic;
using System.Diagnostics;

namespace Start
{
    public class RemoteBattleFrameEngine : BattleFrameEngineBase
    {
        /// <summary>
        /// 权威帧
        /// </summary>
        public int AuthorityFrame = -1;
        
        /// <summary>
        /// 预测帧
        /// </summary>
        public int PredictionFrame = -1;
        
        public FrameBuffer FrameBuffer { get; private set;}
        
        public FrameTimeCounter TimeCounter { get; private set; }
        
        /// <summary>
        /// 执行的时间
        /// </summary>
        private Stopwatch _stopWatch;
        
        /// <summary>
        /// 权威帧数据
        /// </summary>
        private readonly Queue<FrameData> _authorityFrameQueue = new Queue<FrameData>();
        
        /// <summary>
        /// 预测帧数据
        /// </summary>
        private readonly Queue<FrameData> _predictionFrameQueue = new Queue<FrameData>();
        
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
        
        public RemoteBattleFrameEngine()
        {
            TimeCounter = new FrameTimeCounter(0, 0, FrameConst.FrameInterval);
            FrameBuffer = new FrameBuffer();
            _stopWatch = new Stopwatch();
            _stopWatch.Start();
        }
        
        protected override void NetworkUpdate()
        {
            
        }

        protected override void LogicUpdate()
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
                if (PredictionFrame - AuthorityFrame > FrameConst.MaxPredictionFrame)
                {
                    break;
                }
                
                //预测帧 + 1
                ++PredictionFrame;
                
                //预测帧数据
                FrameData predictionFrame = FrameBuffer.GetPredictionFrameData(PredictionFrame);
                
                //添加预测帧数据
                _predictionFrameQueue.Enqueue(predictionFrame);
                
                //预测更新数据
                Prediction(predictionFrame);
                
                //更新表现层

            }
            
            //这样的做的目的，获取权威帧
            for (int i = 0; i < FrameConst.MaxPredictionFrame; i++)
            {
                if (FrameBuffer.TryGetAuthorityFrameData(AuthorityFrame + i, out FrameData frame))
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
                FrameData authorityFrame = _authorityFrameQueue.Dequeue();
                
                //预测帧数据
                FrameData predictionFrame = _predictionFrameQueue.Dequeue();
                
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
                    MatchController.UpdateMatchEntity(AuthorityMatchEntity,authorityFrame);
                    MatchController.LogicUpdateState(AuthorityMatchEntity);
                    
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
                        FrameData prediction = FrameBuffer.GetPredictionFrameData(i);
                        _predictionFrameQueue.Enqueue(prediction);
                        Prediction(prediction);
                    }
                }
            }
        }

        protected override void RenderUpdate()
        {
            
        }
        
        /// <summary>
        /// 预测
        /// </summary>
        /// <param name="predictionFrame">预测帧</param>
        private void Prediction(FrameData predictionFrame)
        {
            MatchController.UpdateMatchEntity(PredictionMatchEntity,predictionFrame);
            MatchController.LogicUpdateState(PredictionMatchEntity);
            MatchEntity matchEntity = MatchEntity.Copy(PredictionMatchEntity);
            _predictionMatchEntityQueue.Enqueue(matchEntity);
        }
    }
}