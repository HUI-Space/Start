namespace Start
{
    public class FrameBuffer
    {
        /// <summary>
        /// 网络同步的确认的当前最大的确认帧
        /// </summary>
        public int AuthorityFrame { get; private set; }
        
        private readonly FrameData[] _frameData;

        public FrameBuffer(int capacity = FrameConst.FramePerSecond * 60)
        {
            AuthorityFrame = 0;
            _frameData = new FrameData[capacity];
            for (int i = 0; i < capacity; ++i)
            {
                _frameData[i] = new FrameData();
            }
        }
        
        /// <summary>
        /// 获取预测帧（当预测帧小于权威帧时候，获取的帧数据为正真的权威帧，（优化操作））
        /// </summary>
        /// <param name="PredictionFrame">帧</param>
        /// <returns></returns>
        public FrameData GetPredictionFrameData(int PredictionFrame)
        {
            if (PredictionFrame <= AuthorityFrame)
            {
                //此时获取的是权威帧
                return _frameData[PredictionFrame % _frameData.Length];
            }
            //此时获取的是预测帧
            FrameData frameData = _frameData[AuthorityFrame % _frameData.Length];
            FrameData newFrameData = new FrameData(PredictionFrame,frameData.FrameInputs);
            return newFrameData;
        }

        /// <summary>
        /// 尝试获取权威帧数据
        /// </summary>
        /// <param name="authorityFrame"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryGetAuthorityFrameData(int authorityFrame, out FrameData result)
        {
            result = default;
            if (authorityFrame > AuthorityFrame)
            {
                return false;
            }
            result = _frameData[authorityFrame % _frameData.Length];
            return true;
        }
        
        /// <summary>
        /// 同步帧
        /// </summary>
        /// <param name="frameData">当前帧数据</param>
        public void SyncAuthorityFrameData(FrameData frameData)
        {
            AuthorityFrame = frameData.AuthorityFrame;
            _frameData[frameData.AuthorityFrame % _frameData.Length] = frameData;
        }
    }
}