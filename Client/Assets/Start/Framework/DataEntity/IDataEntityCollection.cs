namespace Start
{
    /// <summary>
    /// 定义数据实体集合的接口，用于管理数据实体的生命周期和状态
    /// </summary>
    public interface IDataEntityCollection
    {
        /// <summary>
        /// 初始化数据实体集合，进行必要的资源分配和状态准备
        /// </summary>
        void Initialize();

        /// <summary>
        /// 重置数据实体集合的状态，可能包括清除数据、释放资源等操作
        /// </summary>
        void Reset();

        /// <summary>
        /// 反初始化数据实体集合，释放所有分配的资源，准备终止使用
        /// </summary>
        void DeInitialize();
    }
}
