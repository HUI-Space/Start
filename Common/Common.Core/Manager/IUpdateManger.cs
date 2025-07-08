namespace Start
{
    /// <summary>
    /// 定义一个更新管理器接口，用于管理软件的更新过程
    /// </summary>
    public interface IUpdateManger
    {
        /// <summary>
        /// 所有游戏框架模块轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        void Update(float elapseSeconds, float realElapseSeconds);
    }
}