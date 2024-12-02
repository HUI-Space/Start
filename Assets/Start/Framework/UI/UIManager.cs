using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Start.Framework
{
    [Manager]
    public partial class UIManager : ManagerBase<UIManager>, IUpdateManger
    {
        public override int Priority => 12;
        
        private IUIHelper _iUIHelper;

        public static void SetHelper(IUIHelper iuiHelper)
        {
            Instance._iUIHelper = iuiHelper;
        }
        
        public override Task DeInitialize()
        {
            _iUIHelper = default;
            _middlewares.Clear();
            return base.DeInitialize();
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            UpdateUIBase();
        }
    }
    
    #region API --- Middleware
    public delegate Task Middleware(UIAction uiAction);
    public partial class UIManager
    {
        private readonly List<Middleware> _middlewares = new List<Middleware>();
        private readonly Dictionary<string, PriorityDelegate<IUIData>> _uiCallbacks = new Dictionary<string, PriorityDelegate<IUIData>>();
        private bool _isInitializeMiddleware;
        
        
        public async Task Dispatch(UIAction uiAction)
        {
            if (!_isInitializeMiddleware)
            {
                _middlewares.Clear();
                _middlewares.AddRange(_iUIHelper.GetMiddlewares());
                _isInitializeMiddleware = true;
            }
            foreach (Middleware middleware in _middlewares)
            {
                try
                {
                    var task = middleware(uiAction);
                    if (task != null)
                        await task;
                }
                catch (Exception e)
                {
                    Log.Error($"middleware error :{e}");
                }
            }
        }
        
        public async Task Middleware(UIAction action)
        {
            // 判断是否为空作为拦截避免不必要的性能浪费（此处中间件都需要以UI名称作为前提）
            if (string.IsNullOrEmpty(action.UIName))
            {
                return;
            }
            
            //打开UI
            if (action.ActionType == ActionType.OpenUI)
            {
                await OpenUI(action);
            }
                
            if (!_uiNames.Contains(action.UIName))
            {
                Log.Error($"{action.UIName} is not exist");
                return;
            }
                
            //派发消息 
            ReceiveAction(action);
            
            //回退UI
            if (action.ActionType == ActionType.GoBackUI)
            {
                await GoBackUI(action);
            }
            
            //关闭UI
            if (action.ActionType == ActionType.CloseUI)
            {
                await CloseUI(action);
            }
        }

        public Task DefaultDispatch(UIAction action)
        {
            ReferencePool.Release(action);
            return default;
        }
        
        private void UpdateUIBase()
        {
            //刷新UI
            foreach (IUIData uiData in _uiDataLinkedList)
            {
                if (uiData.IsDirty)
                {
                    uiData.BeforeClearDirty();
                    if (_uiCallbacks.TryGetValue(uiData.GetType().Name, out PriorityDelegate<IUIData> listeners))
                    {
                        listeners.Invoke(uiData);
                    }
                    uiData.ClearDirty();
                }
            }
        }
        
        public void AddListener<T>(Action<IUIData> callBack, int priority = 0) where T :IUIData
        {
            string dataName = typeof(T).Name;
            if (!_uiCallbacks.TryGetValue(dataName, out PriorityDelegate<IUIData> callbacks))
            {
                callbacks =  PriorityDelegate<IUIData>.Create();
                _uiCallbacks.Add(dataName, callbacks);
            }
            callbacks.AddListener(callBack, priority);
        }

        public void RemoveListener<T>(Action<IUIData> callBack) where T : IUIData
        {
            string dataName = typeof(T).Name;
            if (_uiCallbacks.TryGetValue(dataName, out PriorityDelegate<IUIData> callbacks))
            {
                callbacks.RemoveListener(callBack);
                if (callbacks.CanBeReleased)
                {
                    _uiCallbacks.Remove(dataName);
                    ReferencePool.Release(callbacks);
                }
            }
        }
    }
    
    #endregion

    /// <summary>
    /// 不允许打开同一个UI
    /// </summary>
    public partial class UIManager
    {
        private readonly LinkedList<string> _uiNames = new LinkedList<string>();
        private readonly LinkedList<IUIData> _uiDataLinkedList = new LinkedList<IUIData>();
        private readonly Dictionary<string, UIProxy> _uiProxies = new Dictionary<string, UIProxy>();
        private readonly Dictionary<Type ,IUIData> _uiDataDictionary = new Dictionary<Type, IUIData>();
        
        public bool GetUIData<T>(out IUIData uiData) where T : IUIData
        {
            return _uiDataDictionary.TryGetValue(typeof(T),out uiData);
        }
        
        /// <summary>
        /// 派发UIAction 到 UIData 只支持 UIAction.UIName 等于 UIAction.UIName 的UI
        /// 作用是单一职责，避免UI逻辑和UI管理器耦合
        /// </summary>
        /// <param name="action"></param>
        private void ReceiveAction(UIAction action)
        {
            foreach (IUIData uiData in _uiDataLinkedList)
            {
                if (uiData.UIName == action.UIName)
                {
                    uiData.ReceiveAction(action);
                    break;
                }
            }
        }
        
        private async Task OpenUI(UIAction uiAction)
        {
            if (_uiNames.Contains(uiAction.UIName))
            {
                Log.Error($"不允许打开同一个UI:{uiAction.UIName}");
                return;
            }
            GenericData<bool,EUIType,IUIBase, IUIData> result = await _iUIHelper.OpenUI(uiAction);
            if (result.GetData1<bool>())
            {
                string uiName = uiAction.UIName;
                EUIType uiType = result.GetData2<EUIType>();
                IUIBase uiBase = result.GetData3<IUIBase>();
                IUIData uiData = result.GetData4<IUIData>();
                uiData.Order = _uiNames.Count;
                // 如果当前UI是全屏UI,则需要隐藏上一个全屏UI
                if (uiType == EUIType.Panel)
                {
                    for (LinkedListNode<string> node = _uiNames.Last; node != null; node = node.Previous)
                    {
                        if (_uiProxies.TryGetValue(node.Value, out UIProxy uiProxy) && uiProxy.UIType == EUIType.Panel)
                        {
                            await UIAction.Create(node.Value, ActionType.HideUI).Dispatch();
                            break;
                        }
                    }
                }
                _uiNames.AddLast(uiName);
                _uiDataLinkedList.AddLast(uiData);
                _uiDataDictionary.Add(uiData.GetType(),uiData);
                _uiProxies.Add(uiName,UIProxy.Create(uiName,uiType,uiBase,uiData));
            }
            ReferencePool.Release(result);
        }

        private async Task GoBackUI(UIAction uiAction)
        {
            string uiName = uiAction.UIName;
            if (!_uiNames.Contains(uiName))
            {
                Log.Error($"不允许回退不存在的UI:{uiName}");
                return;
            }
            List<string> uiNames = new List<string>();
            for (LinkedListNode<string> node = _uiNames.Last; node != null; node = node.Previous)
            {
                if (node.Value.Equals(uiName))
                {
                    await UIAction.Create(node.Value, ActionType.ShowUI).Dispatch();
                    break;
                }
                CloseUI(node.Value);
                uiNames.Add(node.Value);
            }
            foreach (var name in uiNames)
            {
                _uiNames.Remove(name);
            }
        }

        private async Task CloseUI(UIAction uiAction)
        {
            string uiName = uiAction.UIName;
            if (!_uiNames.Contains(uiName))
            {
                Log.Error($"不允许关闭不存在的UI:{uiName}");
                return;
            }
            bool needSetOrder = !_uiNames.Last.Value.Equals(uiName);
            if (!needSetOrder)
            {
                for (LinkedListNode<string> node = _uiNames.Last.Previous; node != null; node = node.Previous)
                {
                    if (_uiProxies.TryGetValue(node.Value, out UIProxy uiProxy))
                    {
                        await UIAction.Create(node.Value, ActionType.ShowUI).Dispatch();
                        if (uiProxy.UIType == EUIType.Panel)
                        {
                            break;
                        }
                    }
                }
            }
            for (LinkedListNode<string> node = _uiNames.Last; node != null; node = node.Previous)
            {
                if (node.Value.Equals(uiName))
                {
                    if (CloseUI(node.Value))
                    {
                        break;
                    }
                }
            }
            _uiNames.Remove(uiName);
            if (needSetOrder)
                
            {
                int order = 0;
                foreach (IUIData uiData in _uiDataLinkedList)
                {
                    if (uiData.Order != order)
                    {
                        await UIAction.Create(uiData.UIName, ActionType.SetOrder).SetData(order).Dispatch();
                    }
                    order++;
                }
            }
        }
        
        private bool CloseUI(string uiName)
        {
            if (_uiProxies.TryGetValue(uiName, out UIProxy uiProxy))
            {
                _iUIHelper.CloseUI(uiProxy.UIBase, uiProxy.UIData);
                _uiDataLinkedList.Remove(uiProxy.UIData);
                _uiProxies.Remove(uiName);
                _uiDataDictionary.Remove(uiProxy.UIData.GetType());
                ReferencePool.Release(uiProxy);
                return true;
            }
            return false;
        }
    }
}