namespace Start.Server
{
    public class RoomManager : ManagerBase<RoomManager>, IUpdateManger
    {
        public override int Priority => 4;
        
        private int _roomId = 0;
        
        private readonly List<int> _matchList = new();
        private readonly Dictionary<int,Room> _waitingRooms = new();
        private readonly SortedDictionary<int,Room> _updateRooms = new();
        public override Task Initialize()
        {
            EventManager.Instance.AddListener((int)EMessageType.Kcp,(int)EMessageID.RoomMatch_C2S,RoomMatch);
            EventManager.Instance.AddListener((int)EMessageType.Kcp,(int)EMessageID.RoomReady_C2S,RoomReady);
            EventManager.Instance.AddListener((int)EMessageType.Kcp,(int)EMessageID.RoomFrame_C2S,RoomFrame);
            return base.Initialize();
        }
        
        public override Task DeInitialize()
        {
            EventManager.Instance.RemoveListener((int)EMessageType.Kcp,(int)EMessageID.RoomMatch_C2S,RoomMatch);
            EventManager.Instance.RemoveListener((int)EMessageType.Kcp,(int)EMessageID.RoomReady_C2S,RoomReady);
            EventManager.Instance.RemoveListener((int)EMessageType.Kcp,(int)EMessageID.RoomFrame_C2S,RoomFrame);
            return base.DeInitialize();
        }
        
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            foreach (var item in _updateRooms)
            {
                item.Value.Update(elapseSeconds,realElapseSeconds);
            }
        }
        
        private void RoomMatch(IGenericData genericData)
        {
            int sessionId = genericData.GetData1<int>();
            RoomMatch_C2S roomMatchC2S = genericData.GetData2<RoomMatch_C2S>();
            if (roomMatchC2S.MatchStatus == (int)EMatchStatus.GiveUp)
            {
                if (_matchList.Contains(sessionId))
                {
                    _matchList.Remove(sessionId);
                    RoomMatch_S2C roomMatchS2C = new RoomMatch_S2C();
                    roomMatchS2C.MatchStatus = (int)EMatchStatus.GiveUpSuccess;
                    KcpManager.Instance.SendAsync(sessionId, roomMatchS2C);
                }
            }
            else if (roomMatchC2S.MatchStatus == (int)EMatchStatus.Ready)
            {
                RoomMatch_S2C roomMatchS2C = new RoomMatch_S2C();
                roomMatchS2C.MatchStatus = (int)EMatchStatus.Waiting;
                KcpManager.Instance.SendAsync(sessionId, roomMatchS2C);
                _matchList.Add(sessionId);
                
                if (_matchList.Count < FrameConst.MatchCount)
                {
                    return;
                }
                
                Room room = Room.Create(++_roomId);
                
                for (int i = 0; i < FrameConst.MatchCount; i++)
                {
                    int sessionId1 = _matchList[i];
                    RoomMatchSuccess_S2C s2c = new RoomMatchSuccess_S2C();
                    s2c.RoomId = room.RoomId;
                    s2c.PlayerId = i;
                    room.AddPlayer(sessionId1,i);
                    KcpManager.Instance.SendAsync(sessionId1, s2c);
                }
                _waitingRooms.Add(room.RoomId,room);
                _matchList.Clear();
            }
        }

        private void RoomReady(IGenericData genericData)
        {
            uint sessionId = genericData.GetData1<uint>();
            RoomReady_C2S roomReadyC2S = genericData.GetData2<RoomReady_C2S>();
            if (_waitingRooms.TryGetValue(roomReadyC2S.RoomId,out Room room))
            {
                room.PlayerReady(sessionId);
                if (room.IsAllPlayerReady())
                {
                    long startTime = TimeUtility.TimeStamp() + 3000;//加上三秒
                    foreach (RoomPlayer roomPlayer in room.RoomPlayers)
                    {
                        RoomStart_S2C s2c = new RoomStart_S2C();
                        s2c.StartTime = startTime;
                        KcpManager.Instance.SendAsync(roomPlayer.SessionId, s2c);
                    }
                    room.StartRoom(startTime);
                    _updateRooms.Add(room.RoomId,room);
                }
            }
        }
        
        private void RoomFrame(IGenericData genericData)
        {
            uint sessionId = genericData.GetData1<uint>();
            RoomFrame_C2S roomFrameC2S = genericData.GetData2<RoomFrame_C2S>();
            if (_updateRooms.TryGetValue(roomFrameC2S.RoomId,out Room room))
            {
                room.SyncFrameInput(sessionId, roomFrameC2S.AuthorityFrame, roomFrameC2S.FrameInput);
            }
        }
    }
}