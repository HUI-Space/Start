namespace Start
{
    public class TimeCounter
    {
        /// <summary>
        /// 帧间隔
        /// </summary>
        public int Interval { get; private set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        private long _startTime;
        
        /// <summary>
        /// 开始帧
        /// </summary>
        private int _startFrame;

        public TimeCounter(long startTime, int startFrame, int interval)
        {
            _startTime = startTime;
            _startFrame = startFrame;
            Interval = interval;
        }

        /// <summary>
        /// 修改帧间隔
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="frame"></param>
        public void ChangeInterval(int interval, int frame)
        {
            _startTime += (frame - _startFrame) * Interval;
            _startFrame = frame;
            Interval = interval;
        }

        /// <summary>
        /// 获取当前帧的时间
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public long FrameTime(int frame)
        {
            return _startTime + (frame - _startFrame) * Interval;
        }

        /// <summary>
        /// 重置
        /// </summary>
        /// <param name="time"></param>
        /// <param name="frame"></param>
        public void Reset(long time, int frame)
        {
            _startTime = time;
            _startFrame = frame;
        }
    }
}