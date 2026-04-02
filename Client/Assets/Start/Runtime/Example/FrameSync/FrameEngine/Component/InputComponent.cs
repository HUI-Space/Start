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
    /// 自动生成输入组件
    /// </summary>
    public class InputComponent : IComponent<InputComponent>
    {
        /// <summary>
        /// 玩家摇杆(0 ~ 24) 0 为不动
        /// </summary>
        public Byte Yaw
        {
            get => _common.Yaw;
            set => _common.Yaw = value;
        }

        /// <summary>
        /// 玩家按钮
        /// </summary>
        public Byte Button
        {
            get => _common.Button;
            set => _common.Button = value;
        }

        /// <summary>
        /// 玩家放弃
        /// </summary>
        public Byte GiveUp
        {
            get => _common.GiveUp;
            set => _common.GiveUp = value;
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Auto, Pack = 4)]
        private struct Common
        {
            public Byte Yaw;
            public Byte Button;
            public Byte GiveUp;

            public Common(int no)
            {
                Yaw = default;
                Button = default;
                GiveUp = default;
            }
        }

        private Common _common = new Common();

        public void CopyTo(InputComponent component)
        {
            component._common = _common;
        }

        public void SerializeToLog(StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine($"[InputComponent]");
            stringBuilder.AppendLine($"Yaw : {Yaw}");
            stringBuilder.AppendLine($"Button : {Button}");
            stringBuilder.AppendLine($"GiveUp : {GiveUp}");
        }
    }
}
