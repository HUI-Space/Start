using System;

namespace Start.FrameSync
{
    /// <summary>
    /// 具体的逻辑帧数据
    /// 标记了该帧的序号、玩家数以及每个玩家的操作
    /// </summary>
    [Serializable]
    public struct FrameData : IEquatable<FrameData>
    {
        /// <summary>
        /// 权威帧
        /// </summary>
        public int AuthorityFrame { get; private set; }

        /// <summary>
        /// 帧输入
        /// </summary>
        public FrameInput[] FrameInputs { get; private set; }

        public FrameData(int authorityFrame,int count)
        {
            AuthorityFrame = authorityFrame;
            FrameInputs = new FrameInput[count];
            for (int i = 0; i < count; i++)
            {
                FrameInputs[i] = new FrameInput((uint)i);
            }
        }
        
        public FrameData(int authorityFrame, FrameInput[] frameInputs)
        {
            AuthorityFrame = authorityFrame;
            FrameInputs = frameInputs;
        }
        
        public FrameInput this[int index] 
        {
            get
            {
                if (index < FrameInputs.Length)
                {
                    return FrameInputs[index];
                }
                return new FrameInput();
            }
            set
            {
                if (index < FrameInputs.Length)
                {
                    FrameInputs[index] = value;
                }
            }
        }

        public void SetInputByIndex(int index, FrameInput frameInput)
        {
            if (index < FrameInputs.Length)
            {
                FrameInputs[index] = frameInput;
            }
        }

        public bool GetInputByIndex(int index,out FrameInput frameInput)
        {
            frameInput = default;
            if (index < FrameInputs.Length)
            {
                frameInput = FrameInputs[index];
                return true;
            }
            return false;
        }

        public bool Equals(FrameData other)
        {
            return AuthorityFrame == other.AuthorityFrame && Equals(FrameInputs, other.FrameInputs);
        }

        public override bool Equals(object obj)
        {
            return obj is FrameData other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (AuthorityFrame * 397) ^ (FrameInputs != null ? FrameInputs.GetHashCode() : 0);
            }
        }
    }
}