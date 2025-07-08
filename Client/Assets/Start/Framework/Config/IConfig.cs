namespace Start
{
    /// <summary>
    /// 配置接口，用于定义配置对象的生命周期方法
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        /// 初始化配置对象
        /// </summary>
        void Initialize();
    
        /// <summary>
        /// 反初始化配置对象
        /// </summary>
        void DeInitialize();
    }
}