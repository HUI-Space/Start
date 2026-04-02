namespace Start
{
    public class FrameConst
    {
        /// <summary>
        /// 匹配数量
        /// </summary>
        public const int MatchCount = 2;
        
        /// <summary>
        /// 最大预测帧数量
        /// </summary>
        public const int MaxPredictionFrame = 5;
        
        /// <summary>
        /// 帧间隔
        /// </summary>
        public const int FrameInterval = 10;
        
        /// <summary>
        /// 每秒帧数 (1000 / FrameInterval)
        /// </summary>
        public const int FramePerSecond = 1000 / FrameInterval;
        
        /// <summary>
        /// 最大追赶帧数
        /// </summary>
        public const int MaxCatchUpFrame = 3;
        
        
        public const int MaxConfirmedMatchEntityCacheCount = 6;
        public const int MaxSnapshotMatchEntityCacheCount = 3;
        public const bool EnableDemoRecord = false;
        public static bool CameraFilter;
        public static int MD5CheckFrames = 200;
    }
}