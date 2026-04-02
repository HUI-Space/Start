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
        public FP StartTime { get; private set; }
        
        /// <summary>
        /// 开始帧
        /// </summary>
        public int StartFrame { get; private set; }
        
        /// <summary>
        /// 帧间隔
        /// </summary>
        public FP Interval { get; private set; }
        
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="startTime">时间</param>
        /// <param name="startFrame">当前帧</param>
        /// <param name="interval">帧间隔</param>
        public FrameTimeCounter(long startTime,int startFrame = 0,int interval = FrameConst.FrameInterval)
        {
            StartTime = (FP)startTime;
            StartFrame = startFrame;
            Interval = new FP(interval);
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="startTime">时间</param>
        /// <param name="startFrame">当前帧</param>
        /// <param name="interval">帧间隔</param>
        public FrameTimeCounter(FP startTime,int startFrame = 0,int interval = FrameConst.FrameInterval)
        {
            StartTime = startTime;
            StartFrame = startFrame;
            Interval = new FP(interval);
        }
        
        /// <summary>
        /// 获取当前帧的时间
        /// </summary>
        /// <param name="frame">当前帧</param>
        /// <returns></returns>
        public FP FrameTime(int frame)
        {
            return StartTime + (frame - StartFrame) * Interval;
        }
        
        /// <summary>
        /// 修改帧间隔
        /// </summary>
        /// <param name="interval">帧间隔</param>
        /// <param name="frame">当前帧</param>
        public void ChangeInterval(int interval, int frame)
        {
            StartTime += (frame - StartFrame) * Interval;
            StartFrame = frame;
            Interval = new FP(interval);
        }
        
        /// <summary>
        /// 修改帧间隔
        /// </summary>
        /// <param name="interval">帧间隔</param>
        /// <param name="frame">当前帧</param>
        public void ChangeInterval(FP interval, int frame)
        {
            StartTime += (frame - StartFrame) * Interval;
            StartFrame = frame;
            Interval = interval;
        }
        
        /// <summary>
        /// 重置
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="frame">当前帧</param>
        public void Reset(FP time, int frame)
        {
            StartTime = time;
            StartFrame = frame;
        }
        
        /// <summary>
        /// 重置
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="frame">当前帧</param>
        public void Reset(long time, int frame)
        {
            StartTime = time;
            StartFrame = frame;
        }
    }
}