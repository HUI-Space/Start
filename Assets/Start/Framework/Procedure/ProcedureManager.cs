using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Start.Framework
{
    [Manager]
    public class ProcedureManager:ManagerBase<ProcedureManager>
    {
        public override int Priority => 17;

        public ProcedureBase CurrentProcedure => (ProcedureBase)_fsm.CurrentState;
        private IAsyncFsm<ProcedureManager> _fsm;

        public override async Task Initialize()
        {
            Type[] types = AssemblyUtility.GetTypes();
            List<ProcedureBase> procedures = new List<ProcedureBase>();
            foreach (Type type in types)
            {
                if (type.BaseType == typeof(ProcedureBase))
                {
                    ProcedureBase procedure = Activator.CreateInstance(type) as ProcedureBase;
                    procedures.Add(procedure);
                }
            }
            _fsm = await FsmManager.Instance.CreateFsm("ProcedureManager", this, procedures.ToArray());
        }

        public async Task StartProcedure<T>() where T : ProcedureBase
        {
            if (_fsm == null)
            {
                throw new Exception("You must initialize procedure first.");
            }
            await _fsm.Start<T>();
        }
        
        /// <summary>
        /// 是否存在流程。
        /// </summary>
        /// <typeparam name="T">要检查的流程类型。</typeparam>
        /// <returns>是否存在流程。</returns>
        public bool HasProcedure<T>() where T : ProcedureBase
        {
            if (_fsm == null)
            {
                throw new Exception("You must initialize procedure first.");
            }

            return _fsm.HasState<T>();
        }
        
        /// <summary>
        /// 获取流程。
        /// </summary>
        /// <typeparam name="T">要获取的流程类型。</typeparam>
        /// <returns>要获取的流程。</returns>
        public ProcedureBase GetProcedure<T>() where T : ProcedureBase
        {
            if (_fsm == null)
            {
                throw new Exception("You must initialize procedure first.");
            }

            return _fsm.GetState<T>();
        }
    }
}