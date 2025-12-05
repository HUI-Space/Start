namespace Start.Server
{
    public class Room : IReusable
    {
        public int RoomId { get; private set; }
        
        public RoomPlayer[] RoomPlayers { get; private set; }
        
        // 权威帧
        public int AuthorityFrame { get; set; } = -1;

        private FrameTimeCounter _frameTimeCounter;
        
        private List<Dictionary<int,uint>> _frameInputs;
        
        public static Room Create(int roomId)
        {
            Room room = RecyclableObjectPool.Acquire<Room>();
            room.RoomId = roomId;
            room.RoomPlayers = new RoomPlayer[FrameConst.MatchCount];
            room._frameInputs = new List<Dictionary<int,uint>>
            {
                new(),
                new(),
            };
            return room;
        }

        public void AddPlayer(int sessionId, int playerId)
        {
            RoomPlayer player = RoomPlayer.Create(sessionId, playerId);
            RoomPlayers[playerId] = player;
        }

        public void PlayerReady(uint sessionId)
        {
            foreach (RoomPlayer roomPlayer in RoomPlayers)
            {
                if (roomPlayer.SessionId == sessionId)
                {
                    roomPlayer.SetProgress(100);
                    break;
                }
            }
        }

        public bool IsAllPlayerReady()
        {
            return RoomPlayers.All(x => x.Progress == 100);
        }
        
        public void StartRoom(long startTime)
        {
            _frameTimeCounter = new FrameTimeCounter(startTime);
        }
        
        public void SyncFrameInput(uint sessionId, int authorityFrame, uint frameInput)
        {
            if (authorityFrame < AuthorityFrame || authorityFrame > AuthorityFrame + 10)
            {
                return;
            }
            
            int playerId = 0;
            for (int i = 0; i < RoomPlayers.Length; i++)
            {
                if (sessionId == RoomPlayers[i].SessionId)
                {
                    playerId = i;
                    break;
                }
            }
            Dictionary<int,uint> frameInputs = _frameInputs[playerId];
            frameInputs.TryAdd(authorityFrame, frameInput);
        }
        
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            long timeNow = TimeUtility.TimeStamp();;
            
            int frame = AuthorityFrame + 1;
            if (timeNow < _frameTimeCounter.FrameTime(frame))
            {
                return;
            }

            FrameData frameData = GetFrameData(frame);
            ++AuthorityFrame;
            
            SendFrameData(frameData);
        }
        
        private FrameData GetFrameData(int frame)
        {
            FrameData frameData = new FrameData(frame,FrameConst.MatchCount);
            for (int i = 0; i < _frameInputs.Count; i++)
            {
                Dictionary<int,uint> frameInputs = _frameInputs[i];
                if (frameInputs.TryGetValue(frame, out var input))
                {
                    frameData[i] = new FrameInput(input);
                }
                else
                {
                    uint frameInput = 0;
                    if (frame != 0)
                    {
                        frameInput = frameInputs[frame - 1];
                    }
                    frameData[i] = new FrameInput(frameInput);
                    frameInputs.Add(frame, frameInput);
                }
            }
            return frameData;
        }

        private void SendFrameData(FrameData frameData)
        {
            RoomFrame_S2C roomFrame = new RoomFrame_S2C();
            roomFrame.AuthorityFrame = AuthorityFrame;
            roomFrame.FrameInput = new uint[frameData.FrameInputs.Length];
            for (int i = 0; i < frameData.FrameInputs.Length; i++)
            {
                roomFrame.FrameInput[i] = frameData.FrameInputs[i].Raw;
            }
            foreach (RoomPlayer roomPlayer in RoomPlayers)
            {
                if (roomPlayer.IsOnline)
                {
                    KcpManager.Instance.SendAsync(roomPlayer.SessionId, roomFrame);
                }
            }
        }
        
        public void Reset()
        {
            
        }
    }
}

    