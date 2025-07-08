namespace Start
{
    /// <summary>
    /// 定义一个回收接口，用于规范对象的回收操作
    /// </summary>
    public interface IRecycle
    {
        /// <summary>
        /// 执行回收操作，用于释放对象占用的资源
        /// </summary>
        void Recycle();
    }
}