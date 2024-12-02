using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Start.Runtime
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ScrollRect), typeof(RectMask2D))]
    public class Scroller : UIBase
    {
        public Action<int, ElementUI> SetValue;
        public Action<int, ElementUI> Complete;
        public Action<int, ElementUI> PlayAnim;
        
        public ElementUI Prefab;
        public RectOffset Padding;
        public float Spacing;
        
        private ScrollRect _scroll;//滚动组件 即滚动区域
        private Rect _prefabRect;//预制体的尺寸
        private Rect _scrollRect;//ScrollRect的尺寸 (遮罩的尺寸)
        
        private int _childCount;//子物的体数量
        private int _displayCount;//显示的数量
        
        private float _contentHeight;//容器大小
        private float _scrollPosition;//滚动的到位置
        private float _lastScrollPosition;// 上次滚动的位置
        
        private bool _isDirty;
        
        private LinkedList<ElementUI> _freeElements = new LinkedList<ElementUI>();
        private Queue<ElementUI> _activeElements = new Queue<ElementUI>();
        
        private void Awake()
        {
            Initialize();
        }
        public override void Initialize()
        {
            Prefab.gameObject.SetActive(false);
            // 创建一个容器
            GameObject go = new GameObject("Content", typeof(RectTransform));
            RectTransform content = go.GetComponent<RectTransform>();
            content.SetParent(transform);
            content.pivot = Vector2.up;
            content.anchorMin = Vector2.up;
            content.anchorMax = Vector2.up;
            content.anchoredPosition = Vector2.zero;
            content.sizeDelta = new Vector2(_scrollRect.width, _scrollRect.width);
            
            _scroll = GetComponent<ScrollRect>();
            _scroll.onValueChanged.AddListener(OnScrollValueChanged);
            _prefabRect = Prefab.GetComponent<RectTransform>().rect;
            _scrollRect = _scroll.GetComponent<RectTransform>().rect;
            _displayCount = 2 + Mathf.CeilToInt(_scrollRect.height / _prefabRect.height);
            _scroll.content = content;
            
            Image image = go.AddComponent<Image>();
            image.color = Color.cyan;
        }
        public override void DeInitialize()
        {
            _scroll.onValueChanged.RemoveListener(OnScrollValueChanged);
            _scroll = null;
        }
        
        private void OnScrollValueChanged(Vector2 vector2)
        {
            if (_childCount == 0)
            {
                return;
            }
            _lastScrollPosition = vector2.y;
            _scrollPosition = (1 - vector2.y) * _scrollRect.height;
            _scrollPosition = Mathf.Clamp(_scrollPosition, 0, _contentHeight);
            _isDirty = true;
        }
        
        public void SetData(int count)
        {
            if (_childCount != count)
            {
                _childCount = count;
                _contentHeight = (_prefabRect.height + Spacing) * _childCount - Spacing + Padding.top + Padding.bottom;
                _scroll.content.sizeDelta = new Vector2(_scrollRect.width, _contentHeight);
                _isDirty = true;
            }
        }
        
        public void Update()
        {
            if (_isDirty)
            {
                _isDirty = false;
                float start = _scrollPosition;
                float end = _scrollPosition + _scrollRect.height;
                int index =_childCount - Mathf.FloorToInt((_contentHeight - _scrollPosition - Padding.top) / (_prefabRect.height + Spacing));
                float offset = (_scrollPosition - Padding.top) % (_prefabRect.y + Spacing);
                print($"start:{start}  end:{end}  index :{index} offset:{offset}");
                
            }
        }

        public void ScrollToIndex(int index)
        {
            if (index > _childCount - 1)
            {
                index = _childCount - 1;
            }
            _scrollPosition = (_prefabRect.height + Spacing) * (index + 1) - Spacing + Padding.top;
            ScrollToPosition(_scrollPosition);
        }

        public void ScrollToPosition(float position)
        {
            
        }
        
    }
}