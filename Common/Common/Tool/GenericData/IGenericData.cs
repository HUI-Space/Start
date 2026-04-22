namespace Start
{
    /// <summary>
    /// 泛型数据容器接口，支持存储和获取最多6个不同类型的数据项
    /// </summary>
    public interface IGenericData : IRecycle
    {
        /// <summary>
        /// 获取第一个数据项
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>如果类型匹配则返回数据，否则返回默认值</returns>
        T GetData1<T>();

        /// <summary>
        /// 获取第二个数据项
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>如果类型匹配则返回数据，否则返回默认值</returns>
        T GetData2<T>();

        /// <summary>
        /// 获取第三个数据项
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>如果类型匹配则返回数据，否则返回默认值</returns>
        T GetData3<T>();

        /// <summary>
        /// 获取第四个数据项
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>如果类型匹配则返回数据，否则返回默认值</returns>
        T GetData4<T>();

        /// <summary>
        /// 获取第五个数据项
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>如果类型匹配则返回数据，否则返回默认值</returns>
        T GetData5<T>();

        /// <summary>
        /// 获取第六个数据项
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>如果类型匹配则返回数据，否则返回默认值</returns>
        T GetData6<T>();
    }
}
