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
    /// 自动生成移动组件
    /// </summary>
    public class MoveComponent : IComponent<MoveComponent>
    {
        /// <summary>
        /// 移动方向
        /// </summary>
        public TSVector Direction
        {
            get => _common.Direction;
            set => _common.Direction = value;
        }

        /// <summary>
        /// 移动速度
        /// </summary>
        public FP Speed
        {
            get => _common.Speed;
            set => _common.Speed = value;
        }

        /// <summary>
        /// 上一次移动方向
        /// </summary>
        public TSVector LastDirection
        {
            get => _common.LastDirection;
            set => _common.LastDirection = value;
        }

        /// <summary>
        /// 上一次移动速度
        /// </summary>
        public FP LastSpeed
        {
            get => _common.LastSpeed;
            set => _common.LastSpeed = value;
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Auto, Pack = 4)]
        private struct Common
        {
            public TSVector Direction;
            public FP Speed;
            public TSVector LastDirection;
            public FP LastSpeed;

            public Common(int no)
            {
                Direction = default;
                Speed = default;
                LastDirection = default;
                LastSpeed = default;
            }
        }

        private Common _common = new Common();

        public void CopyTo(MoveComponent component)
        {
            component._common = _common;
        }

        public void SerializeToLog(StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine($"[MoveComponent]");
            stringBuilder.AppendLine($"Direction : {Direction}");
            stringBuilder.AppendLine($"Speed : {Speed}");
            stringBuilder.AppendLine($"LastDirection : {LastDirection}");
            stringBuilder.AppendLine($"LastSpeed : {LastSpeed}");
        }
    }
}
