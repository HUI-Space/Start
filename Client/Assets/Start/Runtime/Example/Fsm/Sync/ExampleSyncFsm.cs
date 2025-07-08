using System;
using System.Collections.Generic;

namespace Start
{
    public class ExampleSyncFsm
    {
        public ExampleSyncFsmState CurrentState => (ExampleSyncFsmState)_syncFsm.CurrentState;
        private ISyncFsm<ExampleSyncFsm> _syncFsm;
        
        public void Initialize()
        {
            List<Type> types = AssemblyUtility.GetChildType(typeof(ExampleSyncFsmState));
            List<ExampleSyncFsmState> syncFsmStates = new List<ExampleSyncFsmState>();
            foreach (Type type in types)
            {
                ExampleSyncFsmState syncFsmState = Activator.CreateInstance(type) as ExampleSyncFsmState;
                syncFsmStates.Add(syncFsmState);
            }
            _syncFsm = FsmManager.Instance.CreateFsm(nameof(ExampleSyncFsm), this, syncFsmStates.ToArray());
        }
        
        /// <summary>
        /// 开始状态
        /// </summary>
        /// <typeparam name="T">要检查的状态类型。</typeparam>
        /// <exception cref="Exception"></exception>
        public void StartState<T>() where T : ExampleSyncFsmState
        {
            if (_syncFsm == null)
            {
                throw new Exception("You must initialize exampleSyncFsm first.");
            }
            _syncFsm.Start<T>();
        }

        public void ChangeState(Type stateType)  
        {
            if (_syncFsm == null)
            {
                throw new Exception("You must initialize exampleAsyncFsm first.");
            }
            
            if (!_syncFsm.IsRunning)
            {
                throw new Exception("ExampleAsyncFsm not running.");
            }
            
            _syncFsm.ChangeState(stateType);
        }
        

        /// <summary>
        /// 是否存在状态。
        /// </summary>
        /// <typeparam name="T">要检查的状态类型。</typeparam>
        /// <returns>是否存在状态。</returns>
        public bool HasState<T>() where T : ExampleSyncFsmState
        {
            if (_syncFsm == null)
            {
                throw new Exception("You must initialize exampleSyncFsm first.");
            }

            return _syncFsm.HasState<T>();
        }

        /// <summary>
        /// 获取状态。
        /// </summary>
        /// <typeparam name="T">要获取的状态类型。</typeparam>
        /// <returns>要获取的状态。</returns>
        public ExampleSyncFsmState GetState<T>() where T : ExampleSyncFsmState
        {
            if (_syncFsm == null)
            {
                throw new Exception("You must initialize exampleSyncFsm first.");
            }

            return _syncFsm.GetState<T>();
        }

        public ExampleSyncFsmState[] GetAllStates()
        {
            if (_syncFsm == null)
            {
                throw new Exception("You must initialize exampleSyncFsm first.");
            }

            SyncFsmState<ExampleSyncFsm>[] syncFsmStates = _syncFsm.GetAllStates();
            ExampleSyncFsmState[] states = new ExampleSyncFsmState[syncFsmStates.Length];
            for (int i = 0; i < syncFsmStates.Length; i++)
            {
                states[i] = (ExampleSyncFsmState)syncFsmStates[i];
            }
            return states;
        }
    }
}