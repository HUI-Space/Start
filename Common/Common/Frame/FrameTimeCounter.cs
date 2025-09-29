namespace Start
{
    /// <summary>
    /// 帧时间计数器
    /// </summary>
    public class FrameTimeCounter
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public long StartTime { get; private set; }
        
        /// <summary>
        /// 开始帧
        /// </summary>
        public int StartFrame { get; private set; }
        
        /// <summary>
        /// 帧间隔
        /// </summary>
        public int Interval { get; private set; }
        
        public FrameTimeCounter(long startTime,int startFrame = 0,int interval = FrameConst.FrameInterval)
        {
            StartTime = startTime;
            StartFrame = startFrame;
            Interval = interval;
        }

        /// <summary>
        /// 修改帧间隔
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="frame"></param>
        public void ChangeInterval(int interval, int frame)
        {
            StartTime += (frame - StartFrame) * Interval;
            StartFrame = frame;
            Interval = interval;
        }

        /// <summary>
        /// 获取当前帧的时间
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public long FrameTime(int frame)
        {
            return StartTime + (frame - StartFrame) * Interval;
        }

        /// <summary>
        /// 重置
        /// </summary>
        /// <param name="time"></param>
        /// <param name="frame"></param>
        public void Reset(long time, int frame)
        {
            StartTime = time;
            StartFrame = frame;
        }
    }
}