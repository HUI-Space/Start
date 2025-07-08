using System.Threading.Tasks;

namespace Start
{
    public partial class UIAction : IGenericData
    {
        public string UIName { get; private set; }
        public string ActionType { get; private set; }

        public Task Dispatch()
        {
            Logger.Info($"派发UIAction UIName:{UIName} 事件类型：{ActionType}");
            return UIManager.Instance.Dispatch(this);
        }

        public void Clear()
        {
            ActionType = default;
            UIName = default;
            if (_data != null)
            {
                ReferencePool.Release(_data);
                _data = default;
            }
        }

        /// <summary>
        /// 创建UI事件类型
        /// </summary>
        /// <param name="actionType">事件类型</param>
        /// <returns></returns>
        public static UIAction Create(string actionType)
        {
            UIAction uiAction = ReferencePool.Acquire<UIAction>();
            uiAction.ActionType = actionType;
            return uiAction;
        }

        /// <summary>
        /// 创建UI事件类型（需指定UI名称才可操作）
        /// </summary>
        /// <param name="uiName">UI名称</param>
        /// <param name="actionType">事件类型</param>
        /// <returns></returns>
        public static UIAction Create(string uiName, string actionType)
        {
            UIAction uiAction = ReferencePool.Acquire<UIAction>();
            uiAction.UIName = uiName;
            uiAction.ActionType = actionType;
            return uiAction;
        }
    }

    public partial class UIAction
    {
        public IGenericData Data => _data;

        private IGenericData _data;

        public void SetGenericData(IGenericData data)
        {
            if (_data != null)
            {
                ReferencePool.Release(_data);
            }
            _data = data;
        }

        public T GetData1<T>()
        {
            return _data.GetData1<T>();
        }

        public T GetData2<T>()
        {
            return _data.GetData2<T>();
        }

        public T GetData3<T>()
        {
            return _data.GetData3<T>();
        }

        public T GetData4<T>()
        {
            return _data.GetData4<T>();
        }

        public T GetData5<T>()
        {
            return _data.GetData5<T>();
        }

        public T GetData6<T>()
        {
            return _data.GetData6<T>();
        }

        public UIAction SetData<T1>(T1 data1)
        {
            if (_data != null)
            {
                ReferencePool.Release(_data);
                _data = null;
            }
            _data = ReferencePool.Acquire<GenericData<T1>>().SetData(data1);
            return this;
        }

        public UIAction SetData<T1, T2>(T1 data1, T2 data2)
        {
            if (_data != null)
            {
                ReferencePool.Release(_data);
                _data = null;
            }
            _data = ReferencePool.Acquire<GenericData<T1, T2>>().SetData(data1, data2);
            return this;
        }

        public UIAction SetData<T1, T2, T3>(T1 data1, T2 data2, T3 data3)
        {
            if (_data != null)
            {
                ReferencePool.Release(_data);
                _data = null;
            }
            _data = ReferencePool.Acquire<GenericData<T1, T2, T3>>().SetData(data1, data2, data3);
            return this;
        }

        public UIAction SetData<T1, T2, T3, T4>(T1 data1, T2 data2, T3 data3, T4 data4)
        {
            if (_data != null)
            {
                ReferencePool.Release(_data);
                _data = null;
            }
            _data = ReferencePool.Acquire<GenericData<T1, T2, T3, T4>>().SetData(data1, data2, data3, data4);
            return this;
        }

        public UIAction SetData<T1, T2, T3, T4, T5>(T1 data1, T2 data2, T3 data3, T4 data4, T5 data5)
        {
            if (_data != null)
            {
                ReferencePool.Release(_data);
                _data = null;
            }
            _data = ReferencePool.Acquire<GenericData<T1, T2, T3, T4, T5>>().SetData(data1, data2, data3, data4, data5);
            return this;
        }

        public UIAction SetData<T1, T2, T3, T4, T5, T6>(T1 data1, T2 data2, T3 data3, T4 data4, T5 data5, T6 data6)
        {
            if (_data != null)
            {
                ReferencePool.Release(_data);
                _data = null;
            }
            _data = ReferencePool.Acquire<GenericData<T1, T2, T3, T4, T5, T6>>().SetData(data1, data2, data3, data4,
                data5, data6);
            return this;
        }
    }
}