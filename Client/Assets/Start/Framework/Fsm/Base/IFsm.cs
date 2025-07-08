using System;

namespace Start
{
    public interface IFsm<T> where T : class
    {
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get;  }
        
        /// <summary>
        /// 拥有者
        /// </summary>
        T Owner  { get; }
        
        /// <summary>
        /// 状态数量
        /// </summary>
        int FsmStateCount { get; } 
        
        /// <summary>
        /// 运行中
        /// </summary>
        bool IsRunning { get; }
        
        /// <summary>
        /// 是否销毁
        /// </summary>
        bool IsDestroyed { get; }
        
        /// <summary>
        /// 获取当前有限状态机状态持续时间
        /// </summary>
        float CurrentStateTime{ get; }
        
        /// <summary>
        /// 是否处于两个状态之间的过渡期
        /// </summary>
        bool InTransition { get; }
        
        /// <summary>
        /// 下一个状态
        /// </summary>
        Type NextState { get; }
        
        /// <summary>
        /// 是否存在有限状态机数据。
        /// </summary>
        /// <param name="name">有限状态机数据名称。</param>
        /// <returns>有限状态机数据是否存在。</returns>
        bool HasData(string name);

        /// <summary>
        /// 获取有限状态机数据。
        /// </summary>
        /// <param name="name">有限状态机数据名称。</param>
        /// <returns>要获取的有限状态机数据。</returns>
        TData GetData<TData>(string name) where TData : class;
        
        /// <summary>
        /// 设置有限状态机数据。
        /// </summary>
        /// <param name="name">有限状态机数据名称。</param>
        /// <param name="data">要设置的有限状态机数据。</param>
        void SetData<TData>(string name, TData data) where TData : class;
        
        /// <summary>
        /// 移除有限状态机数据。
        /// </summary>
        /// <param name="name">有限状态机数据名称。</param>
        /// <returns>是否移除有限状态机数据成功。</returns>
        bool RemoveData(string name);
    }
}