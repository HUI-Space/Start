using System;
using UnityEngine;

namespace Start
{
    /// <summary>
    /// 在ui超出屏幕时，自动将ui设置到屏幕边缘。通常用于点击图标显示某个信息框，信息框出现在点击的位置。
    /// 支持任意枢轴
    /// 锚点必须在左上、右上、左下、右下
    /// 锚点在左上代表不会超出屏幕的上边和左边
    /// </summary>
    public class UIBeyondScreenFix : MonoBehaviour
    {
        public enum EAnchorsType
        {
            None,
            LeftTop,
            LeftBottom,
            RightTop,
            RightBottom,
        }

        [Header("为None时自动判断。不为None时根据枚举设置锚点")]
        public EAnchorsType type;

        private bool _isDirty;

        private RectTransform _rectT;

        private Action _curFixAction;

        private Vector2 temp = Vector2.zero;

        private void Awake()
        {
            _rectT = transform as RectTransform;

            SetAnchor(type);
        }

        private void Update()
        {
            if (_isDirty)
            {
                _curFixAction?.Invoke();
                _isDirty = false;
            }
        }
        
        #region api

        /// <summary>
        /// 在修改位置后调用，立即修正位置
        /// </summary>
        public void SetFix()
        {
            _curFixAction?.Invoke();
        }

        /// <summary>
        /// 在下一帧修正位置
        /// </summary>
        public void SetDirty()
        {
            _isDirty = true;
        }

        #endregion

        #region 设置和判断锚点

        private void SetAnchor(EAnchorsType type)
        {
            switch (type)
            {
                case EAnchorsType.None:
                    CheckAnchor();
                    return;
                case EAnchorsType.LeftBottom:
                    temp.Set(0, 0);
                    _curFixAction = FixLeftBottom;
                    break;
                case EAnchorsType.LeftTop:
                    temp.Set(0, 1);
                    _curFixAction = FixLeftTop;
                    break;
                case EAnchorsType.RightBottom:
                    temp.Set(1, 0);
                    _curFixAction = FixRightBottom;
                    break;
                case EAnchorsType.RightTop:
                    temp.Set(1, 1);
                    _curFixAction = FixRightTop;
                    break;
            }

            _rectT.anchorMax = temp;
            _rectT.anchorMin = temp;
        }

        private void CheckAnchor()
        {
            if (_rectT.anchorMax.x != _rectT.anchorMin.x || _rectT.anchorMax.y != _rectT.anchorMin.y)
            {
                Debug.LogError($"挂载UiBeyondScreenFix的物体锚点必须为左上、右上、左下、右下,GameObject名称：{gameObject.name}");
                return;
            }

            if (_rectT.anchorMax.x != 0 && _rectT.anchorMax.x != 1 || _rectT.anchorMax.y != 0 && _rectT.anchorMax.y != 1)
            {
                Debug.LogError($"挂载UiBeyondScreenFix的物体锚点必须为左上、右上、左下、右下,GameObject名称：{gameObject.name}");
                return;
            }

            if (_rectT.anchorMax.x == 0)
            {
                if (_rectT.anchorMax.y == 0)
                {
                    //左下
                    type = EAnchorsType.LeftBottom;
                    _curFixAction = FixLeftBottom;
                }
                else if (_rectT.anchorMax.y == 1)
                {
                    //左上
                    type = EAnchorsType.LeftTop;
                    _curFixAction = FixLeftTop;
                }
            }
            else if (_rectT.anchorMax.x == 1)
            {
                if (_rectT.anchorMax.y == 0)
                {
                    //右下
                    type = EAnchorsType.RightBottom;
                    _curFixAction = FixRightBottom;
                }
                else if (_rectT.anchorMax.y == 1)
                {
                    //右上
                    type = EAnchorsType.RightTop;
                    _curFixAction = FixRightTop;
                }
            }
        }

        #endregion

        #region Fix方法

        private void FixLeftTop()
        {
            Fix(_rectT.sizeDelta.x * _rectT.pivot.x, _rectT.sizeDelta.y * (1 - _rectT.pivot.y));
        }

        private void FixLeftBottom()
        {
            Fix(_rectT.sizeDelta.x * _rectT.pivot.x, _rectT.sizeDelta.y * _rectT.pivot.y);
        }

        private void FixRightTop()
        {
            Fix(_rectT.sizeDelta.x * (1 - _rectT.pivot.x), _rectT.sizeDelta.y * (1 - _rectT.pivot.y));
        }

        private void FixRightBottom()
        {
            Fix(_rectT.sizeDelta.x * (1 - _rectT.pivot.x), _rectT.sizeDelta.y * _rectT.pivot.y);
        }

        private void Fix(float x, float y)
        {
            temp.Set(_rectT.anchoredPosition.x, _rectT.anchoredPosition.y);

            float xDis = Mathf.Abs(_rectT.anchoredPosition.x) - x;
            if (xDis < 0)
            {
                //x超出
                if (_rectT.anchoredPosition.x > 0)
                {
                    temp.Set(_rectT.anchoredPosition.x - xDis, temp.y);
                }
                else
                {
                    temp.Set(_rectT.anchoredPosition.x + xDis, temp.y);
                }
            }

            float yDis = Mathf.Abs(_rectT.anchoredPosition.y) - y;
            if (yDis < 0)
            {
                //y超出
                if (_rectT.anchoredPosition.y > 0)
                {
                    temp.Set(temp.x, _rectT.anchoredPosition.y - yDis);
                }
                else
                {
                    temp.Set(temp.x, _rectT.anchoredPosition.y + yDis);
                }
            }

            _rectT.anchoredPosition = temp;
        }

        #endregion
    }
}