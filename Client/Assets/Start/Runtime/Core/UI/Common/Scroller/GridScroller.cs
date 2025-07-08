using System;
using UnityEngine;

namespace Start
{
    public class GridScroller : ScrollBase
    {
        public RectTransform.Axis StartAxis;
        public Vector2 Size;
        public Vector2 Spacing;
        public RectOffset Padding;
        
        private int _verticalCount;//垂直数量
        private int _horizontalCount;//水平数量
        private int _verticalIndex;
        private int _horizontalIndex;
        private int _startIndex;
        private int _endIndex;


        public override void Initialize()
        {
            base.Initialize();
            _scrollRect.horizontal = StartAxis != RectTransform.Axis.Horizontal;
            _scrollRect.vertical = StartAxis != RectTransform.Axis.Vertical;
        }

        public override void SetElementUI(UIElement prefab, Action<UIElement, int> renderCell = null, GetSizeAlongDirection getSizeAlongDirection = null)
        {
            base.SetElementUI(prefab, renderCell, getSizeAlongDirection);
            if (Size.x == 0)
            {
                Size.x = PrefabRect.width;
            }
            if (Size.y == 0)
            {
                Size.y = PrefabRect.height;
            }
        }

        public override float GetDefaultSize()
        {
            return StartAxis == RectTransform.Axis.Horizontal ? Size.y : Size.x;
        }

        public override void ScrollToIndex(int index)
        {
            if (index < 0)
            {
                index = 0;
            }
            else if (index >= ChildCount)
            {
                index = ChildCount - 1;
            }
            if (StartAxis == RectTransform.Axis.Horizontal)
            {
                index /= _horizontalCount;
                float offset = Padding.top;
                for (int i = 0; i < index ; i++)
                {
                    offset += Size.y + Spacing.y;
                }
                ScrollToPosition(offset);
            }
            else
            {
                index /= _verticalCount;
                float offset = Padding.left;
                for (int i = 0; i < index; i++)
                {
                    offset += Size.x + Spacing.x;
                }
                ScrollToPosition(-offset);
            }
        }

        public override void ScrollToPosition(float position)
        {
            Content.localPosition = StartAxis == RectTransform.Axis.Horizontal
                ? new Vector3(0, position, 0)
                : new Vector3(position, 0, 0);
            base.ScrollToPosition(position);
        }

        protected override void ResizeContent()
        {
            
            CellOffsetX = Padding.left + PrefabRect.width / 2;
            CellOffsetY = - Padding.top - PrefabRect.height / 2;
            //计算个数
            _verticalCount = 0;
            _horizontalCount = 0;
            if (StartAxis == RectTransform.Axis.Horizontal)
            {
                float offset = ScrollRect.width - Padding.left - Padding.right;
                _horizontalCount = (int) Mathf.Floor(offset / (Size.x + Spacing.x));
                _verticalCount = (int) Mathf.Ceil(ChildCount / (float)_horizontalCount);
                ContentSize = Size.y * _verticalCount + Spacing.y * (_verticalCount - 1) + Padding.top + Padding.bottom;
                Content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,ContentSize);
            }
            else
            {
                float offset = ScrollRect.height - Padding.top - Padding.bottom;
                _verticalCount = (int) Mathf.Floor(offset / (Size.y + Spacing.y));
                _horizontalCount = (int) Mathf.Ceil( ChildCount / (float)_verticalCount);
                ContentSize = Size.x * _horizontalCount + Spacing.x * (_horizontalCount - 1) + Padding.left + Padding.right;
                Content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,ContentSize);
            }
        }

        protected override void Reload()
        {
            if (StartAxis == RectTransform.Axis.Horizontal)
            {
                ReloadHorizontal();
            }
            else
            {
                ReloadVertical();
            }
        }

        private void ReloadHorizontal()
        {
            ScrollStartPosition = Content.localPosition.y;
            ScrollEndPosition = ScrollStartPosition + ScrollRect.height >= ContentSize
                ? ContentSize : ScrollStartPosition + ScrollRect.height;
            CellSpacing = Spacing.y;
            Offset = Padding.left;
            
            _startIndex = 0;
            _endIndex = _verticalCount - 1;
            for (int i = 0; i < _verticalCount; i++)
            {
                Offset += GetDefaultSize();
                if (Offset < ScrollStartPosition)
                {
                    _startIndex = i;
                }
                if (Offset >= ScrollEndPosition)
                {
                    _endIndex = i;
                    break;
                }
                Offset += CellSpacing;
            }
            for (int i = _startIndex; i < _endIndex + 1; i++)
            {
                for (int j = 0; j < _horizontalCount; j++)
                {
                    Index = j + i * _horizontalCount;
                    if (Index > ChildCount -1)
                    {
                        break;
                    }
                    UIElement uiElement = GetActiveElements(Index);
                    float x = CellOffsetX + j * (Size.x + Spacing.x);
                    float y = CellOffsetY - i * (Size.y + Spacing.y);
                    uiElement.transform.localPosition = new Vector3(x, y, 0);
                }
            }
        }

        private void ReloadVertical()
        {
            ScrollStartPosition = -Content.localPosition.x;
            ScrollEndPosition = ScrollStartPosition + ScrollRect.width >= ContentSize
                ? ContentSize : ScrollStartPosition + ScrollRect.width;
            CellSpacing = Spacing.x;
            Offset = Padding.top;
            
            _startIndex = 0;
            _endIndex = _horizontalCount - 1;
            for (int i = 0; i < _horizontalCount; i++)
            {
                Offset += GetDefaultSize();
                if (Offset < ScrollStartPosition)
                {
                    _startIndex = i;
                }
                if (Offset >= ScrollEndPosition)
                {
                    _endIndex = i;
                    break;
                }
                Offset += CellSpacing;
            }
            
            for (int i = _startIndex; i < _endIndex + 1; i++)
            {
                for (int j = 0; j < _verticalCount; j++)
                {
                    Index = j + i * _verticalCount;
                    if (Index > ChildCount -1)
                    {
                        break;
                    }
                    UIElement uiElement = GetActiveElements(Index);
                    float x = CellOffsetX + i * (Size.x + Spacing.x);
                    float y = CellOffsetY - j * (Size.y + Spacing.y);
                    uiElement.transform.localPosition = new Vector3(x, y, 0);
                }
            }
        }
    }
}