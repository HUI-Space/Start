using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Start
{
    public partial class UIWindow : UIModuleBase<UIWindow>
    {
        public event UIMiddleware BeforeReceive;
        public event UIMiddleware AfterReceive;

        private LinkedList<string> _uiNames;
        private Dictionary<string, UIProxy> _uiProxies;
        private Dictionary<string, PriorityDelegate<IUIData>> _uiCallbacks;
        private Dictionary<IUIBase, AsyncOperationHandle<GameObject>> _loadHandles;

        protected override int Priority => 0;

        public override void Initialize()
        {
            base.Initialize();
            _uiNames = new LinkedList<string>();
            _uiProxies = new Dictionary<string, UIProxy>();
            _uiCallbacks = new Dictionary<string, PriorityDelegate<IUIData>>();
            _loadHandles = new Dictionary<IUIBase, AsyncOperationHandle<GameObject>>();
        }
        
        public override async Task UIMiddleware(UIAction action)
        {
            //关闭所有UI
            await CloseAllUI(action);
            // 判断是否为空作为拦截避免不必要的性能浪费（此处中间件都需要以UI名称作为前提）
            if (string.IsNullOrEmpty(action.UIName))
            {
                return;
            }
            //加载预制体 并初始化
            await OpenUI(action);
            //派发UIAction到UIData 之前
            await BeforeReceiveUIData(action);
            //UI管理派发消息
            await ReceiveUIData(action);
            //派发UIAction到UIData 之后
            await AfterReceiveUIData(action);
            //回退UI
            await BackUI(action);
            //关闭UI 并销毁
            await CloseUI(action);
        }

        public override void DeInitialize()
        {
            ClearUIProxy();
            ClearEvent();
        }
        
        /// <summary>
        /// 更新UI
        /// </summary>
        public void UpdateUIBase(IUIData uiData)
        {
            //刷新UI
            uiData.BeforeClearDirty();
            if (_uiCallbacks.TryGetValue(uiData.GetType().Name, out PriorityDelegate<IUIData> listeners))
            {
                listeners.Invoke(uiData);
            }
            uiData.ClearDirty();
        }

        private async Task OpenUI(UIAction uiAction)
        {
            if (uiAction.ActionType != UIActionTypes.Open)
            {
                return;
            }

            //打开UI
            string uiName = uiAction.UIName;
            if (_uiNames.Contains(uiName))
            {
                Logger.Error($"不允许打开同一个UI:{uiName}");
                return;
            }

            UIProxy proxy = await OpenUI(uiName);
            // 如果当前UI是全屏UI,则需要隐藏上一个全屏UI
            if (proxy.UIType == EUIType.Panel)
            {
                for (LinkedListNode<string> node = _uiNames.Last; node != null; node = node.Previous)
                {
                    if (_uiProxies.TryGetValue(node.Value, out UIProxy uiProxy) && uiProxy.UIType == EUIType.Panel)
                    {
                        await UIAction.Create(node.Value, UIActionTypes.Hide).Dispatch();
                        break;
                    }
                }
            }

            _uiNames.AddLast(uiName);
            _uiProxies.Add(uiName, proxy);
            proxy.UIData.Order = _uiNames.Count;
            proxy.UIData.Initialize();
            foreach (IUIBase ui in proxy.AllUIBases)
            {
                ui.Initialize();
            }
        }

        private async Task BeforeReceiveUIData(UIAction action)
        {
            if (BeforeReceive != null)
            {
                foreach (var @delegate in BeforeReceive.GetInvocationList())
                {
                    UIMiddleware uiMiddleware = (UIMiddleware)@delegate;
                    Task task = uiMiddleware?.Invoke(action);
                    if (task != null)
                    {
                        await task;
                    }
                }
            }
        }

        /// <summary>
        /// 派发UIAction 到 UIData 只支持 UIAction.UIName 等于 UIAction.UIName 的UI
        /// 作用是单一职责，避免UI逻辑和UI管理器耦合
        /// </summary>
        /// <param name="action"></param>
        private Task ReceiveUIData(UIAction action)
        {
            if (_uiNames.Contains(action.UIName) && _uiProxies.TryGetValue(action.UIName, out UIProxy uiProxy))
            {
                uiProxy.UIData.ReceiveAction(action);
            }

            return Task.CompletedTask;
        }

        private async Task AfterReceiveUIData(UIAction action)
        {
            if (AfterReceive != null)
            {
                foreach (var @delegate in AfterReceive.GetInvocationList())
                {
                    UIMiddleware uiMiddleware = (UIMiddleware)@delegate;
                    Task task = uiMiddleware?.Invoke(action);
                    if (task != null)
                    {
                        await task;
                    }
                }
            }
        }

        private async Task BackUI(UIAction uiAction)
        {
            if (uiAction.ActionType != UIActionTypes.Back)
            {
                return;
            }

            if (!_uiNames.Contains(uiAction.UIName))
            {
                Logger.Error($"{uiAction.UIName} is not exist");
                return;
            }

            string uiName = uiAction.UIName;
            if (!_uiNames.Contains(uiName))
            {
                Logger.Error($"不允许回退不存在的UI:{uiName}");
                return;
            }

            List<string> uiNames = new List<string>();
            for (LinkedListNode<string> node = _uiNames.Last; node != null; node = node.Previous)
            {
                if (node.Value.Equals(uiName))
                {
                    await UIAction.Create(node.Value, UIActionTypes.Show).Dispatch();
                    break;
                }

                CloseUI(node.Value);
                uiNames.Add(node.Value);
            }

            foreach (var item in uiNames)
            {
                _uiNames.Remove(item);
            }
        }

        private async Task CloseUI(UIAction uiAction)
        {
            if (uiAction.ActionType != UIActionTypes.Close)
            {
                return;
            }

            if (!_uiNames.Contains(uiAction.UIName))
            {
                Logger.Error($"{uiAction.UIName} is not exist");
                return;
            }

            string uiName = uiAction.UIName;
            if (!_uiNames.Contains(uiName))
            {
                Logger.Error($"不允许关闭不存在的UI:{uiName}");
                return;
            }

            bool needSetOrder = !_uiNames.Last.Value.Equals(uiName);
            if (!needSetOrder)
            {
                for (LinkedListNode<string> node = _uiNames.Last.Previous; node != null; node = node.Previous)
                {
                    if (_uiProxies.TryGetValue(node.Value, out UIProxy uiProxy))
                    {
                        await UIAction.Create(node.Value, UIActionTypes.Show).Dispatch();
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
                int order = 1;
                foreach (string ui in _uiNames)
                {
                    if (_uiProxies.TryGetValue(ui, out UIProxy uiProxy))
                    {
                        IUIData uiData = uiProxy.UIData;
                        if (uiData.Order != order)
                        {
                            await UIAction.Create(uiData.UIName, UIActionTypes.SetOrder).SetData(order).Dispatch();
                        }

                        order++;
                    }
                }
            }
        }

        
        private bool CloseUI(string uiName)
        {
            if (_uiProxies.TryGetValue(uiName, out UIProxy uiProxy))
            {
                uiProxy.UIData.DeInitialize();
                foreach (IUIBase ui in uiProxy.AllUIBases)
                {
                    ui.DeInitialize();
                }

                IUIBase uiBase = uiProxy.UIBase;
                if (uiBase is MonoBehaviour monoBehaviour)
                {
                    Destroy(monoBehaviour.gameObject);
                    //释放
                    if (_loadHandles.TryGetValue(uiBase, out AsyncOperationHandle<GameObject> handle))
                    {
                        ResourceManager.Instance.Unload(handle);
                        _loadHandles.Remove(uiBase);
                    }
                }
                RecyclableObjectPool.Recycle(uiProxy);
                _uiProxies.Remove(uiName);
                return true;
            }

            return false;
        }
        
        private async Task<UIProxy> OpenUI(string uiName)
        {
            UIConfig uiConfig = ConfigManager.Instance.GetConfig<UIConfig>();
            if (!uiConfig.GetUIConfigItem(uiName, out UIConfigItem uiConfigItem))
            {
                throw new Exception("没有找到UI配置");
            }

            Type uiDataType = Type.GetType(uiConfigItem.UIDataName);
            if (uiDataType == null)
            {
                throw new Exception("UIDataType 为空");
            }
            IUIData uiData = Activator.CreateInstance(uiDataType) as IUIData;
            if (uiData == null)
            {
                throw new Exception("IUIData 为空");
            }

            string uiAssetPath = AssetConfig.GetAssetPath(EAssetType.UI, uiConfigItem.UIName + AssetConfig.Prefab);

            AsyncOperationHandle<GameObject> handle = ResourceManager.Instance.LoadAssetAsync<GameObject>(uiAssetPath);
            await handle.Task;

            GameObject uiPrefab = handle.Result;
            GameObject go = Instantiate(uiPrefab, this.transform);
            IUIBase uiBase = go.GetComponent<IUIBase>();
            IUIBase[] uiBases = go.GetComponentsInChildren<IUIBase>();
            _loadHandles.Add(uiBase, handle);
            return await Task.FromResult(UIProxy.Create(uiName, uiConfigItem.UIType, uiBase, uiData, uiBases));
        }
        
        private async Task CloseAllUI(UIAction uiAction)
        {
            if (uiAction.ActionType != UIActionTypes.CloseAll)
            {
                return;
            }

            while (_uiNames.Count != 0)
            {
                await UIActions.CloseUI(_uiNames.Last.Value);
            }
        }
    }
    
    #region API --- UIProxy
    public partial class UIWindow
    {
        public bool GetUIData<T>(out T uiData) where T : IUIData
        {
            uiData = default;
            foreach (UIProxy uiProxy in _uiProxies.Values)
            {
                if (uiProxy.UIDataName == typeof(T).Name)
                {
                    uiData = (T)uiProxy.UIData;
                    return true;
                }
            }
            return false;
        }
        
        public bool GetUIData(string uiDataName,out IUIData uiData)
        {
            uiData = default;
            foreach (UIProxy uiProxy in _uiProxies.Values)
            {
                if (uiProxy.UIDataName == uiDataName)
                {
                    uiData = uiProxy.UIData;
                    return true;
                }
            }
            return false;
        }

        private void ClearUIProxy()
        {
            _uiNames.Clear();
            foreach (var item in _uiProxies)
            {
                RecyclableObjectPool.Recycle(item.Value);
            }
            _uiProxies.Clear();
        }
    }

    #endregion
    
    #region API --- Event
    
    public partial class UIWindow
    {
        
        public void AddListener<T>(Action<IUIData> callBack, int priority = 0) where T :IUIData
        {
            string uiDataName = typeof(T).Name;
            AddListener(uiDataName,callBack,priority);
        }

        public void RemoveListener<T>(Action<IUIData> callBack) where T : IUIData
        {
            string uiDataName = typeof(T).Name;
            RemoveListener(uiDataName, callBack);
        }

        public void AddListener(string uiDataName, Action<IUIData> callBack, int priority = 0)
        {
            if (!_uiCallbacks.TryGetValue(uiDataName, out PriorityDelegate<IUIData> callbacks))
            {
                callbacks =  PriorityDelegate<IUIData>.Create();
                _uiCallbacks.Add(uiDataName, callbacks);
            }
            callbacks.AddListener(callBack, priority);
        }

        public void RemoveListener(string uiDataName, Action<IUIData> callBack)
        {
            if (_uiCallbacks.TryGetValue(uiDataName, out PriorityDelegate<IUIData> callbacks))
            {
                callbacks.RemoveListener(callBack);
                if (callbacks.CanBeReleased)
                {
                    _uiCallbacks.Remove(uiDataName);
                    RecyclableObjectPool.Recycle(callbacks);
                }
            }
        }

        private void ClearEvent()
        {
            foreach (var item in _uiCallbacks)
            {
                RecyclableObjectPool.Recycle(item.Value);
            }
            _uiCallbacks.Clear();
        }
    }
    
    #endregion
}