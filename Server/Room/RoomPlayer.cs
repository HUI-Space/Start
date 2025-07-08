

namespace Start.Server
{
    public class RoomPlayer : IReference
    {
        public int SessionId { get; private set; }
        
        public int PlayerId { get; private set; }
        
        public int Progress { get; private set; }
        
        public bool IsOnline { get; private set; } = true;
        
        public static RoomPlayer Create(int sessionId, int playerId)
        {
            RoomPlayer roomPlayer = ReferencePool.Acquire<RoomPlayer>();
            roomPlayer.SessionId = sessionId;
            roomPlayer.PlayerId = playerId;
            return roomPlayer;
        }

        public void SetOnline(bool isOnline)
        {
            IsOnline = isOnline;
        }

        public void SetProgress(int progress)
        {
            Progress = progress;
        }
        
        public void Clear()
        {
            Progress = default;
            IsOnline = default;
        }
    }
}

    