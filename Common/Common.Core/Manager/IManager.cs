using System.Threading.Tasks;

namespace Start
{
    /// <summary>
    /// 接口定义了管理系统中各个模块管理器的标准接口。
    /// </summary>
    public interface IManager
    {
        /// <summary>
        /// 获取管理器的优先级。
        /// </summary>
        /// <value>管理器的优先级值。</value>
        int Priority { get; }
    
        /// <summary>
        /// 初始化管理器。在系统启动或模块加载时调用。
        /// </summary>
        /// <returns>初始化过程的异步任务。</returns>
        Task Initialize();
    
        /// <summary>
        /// 反初始化管理器。在系统关闭或模块卸载时调用。
        /// </summary>
        /// <returns>反初始化过程的异步任务。</returns>
        Task DeInitialize();
    }
}