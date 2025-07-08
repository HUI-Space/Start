namespace Start
{
    public class GenericData<T1> : IGenericData
    {
        private T1 _data1;

        public static GenericData<T1> Create()
        {
            return ReferencePool.Acquire<GenericData<T1>>();
        }

        public static GenericData<T1> Create(T1 t1)
        {
            GenericData<T1> genericData = ReferencePool.Acquire<GenericData<T1>>();
            return genericData.SetData(t1);
        }

        public GenericData<T1> SetData(T1 data)
        {
            _data1 = data;
            return this;
        }

        public T GetData1<T>()
        {
            if (_data1 is T t1)
            {
                return t1;
            }

            return default;
        }

        public T GetData2<T>()
        {
            return default;
        }

        public T GetData3<T>()
        {
            return default;
        }

        public T GetData4<T>()
        {
            return default;
        }

        public T GetData5<T>()
        {
            return default;
        }

        public T GetData6<T>()
        {
            return default;
        }

        public void Clear()
        {
            _data1 = default;
        }
    }

    public class GenericData<T1, T2> : IGenericData
    {
        private T1 _data1;
        private T2 _data2;

        public static GenericData<T1, T2> Create()
        {
            return ReferencePool.Acquire<GenericData<T1, T2>>();
        }

        public static GenericData<T1, T2> Create(T1 t1, T2 t2)
        {
            GenericData<T1, T2> genericData = ReferencePool.Acquire<GenericData<T1, T2>>();
            return genericData.SetData(t1, t2);
        }

        public GenericData<T1, T2> SetData(T1 data1, T2 data2)
        {
            _data1 = data1;
            _data2 = data2;
            return this;
        }

        public T GetData1<T>()
        {
            if (_data1 is T t1)
            {
                return t1;
            }

            return default;
        }

        public T GetData2<T>()
        {
            if (_data2 is T t2)
            {
                return t2;
            }

            return default;
        }

        public T GetData3<T>()
        {
            return default;
        }

        public T GetData4<T>()
        {
            return default;
        }

        public T GetData5<T>()
        {
            return default;
        }

        public T GetData6<T>()
        {
            return default;
        }

        public void Clear()
        {
            _data1 = default;
            _data2 = default;
        }
    }

    public class GenericData<T1, T2, T3> : IGenericData
    {
        private T1 _data1;
        private T2 _data2;
        private T3 _data3;

        public static GenericData<T1, T2, T3> Create()
        {
            return ReferencePool.Acquire<GenericData<T1, T2, T3>>();
        }

        public static GenericData<T1, T2, T3> Create(T1 t1, T2 t2, T3 t3)
        {
            GenericData<T1, T2, T3> genericData = ReferencePool.Acquire<GenericData<T1, T2, T3>>();
            return genericData.SetData(t1, t2, t3);
        }

        public GenericData<T1, T2, T3> SetData(T1 data1, T2 data2, T3 data3)
        {
            _data1 = data1;
            _data2 = data2;
            _data3 = data3;
            return this;
        }

        public void Clear()
        {
            _data1 = default;
            _data2 = default;
            _data3 = default;
        }

        public T GetData1<T>()
        {
            if (_data1 is T t1)
            {
                return t1;
            }

            return default;
        }

        public T GetData2<T>()
        {
            if (_data2 is T t2)
            {
                return t2;
            }

            return default;
        }

        public T GetData3<T>()
        {
            if (_data3 is T t3)
            {
                return t3;
            }

            return default;
        }

        public T GetData4<T>()
        {
            return default;
        }

        public T GetData5<T>()
        {
            return default;
        }

        public T GetData6<T>()
        {
            return default;
        }
    }

    public class GenericData<T1, T2, T3, T4> : IGenericData
    {
        private T1 _data1;
        private T2 _data2;
        private T3 _data3;
        private T4 _data4;

        public static GenericData<T1, T2, T3, T4> Create()
        {
            return ReferencePool.Acquire<GenericData<T1, T2, T3, T4>>();
        }

        public static GenericData<T1, T2, T3, T4> Create(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            GenericData<T1, T2, T3, T4> genericData = ReferencePool.Acquire<GenericData<T1, T2, T3, T4>>();
            return genericData.SetData(t1, t2, t3, t4);
        }

        public GenericData<T1, T2, T3, T4> SetData(T1 data1, T2 data2, T3 data3, T4 data4)
        {
            _data1 = data1;
            _data2 = data2;
            _data3 = data3;
            _data4 = data4;
            return this;
        }

        public void Clear()
        {
            _data1 = default;
            _data2 = default;
            _data3 = default;
            _data4 = default;
        }

        public T GetData1<T>()
        {
            if (_data1 is T t1)
            {
                return t1;
            }

            return default;
        }

        public T GetData2<T>()
        {
            if (_data2 is T t2)
            {
                return t2;
            }

            return default;
        }

        public T GetData3<T>()
        {
            if (_data3 is T t3)
            {
                return t3;
            }

            return default;
        }

        public T GetData4<T>()
        {
            if (_data4 is T t4)
            {
                return t4;
            }

            return default;
        }

        public T GetData5<T>()
        {
            return default;
        }

        public T GetData6<T>()
        {
            return default;
        }
    }

    public class GenericData<T1, T2, T3, T4, T5> : IGenericData
    {
        private T1 _data1;
        private T2 _data2;
        private T3 _data3;
        private T4 _data4;
        private T5 _data5;

        public static GenericData<T1, T2, T3, T4, T5> Create()
        {
            return ReferencePool.Acquire<GenericData<T1, T2, T3, T4, T5>>();
        }

        public static GenericData<T1, T2, T3, T4, T5> Create(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        {
            GenericData<T1, T2, T3, T4, T5> genericData = ReferencePool.Acquire<GenericData<T1, T2, T3, T4, T5>>();
            return genericData.SetData(t1, t2, t3, t4, t5);
        }

        public GenericData<T1, T2, T3, T4, T5> SetData(T1 data1, T2 data2, T3 data3, T4 data4, T5 data5)
        {
            _data1 = data1;
            _data2 = data2;
            _data3 = data3;
            _data4 = data4;
            _data5 = data5;
            return this;
        }

        public void Clear()
        {
            _data1 = default;
            _data2 = default;
            _data3 = default;
            _data4 = default;
            _data5 = default;
        }

        public T GetData1<T>()
        {
            if (_data1 is T t1)
            {
                return t1;
            }

            return default;
        }

        public T GetData2<T>()
        {
            if (_data2 is T t2)
            {
                return t2;
            }

            return default;
        }

        public T GetData3<T>()
        {
            if (_data3 is T t3)
            {
                return t3;
            }

            return default;
        }

        public T GetData4<T>()
        {
            if (_data4 is T t4)
            {
                return t4;
            }

            return default;
        }

        public T GetData5<T>()
        {
            if (_data5 is T t5)
            {
                return t5;
            }

            return default;
        }

        public T GetData6<T>()
        {
            return default;
        }
    }

    public class GenericData<T1, T2, T3, T4, T5, T6> : IGenericData
    {
        private T1 _data1;
        private T2 _data2;
        private T3 _data3;
        private T4 _data4;
        private T5 _data5;
        private T6 _data6;

        public static GenericData<T1, T2, T3, T4, T5, T6> Create()
        {
            return ReferencePool.Acquire<GenericData<T1, T2, T3, T4, T5, T6>>();
        }

        public static GenericData<T1, T2, T3, T4, T5, T6> Create(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        {
            GenericData<T1, T2, T3, T4, T5, T6> genericData =
                ReferencePool.Acquire<GenericData<T1, T2, T3, T4, T5, T6>>();
            return genericData.SetData(t1, t2, t3, t4, t5, t6);
        }

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

        public void Clear()
        {
            _data1 = default;
            _data2 = default;
            _data3 = default;
            _data4 = default;
            _data5 = default;
            _data6 = default;
        }

        public T GetData1<T>()
        {
            if (_data1 is T t1)
            {
                return t1;
            }

            return default;
        }

        public T GetData2<T>()
        {
            if (_data2 is T t2)
            {
                return t2;
            }

            return default;
        }

        public T GetData3<T>()
        {
            if (_data3 is T t3)
            {
                return t3;
            }

            return default;
        }

        public T GetData4<T>()
        {
            if (_data4 is T t4)
            {
                return t4;
            }

            return default;
        }

        public T GetData5<T>()
        {
            if (_data5 is T t5)
            {
                return t5;
            }

            return default;
        }

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