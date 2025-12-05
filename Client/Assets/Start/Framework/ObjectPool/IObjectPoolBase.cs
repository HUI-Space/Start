namespace Start
{
    // 泛型对象池基接口
    public interface IObjectPoolBase : IReusable
    {
        /// <summary>
        /// 获取或设置对象池的优先级。
        /// </summary>
        int Priority { get; set; }
        
        void Update(float elapseSeconds, float realElapseSeconds);
        
        /// <summary>
        /// 释放对象池中的可释放对象。
        /// </summary>
        void Release();
        
        /// <summary>
        /// 释放对象池中的所有未使用对象。
        /// </summary>
        void ReleaseAllUnused();
        
        void DeInitialize();
    }
}