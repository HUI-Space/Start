namespace Start
{
    public static class InputSystem
    {
        /// <summary>
        /// 摇杆旋转轴数
        /// </summary>
        public static readonly int Divisions = 24;

        /// <summary>
        /// 摇杆旋转轴角度
        /// </summary>
        public static readonly FP YawAngle = new FP(360) / new FP(Divisions) ;
        
        private static readonly TSVector[] _cacheYawToTSVector3 = new TSVector[Divisions];
        
        public static bool IsYawStop(int yaw)
        {
            return yaw == -1;
        }
        
        public static TSVector YawToTSVector(int yaw)
        {
            if (yaw <= 0)
            {
                return TSVector.Zero;
            }
            yaw -= 1;
            if (_cacheYawToTSVector3[yaw] == TSVector.Zero)
            {
                _cacheYawToTSVector3[yaw] = TSQuaternion.Euler(FP.Zero, yaw * YawAngle, FP.Zero) * TSVector.Forward;
            }
            
            return _cacheYawToTSVector3[yaw];
        }
        
    }
}