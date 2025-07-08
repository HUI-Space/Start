using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Start
{
    public class ProcedureManager : ManagerBase<ProcedureManager>
    {
        public override int Priority => 17;

        public ProcedureBase CurrentProcedure => (ProcedureBase)_asyncFsm.CurrentState;
        private IAsyncFsm<ProcedureManager> _asyncFsm;

        public override async Task Initialize()
        {
            List<Type> types = AssemblyUtility.GetChildType(typeof(ProcedureBase));
            List<ProcedureBase> procedures = new List<ProcedureBase>();
            foreach (Type type in types)
            {
                ProcedureBase procedure = Activator.CreateInstance(type) as ProcedureBase;
                procedures.Add(procedure);
            }
            _asyncFsm = await FsmManager.Instance.CreateFsm("ProcedureManager", this, procedures.ToArray());
        }

        public async Task StartProcedure<T>() where T : ProcedureBase
        {
            if (_asyncFsm == null)
            {
                throw new Exception("你必须首先初始化流程.");
            }
            await _asyncFsm.Start<T>();
        }

        /// <summary>
        /// 是否存在流程。
        /// </summary>
        /// <typeparam name="T">要检查的流程类型。</typeparam>
        /// <returns>是否存在流程。</returns>
        public bool HasProcedure<T>() where T : ProcedureBase
        {
            if (_asyncFsm == null)
            {
                throw new Exception("你必须首先初始化流程.");
            }

            return _asyncFsm.HasState<T>();
        }

        /// <summary>
        /// 获取流程。
        /// </summary>
        /// <typeparam name="T">要获取的流程类型。</typeparam>
        /// <returns>要获取的流程。</returns>
        public ProcedureBase GetProcedure<T>() where T : ProcedureBase
        {
            if (_asyncFsm == null)
            {
                throw new Exception("你必须首先初始化流程.");
            }

            return _asyncFsm.GetState<T>();
        }
    }
}