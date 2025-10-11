using System.Threading;

namespace Start
{
    public abstract class BattleFrameEngineBase : IBattleFrameEngine
    {
        public FP FrameInterval { get; private set;}
        
        public FP TimeScale { get; private set;}
        
        public bool Running { get; private set;}
        
        /// <summary>
        /// 逻辑线程
        /// </summary>
        private Thread _logicThread;
        
        /// <summary>
        /// 网络线程
        /// </summary>
        private Thread _netThread;
        
        /// <summary>
        /// 线程停止
        /// </summary>
        private bool _threadStop;
        
        public void StartEngine(EBattleType battleType,BattleData battleData)
        {
            FrameInterval = new FP(FrameConst.FrameInterval) / new FP(1000);
            TimeScale = FP.One;
            if (battleType == EBattleType.Remote)
            {
                _netThread = new Thread(NetworkEngineUpdate);
                _netThread.IsBackground = true;
                _netThread.Start();
            }
            _logicThread = new Thread(LogicEngineUpdate);
            _logicThread.IsBackground = true;
            _logicThread.Start();
            Running = true;
        }

        public void StopEngine()
        {
            _threadStop = true;
            if (_logicThread != null)
            {
                _logicThread.Join();
                _logicThread = null;
            }
            if (_netThread != null)
            {
                _netThread.Join();
                _netThread = null;
            }
            _threadStop = false;
            Running = false;
            TimeScale = FP.One;
            FrameInterval = FP.Zero;
        }

        public void Pause()
        {
            Running = false;
        }

        public void Resume()
        {
            Running = true;
        }

        protected virtual void NetworkUpdate()
        {
            
        }
        
        protected abstract void LogicUpdate();
        
        protected abstract void RenderUpdate();
        
        
        public void RenderEngineUpdate()
        {
            if (Running)
            {
                RenderUpdate();
            }
        }

        private void NetworkEngineUpdate()
        {
            while (_threadStop)
            {
                if (Running)
                {
                    NetworkUpdate();
                }
                Thread.Sleep(1);
            }
        }
        
        private void LogicEngineUpdate()
        {
            while (_threadStop)
            {
                if (Running)
                {
                    LogicUpdate();
                }
                Thread.Sleep(1);
            }
        }
    }
}