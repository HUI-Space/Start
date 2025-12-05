using System;
using System.Collections.Generic;

namespace Start
{
    public class SyncFsm<T> : FsmBase, IReusable, ISyncFsm<T> where T : class
    {
        private readonly Dictionary<Type, SyncFsmState<T>> _states = new Dictionary<Type, SyncFsmState<T>>();
        private Dictionary<string, IGenericData> _data = new Dictionary<string, IGenericData>();
        public string Name { get; private set; }

        public T Owner { get; private set; }

        public override bool IsDestroyed => _isDestroyed;

        public int FsmStateCount => _states.Count;

        public bool IsRunning => CurrentState != null;

        public Type NextState { get; private set; }

        public SyncFsmState<T> CurrentState { get; private set; }

        public float CurrentStateTime { get; private set; }

        public bool InTransition { get; private set; }

        private bool _isDestroyed;

        public static SyncFsm<T> Create(string name, T owner, params SyncFsmState<T>[] states)
        {
            if (owner == null)
            {
                throw new Exception("FSM 所有者为空.");
            }

            if (states == null || states.Length < 1)
            {
                throw new Exception("FSM 的状态为空.");
            }

            SyncFsm<T> syncFsm = RecyclableObjectPool.Acquire<SyncFsm<T>>();
            syncFsm.Name = name;
            syncFsm.Owner = owner;
            syncFsm._isDestroyed = false;
            foreach (SyncFsmState<T> state in states)
            {
                if (state == null)
                {
                    throw new Exception("FSM 的状态为空.");
                }

                Type stateType = state.GetType();
                if (syncFsm._states.ContainsKey(stateType))
                {
                    throw new Exception($"FSM '{name}' 状态 '{stateType.FullName}' 已经存在.");
                }

                syncFsm._states.Add(stateType, state);
                state.OnInitialize(syncFsm);
            }

            return syncFsm;
        }

        public void Start<TState>() where TState : SyncFsmState<T>
        {
            Start(typeof(TState));
        }

        public void Start(Type stateType)
        {
            if (IsRunning)
            {
                throw new Exception("FSM 正在运行中, 不能重新开始.");
            }

            if (stateType == null)
            {
                throw new Exception("状态类型为空.");
            }

            if (!typeof(SyncFsmState<T>).IsAssignableFrom(stateType))
            {
                throw new Exception($"状态类型不是继承自 SyncFsmState<T> 状态全称： '{stateType.FullName}'.");
            }

            SyncFsmState<T> state = GetState(stateType);
            CurrentStateTime = 0f;
            CurrentState = state ??
                           throw new Exception(
                               $"Fsm '{Name}' 不能切换到状态 '{stateType}' 因为它不存在.");
            CurrentState.OnEnter();
        }

        public bool HasState<TState>() where TState : SyncFsmState<T>
        {
            return HasState(typeof(TState));
        }

        public bool HasState(Type stateType)
        {
            return _states.ContainsKey(stateType);
        }

        public TState GetState<TState>() where TState : SyncFsmState<T>
        {
            if (_states.TryGetValue(typeof(TState), out var state))
            {
                return (TState)state;
            }

            return null;
        }

        public SyncFsmState<T> GetState(Type stateType)
        {
            if (stateType == null)
            {
                throw new Exception("状态类型为空.");
            }

            if (!typeof(SyncFsmState<T>).IsAssignableFrom(stateType))
            {
                throw new Exception($"状态类型不是继承自 SyncFsmState<T> 状态全称： '{stateType.FullName}'.");
            }

            SyncFsmState<T> state = null;
            if (_states.TryGetValue(stateType, out state))
            {
                return state;
            }

            return null;
        }

        public SyncFsmState<T>[] GetAllStates()
        {
            int index = 0;
            SyncFsmState<T>[] results = new SyncFsmState<T>[_states.Count];
            foreach (KeyValuePair<Type, SyncFsmState<T>> state in _states)
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
                RecyclableObjectPool.Recycle(oldData);
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
            RecyclableObjectPool.Recycle(this);
        }

        public void ChangeState<TState>() where TState : SyncFsmState<T>
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

        public void Reset()
        {
            foreach (KeyValuePair<Type, SyncFsmState<T>> state in _states)
            {
                state.Value.OnDeInitialize();
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

                    RecyclableObjectPool.Recycle(data.Value);
                }

                _data.Clear();
            }

            CurrentState = null;
            CurrentStateTime = 0f;
            _isDestroyed = true;
        }

        private void ReallyChangeState(Type stateType)
        {
            InTransition = true;
            if (CurrentState == null)
            {
                throw new Exception("当前状态机正在切换状态.");
            }

            SyncFsmState<T> state = GetState(stateType);
            if (state == null)
            {
                throw new Exception($"Fsm '{Name}' 不能切换到状态 '{stateType}' 因为它不存在.");
            }

            CurrentState.OnExit();
            CurrentStateTime = 0f;
            CurrentState = state;
            CurrentState.OnEnter();
            InTransition = false;
        }
    }
}