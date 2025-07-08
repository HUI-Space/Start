using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Start
{
    public class ExampleAsyncFsm
    {
        public ExampleAsyncFsmState CurrentState => (ExampleAsyncFsmState)_asyncFsm.CurrentState;
        private IAsyncFsm<ExampleAsyncFsm> _asyncFsm;
        
        public async Task Initialize()
        {
            List<Type> types = AssemblyUtility.GetChildType(typeof(ExampleAsyncFsmState));
            List<ExampleAsyncFsmState> asyncFsmStates = new List<ExampleAsyncFsmState>();
            foreach (Type type in types)
            {
                ExampleAsyncFsmState asyncFsmState = Activator.CreateInstance(type) as ExampleAsyncFsmState;
                asyncFsmStates.Add(asyncFsmState);
            }
            _asyncFsm = await FsmManager.Instance.CreateFsm(nameof(ExampleAsyncFsm), this, asyncFsmStates.ToArray());
        }
        
        /// <summary>
        /// 开始状态
        /// </summary>
        /// <typeparam name="T">要检查的状态类型。</typeparam>
        /// <exception cref="Exception"></exception>
        public async Task StartState<T>() where T : ExampleAsyncFsmState
        {
            if (_asyncFsm == null)
            {
                throw new Exception("You must initialize exampleAsyncFsm first.");
            }

            if (_asyncFsm.IsRunning)
            {
                throw new Exception("ExampleAsyncFsm is running.");
            }
            
            await _asyncFsm.Start<T>();
        }

        public void ChangeState(Type stateType)
        {
            if (_asyncFsm == null)
            {
                throw new Exception("You must initialize exampleAsyncFsm first.");
            }
            
            if (!_asyncFsm.IsRunning)
            {
                throw new Exception("ExampleAsyncFsm not running.");
            }
            
            _asyncFsm.ChangeState(stateType);
        }
        
        public ExampleAsyncFsmState[] GetAllStates()
        {
            if (_asyncFsm == null)
            {
                throw new Exception("You must initialize exampleSyncFsm first.");
            }
            AsyncFsmState<ExampleAsyncFsm>[] asyncFsmStates = _asyncFsm.GetAllStates();
            ExampleAsyncFsmState[] states = new ExampleAsyncFsmState[asyncFsmStates.Length];
            for (int i = 0; i < asyncFsmStates.Length; i++)
            {
                states[i] = (ExampleAsyncFsmState)asyncFsmStates[i];
            }
            return states;
        }
    }
}