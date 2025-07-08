using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Start
{
    [RequireComponent(typeof(RectTransform),typeof(Canvas),typeof(GraphicRaycaster))]
    public abstract class UIModuleBase<T> : MonoBehaviour ,ISingleton where T : UIModuleBase<T>
    {
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = UIController.Instance.UIRoot.GetComponentInChildren<T>();
                    if (_instance == null)
                    {
                        _instance = new GameObject(nameof(T)).AddComponent<T>();
                        _instance.transform.SetParent(UIController.Instance.UIRoot);
                    }
                    SingletonMonoBehaviourController.Instance.RegisterSingleton(_instance);
                }
                return _instance;
            }
        }
        
        private static T _instance;
        
        protected abstract int Priority { get; }
        
        private Canvas _canvas;

        public virtual void Initialize()
        {
            gameObject.layer = LayerMask.NameToLayer("UI");
            _canvas = GetComponent<Canvas>();
            _canvas.overrideSorting = true;
            _canvas.sortingOrder = Priority * 10000;
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.localScale =  Vector3.one;
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localPosition = Vector3.zero;
            rectTransform.offsetMin = Vector2.zero; // Left and Bottom
            rectTransform.offsetMax = Vector2.zero; // Right and Top
            rectTransform.SetSiblingIndex(Priority);
        }

        public abstract Task UIMiddleware(UIAction action);

        public virtual void DeInitialize()
        {
            
        }
    }
}