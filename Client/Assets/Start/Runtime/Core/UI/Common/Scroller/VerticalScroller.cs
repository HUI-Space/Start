using System;
using System.Collections.Generic;
using UnityEngine;

namespace Start
{
    public class VerticalScroller : ScrollBase
    {
        public RectOffset Padding;
        public float Spacing;
        private Dictionary<int,float> _sizeCache;
        
        public override void SetElementUI(UIElement prefab, Action<UIElement, int> renderCell = null, GetSizeAlongDirection getSizeAlongDirection = null)
        {
            base.SetElementUI(prefab, renderCell, getSizeAlongDirection);
            if (getSizeAlongDirection != null)
            {
                _sizeCache = new Dictionary<int, float>();
            }
        }

        public override void DeInitialize()
        {
            base.DeInitialize();
            if (_sizeCache != null)
            {
                _sizeCache.Clear();
            }
            _sizeCache = null;
        }
        
        public override float GetDefaultSize()
        {
            return PrefabRect.height;
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
            float offset = Padding.top;
            if (GetSizeAlongDirection != null)
            {
                for (int i = 0; i < index; i++)
                {
                    offset += _sizeCache[i] + Spacing;
                }
            }
            else
            {
                offset += (GetDefaultSize() + Spacing) * index;
            }
            ScrollToPosition(offset);
        }

        public override void ScrollToPosition(float position)
        {
            Content.localPosition = new Vector3(0, position, 0); 
            base.ScrollToPosition(position);
        }
        
        protected override void ResizeContent()
        {
            CellOffsetX = Padding.left + PrefabRect.width / 2;
            //CellOffsetY = GetSize(1) / 2;
            ContentSize = Padding.top + Padding.bottom + ChildCount * Spacing - Spacing;
            if (GetSizeAlongDirection != null)
            {
                for (int i = 0; i < ChildCount; i++)
                {
                    float size = GetSizeAlongDirection(i);
                    ContentSize += size;
                    _sizeCache[i] = size;
                }
            }
            else
            {
                ContentSize += ChildCount * GetDefaultSize();
            }
            Content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,ContentSize);
        }

        protected override void Reload()
        {
            ScrollStartPosition = Content.localPosition.y;
            ScrollEndPosition = ScrollStartPosition + ScrollRect.height >= ContentSize
                ? ContentSize : ScrollStartPosition + ScrollRect.height;
            End = 0;
            CellSpacing = Spacing;
            Offset = Padding.top;

            if (GetSizeAlongDirection != null)
            {
                for (Index = 0; Index < ChildCount; Index++)
                {
                    Offset += _sizeCache[Index];
                    if (Offset >= ScrollStartPosition)
                    {
                        break;
                    }
                    Offset += Spacing;
                }
                
                for (int i = Index; i < ChildCount; i++)
                {
                    UIElement uiElement = GetActiveElements(i);
                    float cellY = _sizeCache[i];
                    uiElement.transform.localPosition = new Vector3(CellOffsetX,cellY- Offset - cellY / 2,0);
                    if (i == ChildCount - 1)
                    {
                        End = Padding.right;
                        CellSpacing = 0;
                    }
                    if (Offset >= ScrollEndPosition - End)
                    {
                        break;
                    }
                    Offset += _sizeCache[Index] + CellSpacing;
                    Index++;
                }
            }
            else
            {
                float size = GetDefaultSize();
                float halfSize = size / 2;
                Index = Mathf.FloorToInt(ScrollStartPosition / (size + Spacing));
                if (Index < 0)
                {
                    Index = 0;
                }
                Offset += (Index + 1) * size + Index * Spacing;
                for (int i = Index; i < ChildCount; i++)
                {
                    UIElement uiElement = GetActiveElements(i);
                    uiElement.transform.localPosition = new Vector3(CellOffsetX,size - Offset - halfSize,0);
                    if (i == ChildCount - 1)
                    {
                        End = Padding.right;
                        CellSpacing = 0;
                    }
                    if (Offset >= ScrollEndPosition - End)
                    {
                        break;
                    }
                    Offset += size + CellSpacing;
                }
            }
        }
    }
}