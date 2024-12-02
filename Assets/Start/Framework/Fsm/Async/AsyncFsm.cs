using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Start.Framework
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

        public float CurrentStateTime { get; private set; }

        public bool InTransition { get; private set; }

        public static async Task<AsyncFsm<T>> Create(string name, T owner, params AsyncFsmState<T>[] states)
        {
            if (owner == null)
            {
                throw new Exception("FSM owner is invalid.");
            }

            if (states == null || states.Length < 1)
            {
                throw new Exception("FSM states is invalid.");
            }

            AsyncFsm<T> asyncFsm = ReferencePool.Acquire<AsyncFsm<T>>();
            asyncFsm.Name = name;
            asyncFsm.Owner = owner;
            asyncFsm._isDestroyed = false;
            foreach (AsyncFsmState<T> state in states)
            {
                if (state == null)
                {
                    throw new Exception("FSM states is invalid.");
                }

                Type stateType = state.GetType();
                if (asyncFsm._states.ContainsKey(stateType))
                {
                    throw new Exception($"FSM '{name}' state '{stateType.FullName}' is already exist.");
                }

                asyncFsm._states.Add(stateType, state);
                await state.OnInitialize(asyncFsm);
            }

            return asyncFsm;
        }

        public async Task Start<TState>() where TState : AsyncFsmState<T>
        {
            if (IsRunning)
            {
                throw new Exception("FSM is running, can not start again.");
            }

            AsyncFsmState<T> state = GetState<TState>();
            CurrentStateTime = 0f;
            CurrentState = state ??
                           throw new Exception(
                               $"AsyncFsm '{Name}' can not change state to '{typeof(TState)}' which is not exist.");
            await CurrentState.OnEnter();
        }

        public bool HasState<TState>() where TState : AsyncFsmState<T>
        {
            return _states.ContainsKey(typeof(TState));
        }

        public TState GetState<TState>() where TState : AsyncFsmState<T>
        {
            if (_states.TryGetValue(typeof(TState), out var state))
            {
                return (TState)state;
            }

            return default;
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
                throw new Exception("Data name is invalid.");
            }

            return _data != null && _data.ContainsKey(name);
        }

        public TData GetData<TData>(string name) where TData : class
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Data name is invalid.");
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
                throw new Exception("Data name is invalid.");
            }

            if (_data == null)
            {
                _data = new Dictionary<string, IGenericData>(StringComparer.Ordinal);
            }
            
            if (_data.ContainsKey(name))
            {
                throw new Exception("Data is Contains.");
            }
            
            IGenericData genericData = GenericData<TData>.Create(data);
            _data[name] = genericData;
        }

        public bool RemoveData(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Data name is invalid.");
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
        }

        internal override void DeInitialize()
        {
            ReferencePool.Release(this);
        }

        internal async Task ChangeState<TState>() where TState : AsyncFsmState<T>
        {
            InTransition = true;
            if (CurrentState == null)
            {
                throw new Exception("Current AsyncFsmState is invalid.");
            }

            AsyncFsmState<T> state = GetState<TState>();
            if (state == null)
            {
                throw new Exception(
                    $"AsyncFsm '{Name}' can not change state to '{typeof(TState)}' which is not exist.");
            }

            await CurrentState.OnExit();
            CurrentStateTime = 0f;
            CurrentState = state;
            await CurrentState.OnEnter();
            InTransition = false;
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
    }
}