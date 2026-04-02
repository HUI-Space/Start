using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Start
{
    public partial class UIController : MonoBehaviour ,ISingleton
    {
        public static UIController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Main.Root.GetComponentInChildren<UIController>();
                    if (_instance == null)
                    {
                        _instance = new GameObject("UI").AddComponent<UIController>();
                        _instance.transform.SetParent(Main.Root);
                    }
                    SingletonMonoBehaviourController.Instance.RegisterSingleton(_instance);
                }
                return _instance;
            }
        }
        
        private static UIController _instance;
        
        public GameObject UIRoot { get; private set; }
        
        public Canvas UICanvas { get; private set; }
        
        public CanvasScaler UICanvasScaler { get; private set; }
        
        public GraphicRaycaster UICanvasRaycaster { get; private set; }
        
        public Camera UICamera { get; private set; }
        public EventSystem EventSystem { get; private set; }
        public Vector3 PanelScale => UICanvas.transform.localScale;

        public void Initialize()
        {
            //兼容性代码
            UICamera = GetComponentInChildren<Camera>();
            if (UICamera == null)
            {
                UICamera = new GameObject(nameof(UICamera)).AddComponent<Camera>();
                UICamera.clearFlags = CameraClearFlags.Depth;
                UICamera.cullingMask =  1 << LayerMask.NameToLayer("UI");
                UICamera.orthographic = true;
                UICamera.depth = 10;
                UICamera.transform.SetParent(this);
            }
            
            EventSystem = GetComponentInChildren<EventSystem>();
            if (EventSystem == null)
            {
                EventSystem = new GameObject(nameof(EventSystem)).AddComponent<EventSystem>();
                EventSystem.AddComponent<StandaloneInputModule>();
                EventSystem.transform.SetParent(this);
            }

            Transform uiRootTransform = transform.Find(nameof(UIRoot));
            if (uiRootTransform == null)
            {
                UIRoot = new GameObject(nameof(UIRoot),typeof(RectTransform));
                UIRoot.layer = LayerMask.NameToLayer("UI");
                UIRoot.transform.SetParent(this);
                UIRoot.transform.SetAsFirstSibling();
            }
            else
            {
                UIRoot = uiRootTransform.gameObject;
            }

            UICanvas = UIRoot.GetComponent<Canvas>();
            if (UICanvas == null )
            {
                UICanvas = UIRoot.AddComponent<Canvas>();
                UICanvas.renderMode = RenderMode.ScreenSpaceCamera;
                UICanvas.worldCamera = UICamera;
                UICanvas.planeDistance = 100;
            }
            
            UICanvasScaler = UIRoot.GetComponent<CanvasScaler>();
            if (UICanvasScaler == null)
            {
                UICanvasScaler = UIRoot.AddComponent<CanvasScaler>();
                UICanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                UICanvasScaler.referenceResolution = new Vector2(Screen.width,Screen.height);
                UICanvasScaler.matchWidthOrHeight = 0;
            }
            
            UICanvasRaycaster = UIRoot.GetComponent<GraphicRaycaster>();
            if (UICanvasRaycaster == null)
            {
                UICanvasRaycaster = UIRoot.AddComponent<GraphicRaycaster>();
            }
        }

        public void DeInitialize()
        {
            
        }
        
        public T InstantiateElement<T>(T elementOrigin, Transform parent) where T : UIElement
        {
            T newElement = Instantiate(elementOrigin, parent);
            newElement.gameObject.SetActive(true);
            List<IUIBase> uis = new List<IUIBase>();
            newElement.GetComponentsInChildren(uis);
            foreach (IUIBase uiBase in uis)
            {
                uiBase.Initialize();
            }
            return newElement;
        }
    }
    
    
    #region DynamicEffect
    
    public partial class UIController
    {
        private readonly Dictionary<string, Dictionary<string, StructTask>> _beforeDynamicEffects = new Dictionary<string, Dictionary<string, StructTask>>();

        private readonly Dictionary<string, Dictionary<string, StructTask>> _afterDynamicEffects = new Dictionary<string, Dictionary<string, StructTask>>();
        
        
        public void RegisterDynamicEffect(string uiName, string actionType, StructTask task , bool isBefore)
        {
            Dictionary<string, Dictionary<string, StructTask>> dynamicEffects = isBefore?_beforeDynamicEffects : _afterDynamicEffects;
            if (!dynamicEffects.TryGetValue(uiName,out Dictionary<string,StructTask> dictionary))
            {
                dictionary = new Dictionary<string, StructTask>();
                dynamicEffects[uiName] = dictionary;
            }
            dictionary[actionType] = task;
        }
        
        public async Task Before(UIAction action)
        {
            if (_afterDynamicEffects.TryGetValue(action.UIName, out Dictionary<string, StructTask> dictionary))
            {
                if (dictionary.TryGetValue(action.ActionType,out StructTask task))
                {
                    await task;
                    dictionary.Remove(action.ActionType);
                }
                if (dictionary.Count == 0)
                {
                    _afterDynamicEffects.Remove(action.UIName);
                }
            }
        }
        
        public async Task After(UIAction action)
        {
            if (_afterDynamicEffects.TryGetValue(action.UIName, out Dictionary<string, StructTask> dictionary))
            {
                if (dictionary.TryGetValue(action.ActionType,out StructTask task))
                {
                    await task;
                    dictionary.Remove(action.ActionType);
                }
                if (dictionary.Count == 0)
                {
                    _afterDynamicEffects.Remove(action.UIName);
                }
            }
        }
    }
    
    #endregion
}
