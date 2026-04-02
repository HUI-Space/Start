using System.Diagnostics;
using System.Threading;

namespace Start
{
    public abstract class FrameEngineBase : IFrameEngine
    {
        public bool Running { get; private set;}
        
        /// <summary>
        /// 时间计数器
        /// </summary>
        protected FrameTimeCounter _timeCounter { get; private set; }
        
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

        protected FrameEngineBase()
        {
            _timeCounter = new FrameTimeCounter(0, 0, FrameConst.FrameInterval);
            if (BattleManager.Instance.BattleType == EBattleType.Remote)
            {
                _netThread = new Thread(NetworkEngineUpdate);
                _netThread.IsBackground = true;
            }
            _logicThread = new Thread(LogicEngineUpdate);
            _logicThread.IsBackground = true;
            _stopWatch = new Stopwatch();
        }
        
        public void StartEngine()
        {
            _netThread?.Start();
            _logicThread.Start();
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
            _stopWatch.Stop();
            Running = false;
        }

        public void Resume()
        {
            _stopWatch.Start();
            Running = true;
        }

        public void RenderEngineUpdate()
        {
            if (Running)
            {
                RenderUpdate();
            }
        }
        
        protected virtual void NetworkUpdate()
        {
            
        }
        
        protected abstract void LogicUpdate();
        
        protected abstract void RenderUpdate();

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