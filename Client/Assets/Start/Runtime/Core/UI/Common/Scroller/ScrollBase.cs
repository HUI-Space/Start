using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Start
{
#if UNITY_EDITOR
    public abstract partial class ScrollBase
    {
        public void Preview(GameObject prefab)
        {
            if (prefab == null)
            {
                return;
            }
            EndPreview();
            _prefabGameObject = prefab;
            PrefabRect = prefab.GetComponent<RectTransform>().rect;
            Initialize();
            SetCount(100);
            GetActiveElements();
            Reload();
            ReleaseElements();
        }

        public void EndPreview()
        {
            _objectPool.Clear();
            _activeElements.Clear();
            _lastActiveElements.Clear();
            _prefabGameObject = default;
            if (_scrollRect != null && _scrollRect.viewport != null)
            {
                DestroyImmediate(_scrollRect.viewport.gameObject);
            }
        }
    }
#endif
    
    public delegate float GetSizeAlongDirection(int cellIndex);
    public abstract partial class ScrollBase : UIElement
    {
        private UIElement _prefab;//预制体
        private bool _isDirty;//是否需要刷新
        private Action<UIElement,int> _renderCell;
        private GameObject _prefabGameObject;//预制体的游戏物体
        protected ScrollRect _scrollRect;//滚动组件 即滚动区域
        
        protected int Index;//索引
        protected float End;//结束
        protected float Offset;//偏移
        protected float CellSpacing;//间隙
        protected float CellOffsetX;//格子尺寸偏移X
        protected float CellOffsetY;//格子尺寸偏移Y
        
        protected float ContentSize;//容器大小   
        protected float ScrollStartPosition;//滚动的到遮罩起始位置
        protected float ScrollEndPosition;//滚动的到遮罩结束位置
        
        protected int ChildCount;//子物的体数量
        protected Rect PrefabRect;//预制体的尺寸
        protected Rect ScrollRect;//Scroll的尺寸 
        protected RectTransform Content;//容器
        protected GetSizeAlongDirection GetSizeAlongDirection;

        private readonly Queue<UIElement> _objectPool = new Queue<UIElement>();
        private readonly Dictionary<int, UIElement> _activeElements = new Dictionary<int, UIElement>();
        private readonly Dictionary<int, UIElement> _lastActiveElements = new Dictionary<int, UIElement>();
        
        public override void Initialize()
        {
            _scrollRect = this.GetOrAddComponent<ScrollRect>();
            _scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
            ScrollRect = _scrollRect.GetComponent<RectTransform>().rect;
            
            // 创建一个Viewport
            RectTransform viewport = _scrollRect.viewport;
            if (viewport == null)
            {
                viewport = new GameObject("Viewport", typeof(RectTransform)).GetComponent<RectTransform>();
                _scrollRect.viewport = viewport;
            }
            viewport.SetParent(transform);
            viewport.pivot = Vector2.up;
            viewport.anchorMin = Vector2.zero;
            viewport.anchorMax = Vector2.one;
            viewport.localScale = Vector3.one;
            viewport.anchoredPosition3D = Vector3.zero;
            viewport.SetSizeWithCurrentAnchors( ScrollRect.width,ScrollRect.height);
            
            // 创建一个容器
            Content = _scrollRect.content;
            if (Content == null)
            {
                Content = new GameObject("Content", typeof(RectTransform)).GetComponent<RectTransform>();
                _scrollRect.content = Content;
            }
            Content.SetParent(_scrollRect.viewport);
            Content.pivot = Vector2.up;
            Content.anchorMin = Vector2.up;
            Content.anchorMax = Vector2.up;
            Content.localScale = Vector3.one;
            Content.anchoredPosition3D = Vector3.zero;
            Content.SetSizeWithCurrentAnchors(ScrollRect.width,ScrollRect.height);
        }
        
        public virtual void SetElementUI(UIElement prefab, Action<UIElement,int> renderCell = null, 
            GetSizeAlongDirection getSizeAlongDirection = null)
        {
            _prefab = prefab;
            _renderCell = renderCell;
            _prefabGameObject = _prefab.gameObject;
            GetSizeAlongDirection = getSizeAlongDirection;
            PrefabRect = prefab.GetComponent<RectTransform>().rect;
            _prefab.gameObject.SetActive(false);
        }

        public override void DeInitialize()
        {
            _scrollRect.onValueChanged.RemoveListener(OnScrollValueChanged);
            _prefab = default;
            _isDirty = default;
            _scrollRect = default;
            _renderCell = default;
            _prefabGameObject = default;
            Index = default;
            End = default;
            Offset = default;
            CellSpacing = default;
            CellOffsetX = default;
            CellOffsetY = default;
            ContentSize = default;
            ScrollStartPosition = default;
            ScrollEndPosition = default;
            ChildCount = default;
            PrefabRect = default;
            ScrollRect = default;
            Content = default;
            GetSizeAlongDirection = default;
            foreach (UIElement elementUI in _objectPool)
            {
                elementUI.DeInitialize();
                Destroy(elementUI.gameObject);
            }
            foreach (UIElement elementUI in _activeElements.Values)
            {
                elementUI.DeInitialize();
                Destroy(elementUI.gameObject);
            }
            foreach (UIElement elementUI in _lastActiveElements.Values)
            {
                elementUI.DeInitialize();
                Destroy(elementUI.gameObject);
            }
            _objectPool.Clear();
            _activeElements.Clear();
            _lastActiveElements.Clear();
        }
        
        
        public void SetCount(int count)
        {
            if (_prefabGameObject == null)
            {
                throw new ArgumentNullException(nameof(_prefabGameObject));
            }
            ChildCount = count;
            ResizeContent();
            _isDirty = true;
        }
        
        public abstract float GetDefaultSize();

        public abstract void ScrollToIndex(int index);

        public virtual void ScrollToPosition(float position)
        {
            _isDirty = true;
        }
        
        protected abstract void ResizeContent();
        
        protected abstract void Reload();

        protected UIElement GetActiveElements(int index)
        {
            if (_lastActiveElements.TryGetValue(index,out UIElement element))
            {
                _lastActiveElements.Remove(index);
            }
            else
            {
                if (_objectPool.Count > 0)
                {
                    element = _objectPool.Dequeue();
                }
                else
                {
                    element = Instantiate(_prefabGameObject, Content).GetComponent<UIElement>();
                    element.Initialize();
                }
            }
            _renderCell?.Invoke(element,index);
            _activeElements[index] = element;
            element.name = index.ToString();
            element.gameObject.SetActive(true);
            element.transform.SetSiblingIndex(index);
            return element;
        }
        
        private void Update()
        {
            if (_isDirty)
            {
                _isDirty = false;
                GetActiveElements();
                Reload();
                ReleaseElements();
            }
        }
        
        private void OnScrollValueChanged(Vector2 vector2)
        {
            if (ChildCount == 0)
            {
                return;
            }
            _isDirty = true;
        }
        
        private void GetActiveElements()
        {
            foreach (var item in _activeElements)
            {
                _lastActiveElements.Add(item.Key, item.Value);
            }
            _activeElements.Clear();
        }

        private void ReleaseElements()
        {
            foreach (var item in _lastActiveElements)
            {
                item.Value.DeInitialize();
                item.Value.gameObject.SetActive(false);
                _objectPool.Enqueue(item.Value);
            }
            _lastActiveElements.Clear();
        }
    }
}