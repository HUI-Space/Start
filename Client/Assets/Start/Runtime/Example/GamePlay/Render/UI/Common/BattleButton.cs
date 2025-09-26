using UnityEngine;
using UnityEngine.EventSystems;

namespace Start
{
    public class BattleButton : MonoBehaviour , IPointerDownHandler , IPointerUpHandler
    {
        /// <summary>
        /// 按钮索引
        /// </summary>
        public int Index;
        
        /// <summary>
        /// 按钮是否按下
        /// </summary>
        public bool IsDown { get; private set; }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            IsDown = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsDown = false;
        }
    }
}