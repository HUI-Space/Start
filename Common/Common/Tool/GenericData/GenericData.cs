namespace Start
{
    /// <summary>
    /// 单参数泛型数据容器，支持存储一个类型的数据
    /// </summary>
    /// <typeparam name="T1">第一个数据项的类型</typeparam>
    public class GenericData<T1> : IGenericData
    {
        private T1 _data1;

        /// <summary>
        /// 从对象池创建一个新的泛型数据实例
        /// </summary>
        /// <returns>从对象池获取的泛型数据实例</returns>
        public static GenericData<T1> Create()
        {
            return RecyclablePool.Acquire<GenericData<T1>>();
        }

        /// <summary>
        /// 从对象池创建一个新的泛型数据实例并设置初始数据
        /// </summary>
        /// <param name="t1">第一个数据项的值</param>
        /// <returns>已设置数据的泛型数据实例</returns>
        public static GenericData<T1> Create(T1 t1)
        {
            GenericData<T1> genericData = RecyclablePool.Acquire<GenericData<T1>>();
            return genericData.SetData(t1);
        }

        /// <summary>
        /// 设置数据项的值
        /// </summary>
        /// <param name="data">要设置的数据值</param>
        /// <returns>当前实例，支持链式调用</returns>
        public GenericData<T1> SetData(T1 data)
        {
            _data1 = data;
            return this;
        }

        /// <summary>
        /// 获取第一个数据项
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>如果类型匹配则返回数据，否则返回默认值</returns>
        public T GetData1<T>()
        {
            if (_data1 is T t1)
            {
                return t1;
            }

            return default;
        }

        /// <summary>
        /// 获取第二个数据项（此容器不支持）
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>始终返回默认值</returns>
        public T GetData2<T>()
        {
            return default;
        }

        /// <summary>
        /// 获取第三个数据项（此容器不支持）
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>始终返回默认值</returns>
        public T GetData3<T>()
        {
            return default;
        }

        /// <summary>
        /// 获取第四个数据项（此容器不支持）
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>始终返回默认值</returns>
        public T GetData4<T>()
        {
            return default;
        }

        /// <summary>
        /// 获取第五个数据项（此容器不支持）
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>始终返回默认值</returns>
        public T GetData5<T>()
        {
            return default;
        }

        /// <summary>
        /// 获取第六个数据项（此容器不支持）
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>始终返回默认值</returns>
        public T GetData6<T>()
        {
            return default;
        }

        /// <summary>
        /// 回收此实例，重置所有数据为默认值
        /// </summary>
        public void Recycle()
        {
            _data1 = default;
        }
    }

    /// <summary>
    /// 双参数泛型数据容器，支持存储两个不同类型的数据
    /// </summary>
    /// <typeparam name="T1">第一个数据项的类型</typeparam>
    /// <typeparam name="T2">第二个数据项的类型</typeparam>
    public class GenericData<T1, T2> : IGenericData
    {
        private T1 _data1;
        private T2 _data2;

        /// <summary>
        /// 从对象池创建一个新的泛型数据实例
        /// </summary>
        /// <returns>从对象池获取的泛型数据实例</returns>
        public static GenericData<T1, T2> Create()
        {
            return RecyclablePool.Acquire<GenericData<T1, T2>>();
        }

        /// <summary>
        /// 从对象池创建一个新的泛型数据实例并设置初始数据
        /// </summary>
        /// <param name="t1">第一个数据项的值</param>
        /// <param name="t2">第二个数据项的值</param>
        /// <returns>已设置数据的泛型数据实例</returns>
        public static GenericData<T1, T2> Create(T1 t1, T2 t2)
        {
            GenericData<T1, T2> genericData = RecyclablePool.Acquire<GenericData<T1, T2>>();
            return genericData.SetData(t1, t2);
        }

        /// <summary>
        /// 设置两个数据项的值
        /// </summary>
        /// <param name="data1">第一个数据项的值</param>
        /// <param name="data2">第二个数据项的值</param>
        /// <returns>当前实例，支持链式调用</returns>
        public GenericData<T1, T2> SetData(T1 data1, T2 data2)
        {
            _data1 = data1;
            _data2 = data2;
            return this;
        }

        /// <summary>
        /// 获取第一个数据项
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>如果类型匹配则返回数据，否则返回默认值</returns>
        public T GetData1<T>()
        {
            if (_data1 is T t1)
            {
                return t1;
            }

            return default;
        }

        /// <summary>
        /// 获取第二个数据项
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>如果类型匹配则返回数据，否则返回默认值</returns>
        public T GetData2<T>()
        {
            if (_data2 is T t2)
            {
                return t2;
            }

            return default;
        }

        /// <summary>
        /// 获取第三个数据项（此容器不支持）
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>始终返回默认值</returns>
        public T GetData3<T>()
        {
            return default;
        }

        /// <summary>
        /// 获取第四个数据项（此容器不支持）
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>始终返回默认值</returns>
        public T GetData4<T>()
        {
            return default;
        }

        /// <summary>
        /// 获取第五个数据项（此容器不支持）
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>始终返回默认值</returns>
        public T GetData5<T>()
        {
            return default;
        }

        /// <summary>
        /// 获取第六个数据项（此容器不支持）
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>始终返回默认值</returns>
        public T GetData6<T>()
        {
            return default;
        }

        /// <summary>
        /// 回收此实例，重置所有数据为默认值
        /// </summary>
        public void Recycle()
        {
            _data1 = default;
            _data2 = default;
        }
    }

    /// <summary>
    /// 三参数泛型数据容器，支持存储三个不同类型的数据
    /// </summary>
    /// <typeparam name="T1">第一个数据项的类型</typeparam>
    /// <typeparam name="T2">第二个数据项的类型</typeparam>
    /// <typeparam name="T3">第三个数据项的类型</typeparam>
    public class GenericData<T1, T2, T3> : IGenericData
    {
        private T1 _data1;
        private T2 _data2;
        private T3 _data3;

        /// <summary>
        /// 从对象池创建一个新的泛型数据实例
        /// </summary>
        /// <returns>从对象池获取的泛型数据实例</returns>
        public static GenericData<T1, T2, T3> Create()
        {
            return RecyclablePool.Acquire<GenericData<T1, T2, T3>>();
        }

        /// <summary>
        /// 从对象池创建一个新的泛型数据实例并设置初始数据
        /// </summary>
        /// <param name="t1">第一个数据项的值</param>
        /// <param name="t2">第二个数据项的值</param>
        /// <param name="t3">第三个数据项的值</param>
        /// <returns>已设置数据的泛型数据实例</returns>
        public static GenericData<T1, T2, T3> Create(T1 t1, T2 t2, T3 t3)
        {
            GenericData<T1, T2, T3> genericData = RecyclablePool.Acquire<GenericData<T1, T2, T3>>();
            return genericData.SetData(t1, t2, t3);
        }

        /// <summary>
        /// 设置三个数据项的值
        /// </summary>
        /// <param name="data1">第一个数据项的值</param>
        /// <param name="data2">第二个数据项的值</param>
        /// <param name="data3">第三个数据项的值</param>
        /// <returns>当前实例，支持链式调用</returns>
        public GenericData<T1, T2, T3> SetData(T1 data1, T2 data2, T3 data3)
        {
            _data1 = data1;
            _data2 = data2;
            _data3 = data3;
            return this;
        }

        /// <summary>
        /// 回收此实例，重置所有数据为默认值
        /// </summary>
        public void Recycle()
        {
            _data1 = default;
            _data2 = default;
            _data3 = default;
        }

        /// <summary>
        /// 获取第一个数据项
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>如果类型匹配则返回数据，否则返回默认值</returns>
        public T GetData1<T>()
        {
            if (_data1 is T t1)
            {
                return t1;
            }

            return default;
        }

        /// <summary>
        /// 获取第二个数据项
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>如果类型匹配则返回数据，否则返回默认值</returns>
        public T GetData2<T>()
        {
            if (_data2 is T t2)
            {
                return t2;
            }

            return default;
        }

        /// <summary>
        /// 获取第三个数据项
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>如果类型匹配则返回数据，否则返回默认值</returns>
        public T GetData3<T>()
        {
            if (_data3 is T t3)
            {
                return t3;
            }

            return default;
        }

        /// <summary>
        /// 获取第四个数据项（此容器不支持）
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>始终返回默认值</returns>
        public T GetData4<T>()
        {
            return default;
        }

        /// <summary>
        /// 获取第五个数据项（此容器不支持）
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>始终返回默认值</returns>
        public T GetData5<T>()
        {
            return default;
        }

        /// <summary>
        /// 获取第六个数据项（此容器不支持）
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>始终返回默认值</returns>
        public T GetData6<T>()
        {
            return default;
        }
    }

    /// <summary>
    /// 四参数泛型数据容器，支持存储四个不同类型的数据
    /// </summary>
    /// <typeparam name="T1">第一个数据项的类型</typeparam>
    /// <typeparam name="T2">第二个数据项的类型</typeparam>
    /// <typeparam name="T3">第三个数据项的类型</typeparam>
    /// <typeparam name="T4">第四个数据项的类型</typeparam>
    public class GenericData<T1, T2, T3, T4> : IGenericData
    {
        private T1 _data1;
        private T2 _data2;
        private T3 _data3;
        private T4 _data4;

        /// <summary>
        /// 从对象池创建一个新的泛型数据实例
        /// </summary>
        /// <returns>从对象池获取的泛型数据实例</returns>
        public static GenericData<T1, T2, T3, T4> Create()
        {
            return RecyclablePool.Acquire<GenericData<T1, T2, T3, T4>>();
        }

        /// <summary>
        /// 从对象池创建一个新的泛型数据实例并设置初始数据
        /// </summary>
        /// <param name="t1">第一个数据项的值</param>
        /// <param name="t2">第二个数据项的值</param>
        /// <param name="t3">第三个数据项的值</param>
        /// <param name="t4">第四个数据项的值</param>
        /// <returns>已设置数据的泛型数据实例</returns>
        public static GenericData<T1, T2, T3, T4> Create(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            GenericData<T1, T2, T3, T4> genericData = RecyclablePool.Acquire<GenericData<T1, T2, T3, T4>>();
            return genericData.SetData(t1, t2, t3, t4);
        }

        /// <summary>
        /// 设置四个数据项的值
        /// </summary>
        /// <param name="data1">第一个数据项的值</param>
        /// <param name="data2">第二个数据项的值</param>
        /// <param name="data3">第三个数据项的值</param>
        /// <param name="data4">第四个数据项的值</param>
        /// <returns>当前实例，支持链式调用</returns>
        public GenericData<T1, T2, T3, T4> SetData(T1 data1, T2 data2, T3 data3, T4 data4)
        {
            _data1 = data1;
            _data2 = data2;
            _data3 = data3;
            _data4 = data4;
            return this;
        }

        /// <summary>
        /// 回收此实例，重置所有数据为默认值
        /// </summary>
        public void Recycle()
        {
            _data1 = default;
            _data2 = default;
            _data3 = default;
            _data4 = default;
        }

        /// <summary>
        /// 获取第一个数据项
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>如果类型匹配则返回数据，否则返回默认值</returns>
        public T GetData1<T>()
        {
            if (_data1 is T t1)
            {
                return t1;
            }

            return default;
        }

        /// <summary>
        /// 获取第二个数据项
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>如果类型匹配则返回数据，否则返回默认值</returns>
        public T GetData2<T>()
        {
            if (_data2 is T t2)
            {
                return t2;
            }

            return default;
        }

        /// <summary>
        /// 获取第三个数据项
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>如果类型匹配则返回数据，否则返回默认值</returns>
        public T GetData3<T>()
        {
            if (_data3 is T t3)
            {
                return t3;
            }

            return default;
        }

        /// <summary>
        /// 获取第四个数据项
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>如果类型匹配则返回数据，否则返回默认值</returns>
        public T GetData4<T>()
        {
            if (_data4 is T t4)
            {
                return t4;
            }

            return default;
        }

        /// <summary>
        /// 获取第五个数据项（此容器不支持）
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>始终返回默认值</returns>
        public T GetData5<T>()
        {
            return default;
        }

        /// <summary>
        /// 获取第六个数据项（此容器不支持）
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>始终返回默认值</returns>
        public T GetData6<T>()
        {
            return default;
        }
    }

    /// <summary>
    /// 五参数泛型数据容器，支持存储五个不同类型的数据
    /// </summary>
    /// <typeparam name="T1">第一个数据项的类型</typeparam>
    /// <typeparam name="T2">第二个数据项的类型</typeparam>
    /// <typeparam name="T3">第三个数据项的类型</typeparam>
    /// <typeparam name="T4">第四个数据项的类型</typeparam>
    /// <typeparam name="T5">第五个数据项的类型</typeparam>
    public class GenericData<T1, T2, T3, T4, T5> : IGenericData
    {
        private T1 _data1;
        private T2 _data2;
        private T3 _data3;
        private T4 _data4;
        private T5 _data5;

        /// <summary>
        /// 从对象池创建一个新的泛型数据实例
        /// </summary>
        /// <returns>从对象池获取的泛型数据实例</returns>
        public static GenericData<T1, T2, T3, T4, T5> Create()
        {
            return RecyclablePool.Acquire<GenericData<T1, T2, T3, T4, T5>>();
        }

        /// <summary>
        /// 从对象池创建一个新的泛型数据实例并设置初始数据
        /// </summary>
        /// <param name="t1">第一个数据项的值</param>
        /// <param name="t2">第二个数据项的值</param>
        /// <param name="t3">第三个数据项的值</param>
        /// <param name="t4">第四个数据项的值</param>
        /// <param name="t5">第五个数据项的值</param>
        /// <returns>已设置数据的泛型数据实例</returns>
        public static GenericData<T1, T2, T3, T4, T5> Create(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            GenericData<T1, T2, T3, T4, T5> genericData = RecyclablePool.Acquire<GenericData<T1, T2, T3, T4, T5>>();
            return genericData.SetData(t1, t2, t3, t4, t5);
        }

        /// <summary>
        /// 设置五个数据项的值
        /// </summary>
        /// <param name="data1">第一个数据项的值</param>
        /// <param name="data2">第二个数据项的值</param>
        /// <param name="data3">第三个数据项的值</param>
        /// <param name="data4">第四个数据项的值</param>
        /// <param name="data5">第五个数据项的值</param>
        /// <returns>当前实例，支持链式调用</returns>
        public GenericData<T1, T2, T3, T4, T5> SetData(T1 data1, T2 data2, T3 data3, T4 data4, T5 data5)
        {
            _data1 = data1;
            _data2 = data2;
            _data3 = data3;
            _data4 = data4;
            _data5 = data5;
            return this;
        }

        /// <summary>
        /// 回收此实例，重置所有数据为默认值
        /// </summary>
        public void Recycle()
        {
            _data1 = default;
            _data2 = default;
            _data3 = default;
            _data4 = default;
            _data5 = default;
        }

        /// <summary>
        /// 获取第一个数据项
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>如果类型匹配则返回数据，否则返回默认值</returns>
        public T GetData1<T>()
        {
            if (_data1 is T t1)
            {
                return t1;
            }

            return default;
        }

        /// <summary>
        /// 获取第二个数据项
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>如果类型匹配则返回数据，否则返回默认值</returns>
        public T GetData2<T>()
        {
            if (_data2 is T t2)
            {
                return t2;
            }

            return default;
        }

        /// <summary>
        /// 获取第三个数据项
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>如果类型匹配则返回数据，否则返回默认值</returns>
        public T GetData3<T>()
        {
            if (_data3 is T t3)
            {
                return t3;
            }

            return default;
        }

        /// <summary>
        /// 获取第四个数据项
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>如果类型匹配则返回数据，否则返回默认值</returns>
        public T GetData4<T>()
        {
            if (_data4 is T t4)
            {
                return t4;
            }

            return default;
        }

        /// <summary>
        /// 获取第五个数据项
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>如果类型匹配则返回数据，否则返回默认值</returns>
        public T GetData5<T>()
        {
            if (_data5 is T t5)
            {
                return t5;
            }

            return default;
        }

        /// <summary>
        /// 获取第六个数据项（此容器不支持）
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>始终返回默认值</returns>
        public T GetData6<T>()
        {
            return default;
        }
    }

    /// <summary>
    /// 六参数泛型数据容器，支持存储六个不同类型的数据
    /// </summary>
    /// <typeparam name="T1">第一个数据项的类型</typeparam>
    /// <typeparam name="T2">第二个数据项的类型</typeparam>
    /// <typeparam name="T3">第三个数据项的类型</typeparam>
    /// <typeparam name="T4">第四个数据项的类型</typeparam>
    /// <typeparam name="T5">第五个数据项的类型</typeparam>
    /// <typeparam name="T6">第六个数据项的类型</typeparam>
    public class GenericData<T1, T2, T3, T4, T5, T6> : IGenericData
    {
        private T1 _data1;
        private T2 _data2;
        private T3 _data3;
        private T4 _data4;
        private T5 _data5;
        private T6 _data6;

        /// <summary>
        /// 从对象池创建一个新的泛型数据实例
        /// </summary>
        /// <returns>从对象池获取的泛型数据实例</returns>
        public static GenericData<T1, T2, T3, T4, T5, T6> Create()
        {
            return RecyclablePool.Acquire<GenericData<T1, T2, T3, T4, T5, T6>>();
        }

        /// <summary>
        /// 从对象池创建一个新的泛型数据实例并设置初始数据
        /// </summary>
        /// <param name="t1">第一个数据项的值</param>
        /// <param name="t2">第二个数据项的值</param>
        /// <param name="t3">第三个数据项的值</param>
        /// <param name="t4">第四个数据项的值</param>
        /// <param name="t5">第五个数据项的值</param>
        /// <param name="t6">第六个数据项的值</param>
        /// <returns>已设置数据的泛型数据实例</returns>
        public static GenericData<T1, T2, T3, T4, T5, T6> Create(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            GenericData<T1, T2, T3, T4, T5, T6> genericData =
                RecyclablePool.Acquire<GenericData<T1, T2, T3, T4, T5, T6>>();
            return genericData.SetData(t1, t2, t3, t4, t5, t6);
        }

        /// <summary>
        /// 设置六个数据项的值
        /// </summary>
        /// <param name="data1">第一个数据项的值</param>
        /// <param name="data2">第二个数据项的值</param>
        /// <param name="data3">第三个数据项的值</param>
        /// <param name="data4">第四个数据项的值</param>
        /// <param name="data5">第五个数据项的值</param>
        /// <param name="data6">第六个数据项的值</param>
        /// <returns>当前实例，支持链式调用</returns>
        public GenericData<T1, T2, T3, T4, T5, T6> SetData(T1 data1, T2 data2, T3 data3, T4 data4, T5 data5, T6 data6)
        {
            _data1 = data1;
            _data2 = data2;
            _data3 = data3;
            _data4 = data4;
            _data5 = data5;
            _data6 = data6;
            return this;
        }

        /// <summary>
        /// 回收此实例，重置所有数据为默认值
        /// </summary>
        public void Recycle()
        {
            _data1 = default;
            _data2 = default;
            _data3 = default;
            _data4 = default;
            _data5 = default;
            _data6 = default;
        }

        /// <summary>
        /// 获取第一个数据项
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>如果类型匹配则返回数据，否则返回默认值</returns>
        public T GetData1<T>()
        {
            if (_data1 is T t1)
            {
                return t1;
            }

            return default;
        }

        /// <summary>
        /// 获取第二个数据项
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>如果类型匹配则返回数据，否则返回默认值</returns>
        public T GetData2<T>()
        {
            if (_data2 is T t2)
            {
                return t2;
            }

            return default;
        }

        /// <summary>
        /// 获取第三个数据项
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>如果类型匹配则返回数据，否则返回默认值</returns>
        public T GetData3<T>()
        {
            if (_data3 is T t3)
            {
                return t3;
            }

            return default;
        }

        /// <summary>
        /// 获取第四个数据项
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>如果类型匹配则返回数据，否则返回默认值</returns>
        public T GetData4<T>()
        {
            if (_data4 is T t4)
            {
                return t4;
            }

            return default;
        }

        /// <summary>
        /// 获取第五个数据项
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>如果类型匹配则返回数据，否则返回默认值</returns>
        public T GetData5<T>()
        {
            if (_data5 is T t5)
            {
                return t5;
            }

            return default;
        }

        /// <summary>
        /// 获取第六个数据项
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <returns>如果类型匹配则返回数据，否则返回默认值</returns>
        public T GetData6<T>()
        {
            if (_data6 is T t6)
            {
                return t6;
            }

            return default;
        }
    }
}
