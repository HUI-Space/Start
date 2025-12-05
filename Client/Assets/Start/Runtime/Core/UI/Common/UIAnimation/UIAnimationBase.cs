using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Start
{
    [RequireComponent(typeof(Animator))]
    public abstract class UIAnimationBase : UIElement
    {
        protected UIAnimationClip[] UIAnimationClip;
        
        private RecycleTask _task;
        private string _parentUIName;
        private UIAnimationPlayableGraph _graph;
        private readonly List<UIAnimationClipData> _list = new List<UIAnimationClipData>();
        
        public override void Initialize()
        {
            int index = 0;
            foreach (UIAnimationClip item in UIAnimationClip)
            {
                if (string.IsNullOrEmpty(item.ActionType) || item.Clip == null || item.Speed <= 0f)
                {
                    continue;
                }
                UIAnimationClipData data = UIAnimationClipData.Create(index,item);
                _list.Add(data);
                index++;
            }
            if (_list.Count > 0)
            {
                _graph = new UIAnimationPlayableGraph();
                _graph.Initialize(GetComponent<Animator>(), _list);
                _graph.OnAnimationEnd += AnimationEnd;
                UIWindow.Instance.AfterReceive += After;
                UIBase parent = GetComponentInParent<UIBase>();
                _parentUIName = parent.GetType().Name;
            }
        }

        private Task After(UIAction action)
        {
            if (action.UIName == _parentUIName)
            {
                return Play(action.ActionType);
            }
            return Task.CompletedTask;
        }

        private async Task Play(string actionType)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                if (actionType.Equals(_list[i].ActionType))
                {
                    UIAnimationClipData data = _list[i];
                
                    if (data.Delay > 0)
                    {
                        await Task.Delay((int)data.Delay * 1000);
                    }
                    if (_task != null)
                    {
                        _task.SetResult();
                        _task = default;
                    }
                    _graph.Play(i, data.Loop,data.Speed,data.AnimationClip.length);
                    if (data.Await)
                    {
                        _task = RecycleTask.Create();
                        await _task;
                    }
                }
            }
        }
        
        private void AnimationEnd(int index)
        {
            if (_task != null)
            {
                _task.SetResult();
                _task = default;
            }
            UIAnimationClipData data = _list[index];
            if (data.ActionType == UIActionTypes.Open)
            {
                for (int i = 0; i < _list.Count; i++)
                {
                    if (_list[i].ActionType.Equals("Loop"))
                    {
                        Play(_list[i].ActionType).Await();
                    }
                }
            }
        }
        
        public override void DeInitialize()
        {
            if (_list.Count > 0)
            {
                UIWindow.Instance.AfterReceive -= After;
                foreach (UIAnimationClipData data in _list)
                {
                    RecyclableObjectPool.Recycle(data);
                }
                _list.Clear();
                _graph.OnAnimationEnd -= AnimationEnd;
                _graph.DeInitialize();
                _graph = default;
                _parentUIName = default;
                if (_task != null)
                {
                    _task.SetResult();
                    _task = default;
                }
            }
        }
    }
}