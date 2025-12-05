using System;

namespace Start
{
    [Serializable]
    public struct FrameInput
    {
        /// <summary>
        /// 原始数据
        /// uint:32位 32 bit
        /// | 0 0 0 0 | 0 0 0 0 | 0 0 0 0 | 0 0 0 0 | 0 0 0 0 | 0 0 0 0 | 0 0 0 0 | 0 0 0 0 |
        /// |   Pos   |        Yaw        |      Button       | 1 1 1 1 | 1 1 1 1 | 1 1 1 1 |
        /// Position    : 4 bit : (read & 0xF0000000) >> 28
        /// Yaw         : 8 bit : (read & 0x00FF0000) >> 20
        /// Button      : 8 bit : (read & 0x0000FF00) >> 12
        /// 
        /// </summary>
        public uint Raw;

        /// <summary>
        /// 角色在玩家列表中的位置
        /// get：使用掩码 0xF0000000 与 Raw 进行按位与操作，
        ///      保留 Raw 的高4位。将结果右移28位，
        ///      使这4位移动到最低的4位位置，得到一个 byte 类型的值 v
        /// 
        /// set：使用 0x0FFFFFFF与 Raw 进行按位与操作，清除 Raw 的高4位
        ///      将传入的 value 与 0xF 进行按位与操作，确保只保留低4位
        ///      将这4位左移28位，使其位于 Raw 的高4位位置。
        ///      将处理后的结果与清除高4位后的 Raw 进行按位或操作，最终更新 Raw
        ///
        ///      因为0xF = 15 所以 0x0 到 0xF 即 0-15 总共16个数字，
        ///      如果Pos = 0xF 则表示Pos为无效值，可以保留15个位置
        ///      返回255则位无效值
        /// </summary>
        public byte Index
        {
            get
            {
                byte pos = (byte)((0xF0000000 & Raw) >>  28);
                if (pos == 0xF)
                {
                    return byte.MaxValue;
                }
                return pos;
            }

            set => Raw = Raw & 0x0FFFFFFF | (uint)((0xF & value) << 28);
        }

        
        /// <summary>
        /// 表示旋转角度（偏航角）
        /// 获取时：从 Raw 中提取第20到27位的值，并返回为字节类型。
        /// 设置时：将新值的低8位左移20位后，与 Raw 的其他位合并，更新 Raw 的第20到27位。
        /// Yaw 使用了8位（即1个字节）来表示数值，因此可以表示的数字范围是从0到255，总共 256 个不同的值
        /// </summary>
        public byte Yaw
        {
            get => (byte)((0x00FF0000 & Raw) >> 20);
            set => Raw = Raw & 0xF00FFFFF | (uint)((0xFF & value) << 20);
        }
        
        /// <summary>
        /// 表示按钮
        /// 获取时：从 Raw 中提取第12到19位的值，并返回为字节类型。
        /// 设置时：将新值的低8位左移12位后，与 Raw 的其他位合并，更新 Raw 的第12到19位。
        /// Yaw 使用了8位（即1个字节）来表示数值，因此可以表示的数字范围是从0到255，总共 256 个不同的值
        /// </summary>
        public byte Button
        {
            get => (byte)((0x0000FF00 & Raw) >> 12);
            set => Raw = Raw & 0xFFFF00FF | (uint)((0xFF & value) << 12);
        }
        
        public FrameInput(uint value)
        {
            Raw = value;
        }
        
        public FrameInput(byte pos)
        {
            Raw = (uint)((0xF & pos) << 28);
        }

        public FrameInput(byte pos, uint value)
        {
            Raw = value & 0x0FFFFFFF | (uint)((0xF & pos) << 28);
        }

        public override string ToString()
        {
            return $"Index:{Index} Yaw:{Yaw} Button:{Button}";
        }
    }
}