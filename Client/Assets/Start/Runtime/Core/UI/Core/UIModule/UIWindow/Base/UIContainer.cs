using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Start
{
    [RequireComponent(typeof(Canvas), typeof(GraphicRaycaster), typeof(CanvasGroup))]
    public abstract class UIContainer<T> : UIBase where T : UIData
    {
        private Canvas _canvas;
        private CanvasGroup _canvasGroup;
        private GraphicRaycaster _graphicRaycaster;
        public Canvas Canvas => _canvas ? _canvas : _canvas = GetComponent<Canvas>();
        public CanvasGroup CanvasGroup => _canvasGroup ? _canvasGroup : _canvasGroup = GetComponent<CanvasGroup>();

        public GraphicRaycaster GraphicRaycaster
            => _graphicRaycaster ? _graphicRaycaster : _graphicRaycaster = GetComponent<GraphicRaycaster>();
        
        public override void Initialize()
        {
            UIWindow.Instance.AddListener<T>(OnUIDataChange);
        }
        
        public override void DeInitialize()
        {
            UIWindow.Instance.RemoveListener<T>(OnUIDataChange);
        }

        private void OnUIDataChange(IUIData uiData)
        {
            T data = uiData as T;
            if (data == null)
            {
                Logger.Error($"{typeof(T).Name} 转换为 {typeof(T)} 失败");
                return;
            }
            
            if (data.IsShow)
            {
                if (data.IsOnShow)
                {
                    Canvas.overrideSorting = true;
                    GraphicRaycaster.enabled = true;
                    CanvasGroup.Switch(true);
                    Show(data);
                }
                Canvas.sortingOrder = uiData.Order * 1000;
                Render(data);
            }
            else
            {
                if (data.IsOnHide)
                {
                    GraphicRaycaster.enabled = false;
                    CanvasGroup.Switch(false);
                    Hide(data);
                }
            }
        }
        
        protected virtual void Show(T uiData)
        {
        }
        
        protected virtual void Render(T uiData)
        {
        }
        
        protected virtual void Hide(T uiData)
        {
        }

        protected void Close()
        {
            if (UIWindow.Instance.GetUIData<T>(out T data))
            {
                UIAction.Create(data.UIName, UIActionTypes.Close).Dispatch();
            }
        }
    }
}