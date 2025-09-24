using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Start
{
    public class UISafeArea : MonoBehaviour
    {
        private static bool _isInit;
        private static Rect _safeArea;
        private RectTransform _rect;

        /// <summary>
        /// 实际区域
        /// </summary>
        /// <returns></returns>
        public Rect RealArea { get; private set; }

        private void Awake()
        {
            if (_isInit == false)
            {
#if UNITY_EDITOR
                // 获取 Game 视图的当前分辨率组
                Vector2 resolution = Handles.GetMainGameViewSize();
                _safeArea = new Rect(0, 0, resolution.x, resolution.y);
                if (!_safeArea.Equals(Screen.safeArea))
                {
                    //开了Simulator
                    _safeArea = Screen.safeArea;
                }
#else
                _safeArea = Screen.safeArea;
#endif
                _isInit = true;
            }

            Logger.Info($"SafeAreaDebug:{_safeArea.position} width {_safeArea.width} height {_safeArea.height} currentRes {Screen.currentResolution.width}x{Screen.currentResolution.height}");
            _rect = transform as RectTransform;

            if (_rect == null)
            {
                Logger.Error(gameObject + "没有RectTransform请检查");
                return;
            }

            bool isCustom = false;
            if (customSafeAreas != null)
            {
                Resolution resolution = Screen.currentResolution;
                foreach (CustomSafeArea customSafeArea in customSafeAreas)
                {
                    if (customSafeArea.CheckResolution(resolution))
                    {
                        isCustom = true;
                        CustomSetSafeArea(customSafeArea);
                    }
                }
            }

            if (isCustom == false)
            {
                RealArea = _safeArea;
                SetSafeArea(_safeArea);
            }
        }

        #region 自定义SafeArea

        [Serializable]
        public struct CustomSafeArea
        {
            public int width;
            public int height;

            /// <summary>
            /// x/分辨率宽
            /// </summary>
            public float xRate;

            /// <summary>
            /// y/分辨率高
            /// </summary>
            public float yRate;

            /// <summary>
            ///  Width/分辨率宽
            /// </summary>
            public float widthRate;

            /// <summary>
            /// Height/分辨率高
            /// </summary>
            public float heightRate;

            /// <summary>
            /// 编辑器调用
            /// </summary>
            public CustomSafeArea(int w, int h, float x, float y, float width, float height)
            {
                int minimumCommonDivisor = MathUtility.MinimumCommonDivisor(w, h);
                this.width = w / minimumCommonDivisor;
                this.height = h / minimumCommonDivisor;

                xRate = x / w;
                yRate = y / h;
                widthRate = width / w;
                heightRate = height / h;
            }

            public bool CheckResolution(Resolution resolution)
            {
                int minimumCommonDivisor = MathUtility.MinimumCommonDivisor(resolution.width, resolution.height);
                return width == resolution.width / minimumCommonDivisor && height == resolution.height / minimumCommonDivisor;
            }

            public Rect ToRect(Resolution resolution)
            {
                return new Rect(xRate * resolution.width, yRate * resolution.height, widthRate * resolution.width, heightRate * resolution.height);
            }
        }

        //TODO 添加自定义SafeArea 适配傻吊手机
        [HideInInspector]
        public List<CustomSafeArea> customSafeAreas;

        private void CustomSetSafeArea(CustomSafeArea custom)
        {
            //系统给的比自定义的小时用系统的
            Resolution resolution = Screen.currentResolution;
            Rect customRect = custom.ToRect(resolution);

            float x, y, width, height;

            x = _safeArea.x > customRect.x ? _safeArea.x : customRect.x;

            float length = _safeArea.x + _safeArea.width;
            float customLength = customRect.x + customRect.width;
            if (length < customLength)
            {
                width = length - x;
            }
            else
            {
                width = customLength - x;
            }

            y = _safeArea.y > customRect.y ? _safeArea.y : customRect.y;

            length = _safeArea.y + _safeArea.height;
            customLength = customRect.y + customRect.height;
            if (length < customLength)
            {
                height = length - y;
            }
            else
            {
                height = customLength - y;
            }

            Rect realRect = new Rect(x, y, width, height);

            RealArea = realRect;

            SetSafeArea(realRect);
        }

        #endregion

        protected virtual void SetSafeArea(Rect safeArea)
        {
            Vector3 panelScale = UIController.Instance.UICanvas.GetComponent<RectTransform>().localScale;
            panelScale.x /= UIController.Instance.PanelScale.x;
            panelScale.y /= UIController.Instance.PanelScale.y;
            panelScale.z /= UIController.Instance.PanelScale.z;
            Vector2 v = Vector2.zero;
            _rect.pivot = v;
            _rect.anchorMin = v;
            _rect.anchorMax = v;
            v.Set(safeArea.x / panelScale.x, safeArea.y / panelScale.y);
            _rect.anchoredPosition = v;
            v.Set(safeArea.width / panelScale.x, safeArea.height / panelScale.y);
            _rect.sizeDelta = v;
            _rect.localScale = Vector3.one;
        }
    }
}