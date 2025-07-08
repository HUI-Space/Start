using System;
using System.Threading;

namespace Start
{
    public class FrameEngine
    {
        /// <summary>
        /// 帧间隔
        /// </summary>
        public static FixedPointNumber FrameInterval { get; private set; }

        /// <summary>
        /// 时间缩放
        /// </summary>
        public static FixedPointNumber TimeScale { get; private set; }
        
        /// <summary>
        /// 运行中
        /// </summary>
        public static bool Running { get; private set; }
        
        /// <summary>
        /// 逻辑线程
        /// </summary>
        private static Thread _logicThread;
        
        /// <summary>
        /// 网络线程
        /// </summary>
        private static Thread _netThread;
        
        /// <summary>
        /// 线程停止
        /// </summary>
        private static bool _threadStop;
        
        /// <summary>
        /// 逻辑更新
        /// </summary>
        private static Action _onLogicUpdate;
        
        /// <summary>
        /// 网络更新
        /// </summary>
        private static Action _onNetworkUpdate;
        
        /// <summary>
        /// 网络更新
        /// </summary>
        private static Action _onRenderUpdate;

        /// <summary>
        /// 本地战斗不需要网络
        /// </summary>
        /// <param name="frameInterval"></param>
        /// <param name="logicUpdate"></param>
        /// <param name="renderUpdate"></param>
        public static void StartEngine(FixedPointNumber frameInterval,Action logicUpdate,Action renderUpdate)
        {
            FrameInterval = frameInterval;
            TimeScale = FixedPointNumber.One;
            Running = true;
            _onLogicUpdate = logicUpdate;
            _onRenderUpdate = renderUpdate;
            _logicThread = new Thread(LogicUpdate);
            _logicThread.IsBackground = true;
            _logicThread.Start();
        }
        
        /// <summary>
        /// 远程战斗不需要网络
        /// </summary>
        /// <param name="frameInterval"></param>
        /// <param name="logicUpdate"></param>
        /// <param name="networkUpdate"></param>
        /// <param name="renderUpdate"></param>
        public static void StartEngine(FixedPointNumber frameInterval,Action logicUpdate,Action networkUpdate,Action renderUpdate)
        {
            FrameInterval = frameInterval;
            TimeScale = FixedPointNumber.One;
            Running = true;
            _onLogicUpdate = logicUpdate;
            _onNetworkUpdate = networkUpdate;
            _onRenderUpdate = renderUpdate;
            _logicThread = new Thread(LogicUpdate);
            _netThread = new Thread(NetworkUpdate);
            _logicThread.IsBackground = true;
            _netThread.IsBackground = true;
            _logicThread.Start();
            _netThread.Start();
        }

        public static void StopEngine()
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
            _onLogicUpdate = default;
            _onNetworkUpdate = default;
            _onRenderUpdate = default;
            Running = false;
            TimeScale = FixedPointNumber.One;
            FrameInterval = FixedPointNumber.Zero;
        }

        public static void Pause()
        {
            Running = false;
        }

        public static void Resume()
        {
            Running = true;
        }
        
        public static void RenderUpdate()
        {
            if (Running)
            {
                _onRenderUpdate?.Invoke();
            }
        }
        
        private static void LogicUpdate()
        {
            while (_threadStop)
            {
                if (Running)
                {
                    _onLogicUpdate?.Invoke();
                }
                Thread.Sleep(1);
            }
        }

        private static void NetworkUpdate()
        {
            while (_threadStop)
            {
                if (Running)
                {
                    _onNetworkUpdate?.Invoke();
                }
                Thread.Sleep(1);
            }
        }
    }
}