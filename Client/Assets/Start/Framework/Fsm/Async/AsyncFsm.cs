using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Start
{
    public class AsyncFsm<T> : FsmBase, IReference, IAsyncFsm<T> where T : class
    {
        private readonly Dictionary<Type, AsyncFsmState<T>> _states = new Dictionary<Type, AsyncFsmState<T>>();
        private Dictionary<string, IGenericData> _data = new Dictionary<string, IGenericData>();
        private bool _isDestroyed;
        public string Name { get; private set; }

        public T Owner { get; private set; }

        public override bool IsDestroyed => _isDestroyed;

        public int FsmStateCount => _states.Count;

        public bool IsRunning => CurrentState != null;

        public AsyncFsmState<T> CurrentState { get; private set; }

        public Type NextState { get; private set; }
        
        public float CurrentStateTime { get; private set; }

        public bool InTransition { get; private set; }

        public static async Task<AsyncFsm<T>> Create(string name, T owner, params AsyncFsmState<T>[] states)
        {
            if (owner == null)
            {
                throw new Exception("FSM 所有者为空.");
            }

            if (states == null || states.Length < 1)
            {
                throw new Exception("FSM 的状态为空.");
            }

            AsyncFsm<T> asyncFsm = ReferencePool.Acquire<AsyncFsm<T>>();
            asyncFsm.Name = name;
            asyncFsm.Owner = owner;
            asyncFsm._isDestroyed = false;
            foreach (AsyncFsmState<T> state in states)
            {
                if (state == null)
                {
                    throw new Exception("FSM 的状态为空.");
                }

                Type stateType = state.GetType();
                if (!asyncFsm._states.TryAdd(stateType, state))
                {
                    throw new Exception($"FSM '{name}' 状态 '{stateType.FullName}' 已经存在.");
                }

                await state.OnInitialize(asyncFsm);
            }

            return asyncFsm;
        }

        public async Task Start<TState>() where TState : AsyncFsmState<T>
        {
            await Start(typeof(TState));
        }

        public async Task Start(Type stateType)
        {
            if (IsRunning)
            {
                throw new Exception("FSM 正在运行中, 不能重新开始.");
            }

            if (stateType == null)
            {
                throw new Exception("状态类型为空.");
            }

            if (!typeof(AsyncFsmState<T>).IsAssignableFrom(stateType))
            {
                throw new Exception($"状态类型不是继承自 AsyncFsmState<T> 状态全称：'{stateType.FullName}'.");
            }

            AsyncFsmState<T> state = GetState(stateType);
            CurrentStateTime = 0f;
            CurrentState = state ??
                           throw new Exception(
                               $"AsyncFsm '{Name}' 不能切换到状态： '{stateType}' 因为它不存在.");
            await CurrentState.OnEnter();
        }
        
        public bool HasState<TState>() where TState : AsyncFsmState<T>
        {
            return HasState(typeof(TState));
        }

        public bool HasState(Type stateType)
        {
            return _states.ContainsKey(stateType);
        }
        
        public TState GetState<TState>() where TState : AsyncFsmState<T>
        {
            if (_states.TryGetValue(typeof(TState), out var state))
            {
                return (TState)state;
            }

            return null;
        }

        public AsyncFsmState<T> GetState(Type stateType)
        {
            if (stateType == null)
            {
                throw new Exception("状态类型为空.");
            }

            if (!typeof(AsyncFsmState<T>).IsAssignableFrom(stateType))
            {
                throw new Exception($"状态类型不是继承自 AsyncFsmState<T> 状态全称： '{stateType.FullName}'.");
            }

            if (_states.TryGetValue(stateType, out var state))
            {
                return state;
            }

            return null;
        }

        public AsyncFsmState<T>[] GetAllStates()
        {
            int index = 0;
            AsyncFsmState<T>[] results = new AsyncFsmState<T>[_states.Count];
            foreach (KeyValuePair<Type, AsyncFsmState<T>> state in _states)
            {
                results[index++] = state.Value;
            }

            return results;
        }

        public bool HasData(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Data 名称为空.");
            }

            return _data != null && _data.ContainsKey(name);
        }

        public TData GetData<TData>(string name) where TData : class
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Data 名称为空.");
            }

            if (_data == null)
            {
                return default;
            }

            if (_data.TryGetValue(name, out IGenericData data))
            {
                return data.GetData1<TData>();
            }
            
            return default;
        }
        
        public void SetData<TData>(string name, TData data) where TData : class
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Data 名称为空.");
            }

            if (_data == null)
            {
                _data = new Dictionary<string, IGenericData>(StringComparer.Ordinal);
            }
            
            if (_data.ContainsKey(name))
            {
                throw new Exception("Data 已存在.");
            }
            
            IGenericData genericData = GenericData<TData>.Create(data);
            _data[name] = genericData;
        }

        public bool RemoveData(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Data 名称为空.");
            }

            if (_data == null)
            {
                return false;
            }

            IGenericData oldData = GetData<IGenericData>(name);
            _data.Remove(name);
            if (oldData != null)
            {
                ReferencePool.Release(oldData);
            }

            return true;
        }

        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (CurrentState == null || InTransition)
            {
                return;
            }

            CurrentStateTime += elapseSeconds;
            CurrentState.OnUpdate(elapseSeconds, realElapseSeconds);

            // 执行状态决策（分离关注点）
            if (NextState != null && NextState != CurrentState.GetType())
            {
                Type _nextState = NextState;
                NextState = null;
                ReallyChangeState(_nextState);
            }
        }
        
        internal override void DeInitialize()
        {
            ReferencePool.Release(this);
        }
        
        public async void Clear()
        {
            foreach (KeyValuePair<Type, AsyncFsmState<T>> state in _states)
            {
                await state.Value.OnDeInitialize();
            }

            Name = null;
            Owner = null;
            _states.Clear();
            if (_data != null)
            {
                foreach (KeyValuePair<string, IGenericData> data in _data)
                {
                    if (data.Value == null)
                    {
                        continue;
                    }

                    ReferencePool.Release(data.Value);
                }

                _data.Clear();
            }

            CurrentState = null;
            CurrentStateTime = 0f;
            _isDestroyed = true;
        }


        public void ChangeState<TState>() where TState : AsyncFsmState<T>
        {
            ChangeState(typeof(TState));
        }
        
        public void ChangeState(Type nextStateType)
        {
            if (nextStateType != null && CurrentState.GetType() != nextStateType && _states.ContainsKey(nextStateType))
            {
                NextState = nextStateType;
            }
        }
        
        private async void ReallyChangeState(Type stateType)
        {
            if (InTransition)
            {
                throw new Exception("当前状态机正在切换状态.");
            }
            
            InTransition = true;
            if (CurrentState == null)
            {
                throw new Exception("当前状态为空.");
            }
            AsyncFsmState<T> state = GetState(stateType);
            if (state == null)
            {
                throw new Exception(
                    $"AsyncFsm '{Name}' 不能切换到状态 '{stateType}' 因为它不存在.");
            }

            await CurrentState.OnExit();
            CurrentStateTime = 0f;
            CurrentState = state;
            await CurrentState.OnEnter();
            InTransition = false;
        }
    }
}