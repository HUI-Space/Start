namespace Start
{
    public class FrameBuffer
    {
        /// <summary>
        /// 网络同步的确认的当前最大的确认帧
        /// </summary>
        private int _authorityFrame;
        private readonly Frame[] _frames;

        public FrameBuffer(int capacity = BattleConst.FramePerSecond * 60)
        {
            _authorityFrame = 0;
            _frames = new Frame[capacity];
            for (int i = 0; i < capacity; ++i)
            {
                _frames[i] = new Frame();
            }
        }
        
        /// <summary>
        /// 获取预测帧（当预测帧小于权威帧时候，获取的帧数据为正真的权威帧，（优化操作））
        /// </summary>
        /// <param name="PredictionFrame">帧</param>
        /// <returns></returns>
        public Frame GetPredictionFrame(int PredictionFrame)
        {
            if (PredictionFrame <= _authorityFrame)
            {
                //此时获取的是权威帧
                return _frames[PredictionFrame % _frames.Length];
            }
            //此时获取的是预测帧（TODO 优化获取玩家的自己输入的操作）
            Frame frame = _frames[_authorityFrame % _frames.Length];
            Frame newFrame = new Frame(PredictionFrame,frame.PlayerInputs);
            return newFrame;
        }

        /// <summary>
        /// 尝试获取权威帧数据
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryGetAuthorityFrame(int frame, out Frame result)
        {
            result = default;
            if (frame > _authorityFrame)
            {
                return false;
            }
            result = _frames[frame % _frames.Length];
            return true;
        }
        
        /// <summary>
        /// 同步帧
        /// </summary>
        /// <param name="frame">当前权威帧</param>
        /// <param name="inputFrame">当前帧数据</param>
        public void SyncAuthorityFrame(int frame, Frame inputFrame)
        {
            _authorityFrame = frame;
            _frames[frame % _frames.Length] = inputFrame;
        }
    }
}