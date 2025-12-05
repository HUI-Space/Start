using System;
using System.Collections.Generic;

namespace Start
{
    /// <summary>
    /// 动态生成的组件类: 状态组件
    /// </summary>
    public class StateComponent : IComponent<StateComponent>
    {
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Auto, Pack = 4)]
        private struct Common
        {
            public int CurrentState;
            public int NextState;
            public int PrevState;
            public int ForceStateId;
            public FP EnterTime;
            public FP EndTime;
            public Common(int no)
            {
                CurrentState = default;
                NextState = default;
                PrevState = default;
                ForceStateId = default;
                EnterTime = default;
                EndTime = default;
            }
        }

        private Common _common = new Common();

        public int CurrentState
        {
            get => _common.CurrentState;
            set => _common.CurrentState = value;
        }

        public int NextState
        {
            get => _common.NextState;
            set => _common.NextState = value;
        }

        public int PrevState
        {
            get => _common.PrevState;
            set => _common.PrevState = value;
        }

        public int ForceState
        {
            get => _common.ForceStateId;
            set => _common.ForceStateId = value;
        }

        public FP EnterTime
        {
            get => _common.EnterTime;
            set => _common.EnterTime = value;
        }

        public FP EndTime
        {
            get => _common.EndTime;
            set => _common.EndTime = value;
        }


        public void CopyTo(StateComponent component)
        {
            component._common = _common;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
