using System.Diagnostics;
using System.Threading;

namespace Start
{
    public abstract class BattleFrameEngineBase : IBattleFrameEngine
    {
        /// <summary>
        /// 时间计数器
        /// </summary>
        protected FrameTimeCounter _timeCounter { get; private set; }
        
        public bool Running { get; private set;}
        
        /// <summary>
        /// 执行的时间
        /// </summary>
        protected Stopwatch _stopWatch;
        
        /// <summary>
        /// 逻辑线程
        /// </summary>
        private Thread _logicThread;
        
        /// <summary>
        /// 网络线程
        /// </summary>
        private Thread _netThread;
        
        /// <summary>
        /// 线程运行
        /// </summary>
        private bool _threadRunning;

        protected abstract void StartEngine(BattleData battleData);
        
        public void StartBattle(BattleData battleData)
        {
            StartEngine(battleData);
            _timeCounter = new FrameTimeCounter(0, 0, battleData.FrameInterval);
            if (battleData.BattleType == EBattleType.Remote)
            {
                _netThread = new Thread(NetworkEngineUpdate);
                _netThread.IsBackground = true;
                _netThread.Start();
            }
            _logicThread = new Thread(LogicEngineUpdate);
            _logicThread.IsBackground = true;
            _logicThread.Start();
            _stopWatch = new Stopwatch();
            _stopWatch.Start();
            _threadRunning = true;
            Running = true;
        }

        public void StopEngine()
        {
            _stopWatch.Stop();
            _stopWatch = null;
            _threadRunning = false;
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
            Running = false;
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
            while (_threadRunning)
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
            while (_threadRunning)
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