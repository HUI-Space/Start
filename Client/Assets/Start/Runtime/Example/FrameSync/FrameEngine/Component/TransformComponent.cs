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
    /// 自动生成坐标组件
    /// </summary>
    public class TransformComponent : IComponent<TransformComponent>
    {
        /// <summary>
        /// 坐标
        /// </summary>
        public TSVector Position
        {
            get => _common.Position;
            set => _common.Position = value;
        }

        /// <summary>
        /// 旋转
        /// </summary>
        public TSQuaternion Rotation
        {
            get => _common.Rotation;
            set => _common.Rotation = value;
        }

        /// <summary>
        /// 缩放
        /// </summary>
        public TSVector Scale
        {
            get => _common.Scale;
            set => _common.Scale = value;
        }

        /// <summary>
        /// 前向向量
        /// </summary>
        public TSVector Forward
        {
            get => _common.Forward;
            set => _common.Forward = value;
        }

        /// <summary>
        /// 偏移量
        /// </summary>
        public TSVector MoveDelta
        {
            get => _common.MoveDelta;
            set => _common.MoveDelta = value;
        }

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Auto, Pack = 4)]
        private struct Common
        {
            public TSVector Position;
            public TSQuaternion Rotation;
            public TSVector Scale;
            public TSVector Forward;
            public TSVector MoveDelta;

            public Common(int no)
            {
                Position = default;
                Rotation = default;
                Scale = default;
                Forward = default;
                MoveDelta = default;
            }
        }

        private Common _common = new Common();

        public void CopyTo(TransformComponent component)
        {
            component._common = _common;
        }

        public void SerializeToLog(StringBuilder stringBuilder)
        {
            stringBuilder.AppendLine($"[TransformComponent]");
            stringBuilder.AppendLine($"Position : {Position}");
            stringBuilder.AppendLine($"Rotation : {Rotation}");
            stringBuilder.AppendLine($"Scale : {Scale}");
            stringBuilder.AppendLine($"Forward : {Forward}");
            stringBuilder.AppendLine($"MoveDelta : {MoveDelta}");
        }
    }
}
