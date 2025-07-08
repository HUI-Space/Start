using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Start
{
    public class ScrollerPanelItem : UIElement
    {
        public ScrollerItem ScrollerItem;
        
        public ScrollBase FirstScrollerBase;
        public ScrollBase SecondScrollBase;
        
        public InputField ScrollToPositionInputField;
        public InputField ScrollToIndexInputField;
        public InputField ScrollItemCountInputField;
        
        public Button ScrollToPositionButton;
        public Button ScrollToIndexButton;
        public Button RefreshButton;
        
        public override void Initialize()
        {
            base.Initialize();
            FirstScrollerBase.SetElementUI(ScrollerItem, UnFixedRenderCell, GetSizeAlongDirection);
            SecondScrollBase.SetElementUI(ScrollerItem, FixedRenderCell);
            
            ScrollToPositionButton.onClick.AddListener(OnScrollToPositionButtonClick);
            ScrollToIndexButton.onClick.AddListener(OnScrollToIndexButtonClick);
            RefreshButton.onClick.AddListener(OnRefreshButtonClick);
        }
        
        public override void DeInitialize()
        {
            base.DeInitialize();
            ScrollToPositionButton.onClick.RemoveListener(OnScrollToPositionButtonClick);
            ScrollToIndexButton.onClick.RemoveListener(OnScrollToIndexButtonClick);
            RefreshButton.onClick.RemoveListener(OnRefreshButtonClick);
        }

        private void OnScrollToPositionButtonClick()
        {
            int.TryParse(ScrollToPositionInputField.text, out int position);
            FirstScrollerBase.ScrollToPosition(position);
            SecondScrollBase.ScrollToPosition(position);
        }
        
        private void OnScrollToIndexButtonClick()
        {
            int.TryParse(ScrollToIndexInputField.text, out int index);
            FirstScrollerBase.ScrollToIndex(index);
            SecondScrollBase.ScrollToIndex(index);
        }
        
        private void OnRefreshButtonClick()
        {
            int.TryParse(ScrollItemCountInputField.text, out int count);
            FirstScrollerBase.SetCount(count);
            SecondScrollBase.SetCount(count);
        }


        private float GetSizeAlongDirection(int cellIndex)
        {
            return FirstScrollerBase.GetDefaultSize() + cellIndex;
        }

        private void UnFixedRenderCell(UIElement element, int cellIndex)
        {
            if (element is ScrollerItem scrollerItem)
            {
                scrollerItem.SetData(cellIndex);
                if (FirstScrollerBase is HorizontalScroller)
                {
                    scrollerItem.GetComponent<RectTransform>().
                        SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 
                            FirstScrollerBase.GetDefaultSize() + cellIndex);
                }
                if (FirstScrollerBase is VerticalScroller)
                {
                    scrollerItem.GetComponent<RectTransform>().
                        SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 
                            FirstScrollerBase.GetDefaultSize() + cellIndex);
                }
            }
        }
        
        private void FixedRenderCell(UIElement element, int cellIndex)
        {
            if (element is ScrollerItem scrollerItem)
            {
                scrollerItem.SetData(cellIndex);
            }
        }
        
    }
}