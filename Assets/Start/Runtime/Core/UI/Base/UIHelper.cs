using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Start.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Start.Runtime
{
    public class UIHelper:IUIHelper
    {
        private HashSet<string> _preloadUINames = new HashSet<string>();
        
        private readonly Dictionary<IUIBase,AsyncOperationHandle<GameObject>> _loadHandles = new Dictionary<IUIBase, AsyncOperationHandle<GameObject>>();
        
        public List<Middleware> GetMiddlewares()
        {
            List<Middleware> middlewares = new List<Middleware>();
            
            //按照需求顺序添加中间件
            middlewares.Add(UIManager.Instance.Middleware);
            
            middlewares.Add(UIManager.Instance.DefaultDispatch);
            return middlewares;
        }

        public async Task<GenericData<bool,EUIType,IUIBase, IUIData>> OpenUI(UIAction uiAction)
        {
            GenericData<bool,EUIType,IUIBase, IUIData> data = GenericData<bool,EUIType,IUIBase, IUIData>.Create();
            
            if (string.IsNullOrEmpty(uiAction.UIName))
            {
                return await Task.FromResult(data);
            }
            UIConfig uiConfig = ConfigManager.Instance.GetConfig<UIConfig>();
            UIConfigItem uiConfigItem = null;
            foreach (UIConfigItem item in uiConfig.DataList)
            {
                if (item.UIName == uiAction.UIName)
                {
                    uiConfigItem = item;
                    break;
                }
            }

            if (uiConfigItem == null)
            {
                return await Task.FromResult(data);
            }

            Type uiDataType = Type.GetType(uiConfigItem.UIDataName);
            if (uiDataType == null)
            {
                return await Task.FromResult(data);
            }
            IUIData uiData = Activator.CreateInstance(uiDataType) as IUIData;
            if (uiData == null)
            {
                return await Task.FromResult(data);
            }
            
            string uiAssetPath = "Assets/Data/UI/"+ uiConfigItem.UIName +".prefab";

            AsyncOperationHandle<GameObject> handle = ResourceManager.Instance.LoadAssetAsync<GameObject>(uiAssetPath);
            await handle.Task;
            
            GameObject uiPrefab = handle.Result;
            
            if (uiPrefab == null)
            {
                return await Task.FromResult(data);
            }
            
            GameObject go = Object.Instantiate(uiPrefab, UIRoot.Instance.Window);
            
            IUIBase uiBase = go.GetComponent<IUIBase>();
            IUIBase[] uiBases = go.GetComponentsInChildren<IUIBase>();
            
            uiData.Initialize();
            foreach (IUIBase ui in uiBases)
            {
                ui.Initialize();
            }
            
            _loadHandles.Add(uiBase,handle);
            
            data.SetData(true,uiConfigItem.euiType, uiBase, uiData);
            
            return await Task.FromResult(data);
        }

        public void CloseUI(IUIBase uiBase, IUIData uiData)
        {
            if (uiBase is MonoBehaviour monoBehaviour)
            {
                IUIBase[] uiBases = monoBehaviour.GetComponentsInChildren<IUIBase>();
                uiData.DeInitialize();
                foreach (IUIBase ui in uiBases)
                {
                    ui.DeInitialize();
                }
                Object.Destroy(monoBehaviour.gameObject);
                //释放
                if (_loadHandles.TryGetValue(uiBase, out AsyncOperationHandle<GameObject> handle)) ;
                {
                    ResourceManager.Instance.UnloadAsset(handle);
                    _loadHandles.Remove(uiBase);
                }
            }
        }
    }
}