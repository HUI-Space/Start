using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Start
{
    /// <summary>
    /// 如挂载的物体同时有其他点击组件，通过MoveUp或MoveDown移动组件可以控制触发的顺序
    /// 如有需要可继续扩展
    /// 只穿透下面的第一个物体
    /// 挂载的物体和需要穿透的物体上都需要有能接受射线的组件
    /// </summary>
    public class ThroughEventHandler : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IInitializePotentialDragHandler
        , IPointerUpHandler, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler
    {
        [FoldoutGroup("控制点击事件")]
        public bool IsPointerClick = true;

        [FoldoutGroup("控制点击事件")]
        public bool IsBeginDrag = true;

        [FoldoutGroup("控制点击事件")]
        public bool IsDrag = true;

        [FoldoutGroup("控制点击事件")]
        public bool IsEndDrag = true;

        [FoldoutGroup("控制点击事件")]
        public bool IsInitializePotentialDrag = true;

        [FoldoutGroup("控制点击事件")]
        public bool IsPointerUp = true;

        [FoldoutGroup("控制点击事件")]
        public bool IsPointerEnter = true;

        [FoldoutGroup("控制点击事件")]
        public bool IsPointerDown = true;

        [FoldoutGroup("控制点击事件")]
        public bool IsPointerExit = true;
        
        [Header("是否穿透下层所有物体")]
        public bool IsThroughAllEvent;

        [Header("穿透下层物体个数,能接收到射线的算一个")]
        public int ThroughEventCount = 1;

        private readonly List<RaycastResult> _results = new List<RaycastResult>();

        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsPointerClick)
                PassEvent(eventData, ExecuteEvents.pointerClickHandler);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (IsPointerUp)
                PassEvent(eventData, ExecuteEvents.pointerUpHandler);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (IsPointerEnter)
                PassEvent(eventData, ExecuteEvents.pointerEnterHandler);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (IsPointerDown)
                PassEvent(eventData, ExecuteEvents.pointerDownHandler);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (IsPointerExit)
                PassEvent(eventData, ExecuteEvents.pointerExitHandler);
        }

        #region 拖动

        private List<GameObject> _curDragObjs = new List<GameObject>();

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (IsInitializePotentialDrag)
                PassEvent(eventData, ExecuteEvents.initializePotentialDrag, ref _curDragObjs);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (IsBeginDrag == false) return;
            if (_curDragObjs.Count > 0)
            {
                int throughCount = 0;
                foreach (var t in _curDragObjs)
                {
                    ExecuteEvents.Execute(t, eventData, ExecuteEvents.beginDragHandler);

                    throughCount++;
                    if (IsThroughAllEvent == false && ThroughEventCount <= throughCount)
                    {
                        break;
                    }
                }
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (IsDrag == false) return;
            if (_curDragObjs.Count > 0)
            {
                int throughCount = 0;
                foreach (var t in _curDragObjs)
                {
                    ExecuteEvents.Execute(t, eventData, ExecuteEvents.dragHandler);

                    throughCount++;
                    if (IsThroughAllEvent == false && ThroughEventCount <= throughCount)
                    {
                        break;
                    }
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (IsEndDrag == false) return;
            if (_curDragObjs.Count > 0)
            {
                int throughCount = 0;
                foreach (var t in _curDragObjs)
                {
                    ExecuteEvents.Execute(t, eventData, ExecuteEvents.endDragHandler);

                    throughCount++;
                    if (IsThroughAllEvent == false && ThroughEventCount <= throughCount)
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        private void PassEvent<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function)
            where T : IEventSystemHandler
        {
            int throughCount = 0;
            _results.Clear();
            //RaycastAll会自动排序
            EventSystem.current.RaycastAll(data, _results);

            float index = -1;
            for (int i = 0; i < _results.Count; i++)
            {
                if (gameObject == _results[i].gameObject)
                {
                    index = _results[i].index;
                    break;
                }
            }

            for (int i = 0; i < _results.Count; i++)
            {
                //忽略上层物体
                if (index == -1 || _results[i].index > index)
                {
                    ExecuteEvents.Execute(_results[i].gameObject, data, function);
                    throughCount++;
                    if (IsThroughAllEvent == false && ThroughEventCount <= throughCount)
                    {
                        break;
                    }
                }
            }
        }

        private void PassEvent<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function, ref List<GameObject> obj)
            where T : IEventSystemHandler
        {
            int throughCount = 0;
            obj.Clear();
            _results.Clear();
            //RaycastAll会自动排序
            EventSystem.current.RaycastAll(data, _results);

            float index = -1;
            for (int i = 0; i < _results.Count; i++)
            {
                if (gameObject == _results[i].gameObject)
                {
                    index = _results[i].index;
                    break;
                }
            }

            for (int i = 0; i < _results.Count; i++)
            {
                //忽略上层物体
                if (index == -1 || _results[i].index > index)
                {
                    ExecuteEvents.Execute(_results[i].gameObject, data, function);
                    obj.Add(_results[i].gameObject);
                    throughCount++;
                    if (IsThroughAllEvent == false && ThroughEventCount <= throughCount)
                    {
                        break;
                    }
                }
            }
        }
    }
}