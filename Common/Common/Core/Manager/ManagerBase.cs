using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Start
{
    /// <summary>
    /// 管理者基类，提供管理器的统一接口和基本功能。此抽象类实现了IManager接口。
    /// 它定义了一个泛型类T，该类必须继承自ManagerBase<T>。
    /// </summary>
    /// <typeparam name="T">管理器的具体类型。</typeparam>
    public abstract class ManagerBase<T> : IManager where T : ManagerBase<T>
    {
        /// <summary>
        /// 获取当前管理器的实例。如果实例不存在，则通过Manager.GetManager<T>()方法创建一个。
        /// </summary>
        public static T Instance => _instance ?? (_instance = Manager.GetManger<T>());
    
        /// <summary>
        /// 获取管理器的优先级。这是一个抽象属性，必须在继承的类中实现。
        /// </summary>
        public abstract int Priority { get; }
    
        /// <summary>
        /// 存储当前管理器的实例。
        /// </summary>
        private static T _instance;
    
        /// <summary>
        /// 初始化管理器。默认实现不执行任何操作，子类可以重写此方法以提供初始化逻辑。
        /// </summary>
        /// <returns>一个已完成的任务。</returns>
        public virtual Task Initialize()
        {
            return Task.CompletedTask;
        }
    
        /// <summary>
        /// 反初始化管理器。默认实现将实例设置为null，子类可以重写此方法以提供反初始化逻辑。
        /// </summary>
        /// <returns>一个已完成的任务。</returns>
        public virtual Task DeInitialize()
        {
            _instance = null;
            return Task.CompletedTask;
        }
    }
}