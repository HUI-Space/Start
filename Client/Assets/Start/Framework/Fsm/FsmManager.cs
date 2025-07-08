using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Start
{
    public class FsmManager:ManagerBase<FsmManager>,IUpdateManger
    {
        public override int Priority => 1;
        public int Count => _allFsm.Count;
        
        private Dictionary<Type, FsmBase> _allFsm;
        private List<FsmBase> _tempFsm;
        public override Task Initialize()
        {
            _allFsm = new Dictionary<Type, FsmBase>();
            _tempFsm = new List<FsmBase>();
            return Task.CompletedTask;
        }
        
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            _tempFsm.Clear();
            if (_allFsm.Count <= 0)
            {
                return;
            }

            foreach (KeyValuePair<Type, FsmBase> fsm in _allFsm)
            {
                _tempFsm.Add(fsm.Value);
            }

            foreach (FsmBase fsm in _tempFsm)
            {
                if (fsm.IsDestroyed)
                {
                    continue;
                }

                fsm.Update(elapseSeconds, realElapseSeconds);
            }
        }
        
        public bool HasFsm<T>() where T : class
        {
            return _allFsm.ContainsKey(typeof(T));
        }
        
        public IFsm<T> GetFsm<T>() where T : class
        {
            return (IFsm<T>)InternalGetFsm(typeof(T));
        }
        
        /// <summary>
        /// 创建同步状态机
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="owner">拥有者</param>
        /// <param name="states">所有状态</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public SyncFsm<T> CreateFsm<T>(string name, T owner, params SyncFsmState<T>[] states) where T : class
        {
            if (HasFsm<T>())
            {
                throw new Exception($"已经存在 FSM '{typeof(T)}'.");
            }

            SyncFsm<T> syncFsm = SyncFsm<T>.Create(name, owner, states);
            _allFsm.Add(typeof(T), syncFsm);
            return syncFsm;
        }
        
        /// <summary>
        /// 创建异步状态机
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="owner">拥有者</param>
        /// <param name="states">所有状态</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<AsyncFsm<T>> CreateFsm<T>(string name, T owner, params AsyncFsmState<T>[] states) where T : class
        {
            if (HasFsm<T>())
            {
                throw new Exception($"已经存在 FSM '{typeof(T)}'.");
            }

            AsyncFsm<T> asyncFsm = await AsyncFsm<T>.Create(name, owner, states);
            _allFsm.Add(typeof(T), asyncFsm);
            return asyncFsm;
        }
        
        public bool DestroyFsm<T>() where T : class
        {
            return InternalDestroyFsm(typeof(T));
        }
        
        public FsmBase[] GetAllFsm()
        {
            int index = 0;
            FsmBase[] results = new FsmBase[_allFsm.Count];
            foreach (KeyValuePair<Type, FsmBase> fsm in _allFsm)
            {
                results[index++] = fsm.Value;
            }

            return results;
        }
        
        private FsmBase InternalGetFsm(Type type)
        {
            if (_allFsm.TryGetValue(type, out FsmBase fsm))
            {
                return fsm;
            }

            return null;
        }

        private bool InternalDestroyFsm(Type type)
        {
            if (_allFsm.TryGetValue(type, out FsmBase fsm))
            {
                fsm.DeInitialize();
                return _allFsm.Remove(type);
            }

            return false;
        }
        public override Task DeInitialize()
        {
            foreach (KeyValuePair<Type, FsmBase> fsm in _allFsm)
            {
                fsm.Value.DeInitialize();
            }

            _allFsm.Clear();
            _tempFsm.Clear();
            _allFsm = default;
            _tempFsm = default;
            return base.DeInitialize();
        }
    }
}