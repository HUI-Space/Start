

namespace Start.Server
{
    public class RoomPlayer : IReusable
    {
        public int SessionId { get; private set; }
        
        public int PlayerId { get; private set; }
        
        public int Progress { get; private set; }
        
        public bool IsOnline { get; private set; } = true;
        
        public static RoomPlayer Create(int sessionId, int playerId)
        {
            RoomPlayer roomPlayer = RecyclableObjectPool.Acquire<RoomPlayer>();
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
        
        public void Reset()
        {
            Progress = default;
            IsOnline = default;
        }
    }
}

    