namespace Start
{
    /// <summary>
    /// 可复用对象接口（标记对象可被回收复用，需实现状态重置）
    /// </summary>
    public interface IReusable
    {
        /// <summary>
        /// 重置对象状态（归还池时调用，确保下次复用无状态残留）
        /// </summary>
        void Reset();
    }
}