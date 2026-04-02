using System;

namespace Start
{
    [Serializable]
    public struct FrameInput
    {
        /// <summary>
        /// 原始32位无符号整数，按位分段存储（位编号从0开始，最低位为第0位，最高位为第31位）：
        /// | 第31-28位 | 第27-20位 | 第19-12位 | 第11-4位  | 第0位  |
        /// |   Index  |    Yaw   |   Button  |  保留位   | GiveUp  |
        /// 各字段掩码与偏移计算：
        /// Index  : 4位 (0xF0000000) → 右移28位
        /// Yaw    : 8位 (0x0FF00000) → 右移20位
        /// Button : 8位 (0x000FF000) → 右移12位
        /// 保留位  : 11位(0x00000FF0) → 暂未使用
        /// GiveUp : 4位 (0x0000000F) → 右移0位
        /// 掩码二进制展开：
        /// 0xF0000000 → 1111 0000 0000 0000 0000 0000 0000 0000（Index）
        /// 0x0FF00000 → 0000 1111 1111 0000 0000 0000 0000 0000（Yaw）
        /// 0x000FF000 → 0000 0000 0000 1111 1111 0000 0000 0000（Button）
        /// 0x00000F00 → 0000 0000 0000 0000 0000 1111 0000 0000（保留位）
        /// 0x000000F0 → 0000 0000 0000 0000 0000 0000 1111 0000（保留位）
        /// 0x0000000F → 0000 0000 0000 0000 0000 0000 0000 1111（GiveUp）
        /// </summary>
        public uint Raw;

        /// <summary>
        /// 角色在玩家列表中的索引（对应Raw的第31-28位）
        /// get：提取Raw的高4位（掩码0xF0000000），右移28位转为byte；
        ///      若提取值为0xF（15），返回byte.MaxValue(255)表示无效索引；
        ///      有效索引范围：0~15（共15个有效位置）。
        /// set：先清除Raw的高4位（掩码0x0FFFFFFF），
        ///      再将输入值的低4位（掩码0xF）左移28位，合并到Raw的高4位。
        /// </summary>
        public byte Index
        {
            get
            {
                byte pos = (byte)((0xF0000000 & Raw) >> 28);
                if (pos == 0xF)
                {
                    return byte.MaxValue;
                }
                return pos;
            }

            set => Raw = Raw & 0x0FFFFFFF | (uint)((0xF & value) << 28);
        }

        /// <summary>
        /// 旋转角度（偏航角，对应Raw的第20~27位）
        /// get：提取Raw的第20~27位（掩码0x0FF00000），右移20位转为byte；
        /// set：清除Raw的第20~27位（掩码0xF00FFFFF），
        ///      再将输入值的低8位（掩码0xFF）左移20位，合并到Raw对应位段。
        /// 取值范围：0~255（共256个离散角度值）
        /// </summary>
        public byte Yaw
        {
            get => (byte)((0x0FF00000 & Raw) >> 20);
            set => Raw = (Raw & 0xF00FFFFF) | (uint)((0xFF & value) << 20);
        }

        /// <summary>
        /// 按钮状态（对应Raw的第12~19位）
        /// get：提取Raw的第12~19位（掩码0x000FF000），右移12位转为byte；
        /// set：清除Raw的第12~19位（掩码0xFFF00FFF），
        ///      再将输入值的低8位（掩码0xFF）左移12位，合并到Raw对应位段。
        /// 取值范围：0~255（每一位可表示一个按钮的按下/释放状态，具体按钮映射需业务层定义）
        /// </summary>
        public byte Button
        {
            get => (byte)((0x000FF000 & Raw) >> 12);
            set => Raw = (Raw & 0xFFF00FFF) | (uint)((0xFF & value) << 12);
        }
        
        /// <summary>
        /// 放弃相关按钮状态（对应Raw的第0~3位，共4位）
        /// get：提取Raw的低4位（掩码0x0000000F），转为byte；
        /// set：清除Raw的低4位（掩码0xFFFFFFF0），再将输入值的低4位合并到Raw对应位段；
        ///     取值范围：0~15（每一位可表示一个放弃相关按钮的按下/释放状态，具体映射需业务层定义）
        /// 若业务需要用 4 位表示多个放弃按钮（如第 0 位 = 放弃、第 1 位 = 确认放弃、第 2 位 = 取消放弃、第 3 位 = 等待 等）
        /// </summary>
        public byte GiveUp
        {
            get => (byte)(0x0000000F & Raw);
            set => Raw = (Raw & 0xFFFFFFF0) | (uint)(0xF & value);
        }

        /// <summary>
        /// 从原始32位无符号整数初始化FrameInput
        /// </summary>
        /// <param name="value">完整的32位原始值</param>
        public FrameInput(uint value)
        {
            Raw = value;
        }

        /// <summary>
        /// 仅初始化索引（Index）的FrameInput（其他字段默认0）
        /// </summary>
        /// <param name="pos">角色索引（0~14为有效，15为无效）</param>
        public FrameInput(byte pos)
        {
            Raw = (uint)((0xF & pos) << 28);
        }

        /// <summary>
        /// 初始化索引（Index）并复用已有原始值的其他位段
        /// </summary>
        /// <param name="pos">角色索引（0~14为有效，15为无效）</param>
        /// <param name="value">原始值（仅保留低28位，高4位被pos覆盖）</param>
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