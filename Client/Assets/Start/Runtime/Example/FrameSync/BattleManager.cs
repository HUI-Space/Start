using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Start
{
    public class BattleManager : ManagerBase<BattleManager> ,IUpdateManager
    {
        public override int Priority => 999999999;

        public EBattleType BattleType { get; set; }
        
        private IFrameEngine _frameEngine;

        public override Task Initialize()
        {
            Application.quitting += OnApplicationQuit; 
            return base.Initialize();
        }
        
        private void OnApplicationQuit()
        {
            StopEngine();
        }
        
        public override Task DeInitialize()
        {
            Application.quitting -= OnApplicationQuit; // 
            return base.DeInitialize();
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            _frameEngine?.RenderEngineUpdate();
        }
        
        /// <summary>
        /// 启动战斗流程
        /// 单机流程：PrepareState(准备状态) -> LoadingState(加载状态) -> RunningState(战斗状态) -> EndState(结束状态)
        /// 远程流程：MatchingState（匹配状态） -> PrepareState(准备状态) -> LoadingState(加载状态) -> RunningState(战斗状态) -> EndState(结束状态)
        /// </summary>
        public async void StartState<T>() where T : AsyncFsmState<BattleManager>
        {
            switch (BattleType)
            {
                case EBattleType.Local:
                case EBattleType.Observer:
                    _frameEngine = new LocalFrameEngine();
                    break;
                case EBattleType.Remote:
                    _frameEngine = new RemoteFrameEngine();
                    break;
            }
            
            List<Type> types = AssemblyUtility.GetChildTypes(typeof(AsyncFsmState<BattleManager>));
            List<AsyncFsmState<BattleManager>> procedures = new List<AsyncFsmState<BattleManager>>();
            foreach (Type type in types)
            {
                AsyncFsmState<BattleManager> procedure = Activator.CreateInstance(type) as AsyncFsmState<BattleManager>;
                procedures.Add(procedure);
            }
            AsyncFsm<BattleManager> asyncFsm = await FsmManager.Instance.CreateFsm(nameof(BattleManager), this, procedures.ToArray());
            await asyncFsm.Start<T>();
        }
        
        public void StartEngine()
        {
            _frameEngine?.StartEngine();
        }
        
        public void StopEngine()
        {
            _frameEngine?.StopEngine();
            _frameEngine = null;
        }
        
        public void Pause()
        {
            _frameEngine?.Pause();
        }

        public void Resume()
        {
            _frameEngine?.Resume();
        }
    }
}