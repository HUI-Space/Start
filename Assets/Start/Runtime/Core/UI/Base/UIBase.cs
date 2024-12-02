using Start.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Start.Runtime
{
    public abstract class UIBase : MonoBehaviour, IUIBase
    {
        public virtual void Initialize()
        {
            
        }

        public virtual void DeInitialize()
        {
            
        }
    }

    [RequireComponent(typeof(Canvas),typeof(GraphicRaycaster),typeof(CanvasGroup))]
    public abstract class UIBase<T> : MonoBehaviour, IUIBase where T : UIData
    {
        private Canvas _canvas;
        private CanvasGroup _canvasGroup;
        private GraphicRaycaster _graphicRaycaster;
        
        public Canvas Canvas => _canvas ? _canvas : _canvas = GetComponent<Canvas>();
        public CanvasGroup CanvasGroup => _canvasGroup ? _canvasGroup : _canvasGroup = GetComponent<CanvasGroup>();
        public GraphicRaycaster GraphicRaycaster => _graphicRaycaster ? _graphicRaycaster : _graphicRaycaster = GetComponent<GraphicRaycaster>();

        public virtual void Initialize()
        {
            UIManager.Instance.AddListener<T>(OnUIDataChange);
        }

        private void OnUIDataChange(IUIData uiData)
        {
            T data = uiData as T;
            if (data == null)
            {
                Debug.LogError($"{typeof(T).Name} 转换为 {typeof(T)} 失败");
                return;
            }
            
            if (data.IsShow)
            {
                if (data.IsOnShow)
                {
                    GraphicRaycaster.enabled = true;
                    CanvasGroup.Switch(true);
                    OnShow(data);
                }
                Canvas.sortingOrder = (uiData.Order + 1) * 1000;
                Render(data);
            }
            else
            {
                if (data.IsOnHide)
                {
                    GraphicRaycaster.enabled = false;
                    CanvasGroup.Switch(false);
                    OnHide(data);
                }
            }
        }
        
        
        protected virtual void OnShow(T uiData)
        {
            
        }
        
        protected virtual void Render(T uiData)
        {
            
        }
        
        protected virtual void OnHide(T uiData)
        {
            
        }
        
        public virtual void DeInitialize()
        {
            UIManager.Instance.RemoveListener<T>(OnUIDataChange);
        }

        protected void OnClose()
        {
            if (UIManager.Instance.GetUIData<T>(out IUIData data))
            {
                UIAction.Create(data.UIName, ActionType.CloseUI).Dispatch();
            }
        }
    }
}