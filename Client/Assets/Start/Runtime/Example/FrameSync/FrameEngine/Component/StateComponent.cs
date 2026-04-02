/*-----------------------------------------------------------------
    以下代码为自动生成的代码，任何对以下代码的修改将在下次自动生成后被覆
    盖，不要对以下代码进行修改
-----------------------------------------------------------------*/

using System;
using System.Text;
using System.Collections.Generic;
namespace Start
{
    /// <summary>
    /// 自动生成状态组件
    /// </summary>
    public class StateComponent : IComponent<StateComponent>
    {
        /// <summary>
        /// 当前状态
        /// </summary>
        public Int32 CurrentState
        {
            get => _common.CurrentState;
            set => _common.CurrentState = value;
        }

        /// <summary>
        /// 下一状态
        /// </summary>
        public Int32 NextState
        {
            get => _common.NextState;
            set => _common.NextState = value;
        }

        /// <summary>
        /// 上一状态
        /// </summary>
        public Int32 PrevState
        {
            get => _common.PrevState;
            set => _common.PrevState = value;
        }

        /// <summary>
        /// 强制状态
        /// </summary>
        public Int32 ForceState
        {
            get => _common.ForceState;
            set => _common.ForceState = value;
        }

        /// <summary>
        /// 状态开始时间
        /// </summary>
        public FP EnterTime
        {
            get => _common.EnterTime;
            set => _common.EnterTime = value;
        }

        /// <summary>
        /// 状态结束时间
        /// </summary>
        public FP EndTime
        {
            get => _common.EndTime;
            set => _common.EndTime = value;
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Auto, Pack = 4)]
        private struct Common
        {
            public Int32 CurrentState;
            public Int32 NextState;
            public Int32 PrevState;
            public Int32 ForceState;
            public FP EnterTime;
            public FP EndTime;

            public Common(int no)
            {
                CurrentState = default;
                NextState = default;
                PrevState = default;
                ForceState = default;
                EnterTime = default;
                EndTime = default;
            }
        }

        private Common _common = new Common();

        public void CopyTo(StateComponent component)
        {
            component._common = _common;
        }

        public void SerializeToLog(StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine($"[StateComponent]");
            stringBuilder.AppendLine($"CurrentState : {CurrentState}");
            stringBuilder.AppendLine($"NextState : {NextState}");
            stringBuilder.AppendLine($"PrevState : {PrevState}");
            stringBuilder.AppendLine($"ForceState : {ForceState}");
            stringBuilder.AppendLine($"EnterTime : {EnterTime}");
            stringBuilder.AppendLine($"EndTime : {EndTime}");
        }
    }
}
