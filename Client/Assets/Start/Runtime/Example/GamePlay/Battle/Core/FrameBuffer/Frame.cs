using System;

namespace Start
{
    /// <summary>
    /// 具体的逻辑帧
    /// 标记了该帧的序号、玩家数以及每个玩家的操作
    /// </summary>
    [Serializable]
    public struct Frame : IEquatable<Frame>
    {
        /// <summary>
        /// 权威帧
        /// </summary>
        public int AuthorityFrame;
        
        /// <summary>
        /// 玩家数量
        /// </summary>
        public int PlayerCount;
        
        /// <summary>
        /// 玩家输入
        /// </summary>
        public PlayerInput[] PlayerInputs;

        public Frame(int authorityFrame,int playerCount)
        {
            AuthorityFrame = authorityFrame;
            PlayerCount = playerCount;
            PlayerInputs = new PlayerInput[playerCount];
            for (int i = 0; i < playerCount; i++)
            {
                PlayerInputs[i] = new PlayerInput((uint)i);
            }
        }
        
        public Frame(int authorityFrame, PlayerInput[] playerInputs)
        {
            AuthorityFrame = authorityFrame;
            PlayerCount = playerInputs.Length;
            PlayerInputs = playerInputs;
        }
        
        public PlayerInput this[int index] 
        {
            get
            {
                if (index < PlayerCount)
                {
                    return PlayerInputs[index];
                }
                return new PlayerInput();
            }
            set
            {
                if (index < PlayerCount)
                {
                    PlayerInputs[index] = value;
                }
            }
        }

        public void SetPlayerInputByIndex(int index, PlayerInput playerInput)
        {
            if (index < PlayerCount)
            {
                PlayerInputs[index] = playerInput;
            }
        }

        public bool GetPlayerInputByIndex(int index,out PlayerInput playerInput)
        {
            playerInput = default;
            if (index < PlayerCount)
            {
                playerInput = PlayerInputs[index];
                return true;
            }
            return false;
        }


        public bool Equals(Frame other)
        {
            return AuthorityFrame == other.AuthorityFrame && PlayerCount == other.PlayerCount && Equals(PlayerInputs, other.PlayerInputs);
        }

        public override bool Equals(object obj)
        {
            return obj is Frame other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = AuthorityFrame;
                hashCode = (hashCode * 397) ^ PlayerCount;
                hashCode = (hashCode * 397) ^ (PlayerInputs != null ? PlayerInputs.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}