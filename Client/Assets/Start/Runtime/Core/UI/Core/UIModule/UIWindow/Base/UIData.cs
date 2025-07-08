using System;
using System.Collections.Generic;

namespace Start
{
    public abstract class UIData : IUIData
    {
        private bool _isDirty;
        public bool IsDirty
        {
            get => _isDirty;
            protected set
            {
                _isDirty = value;
                if (value)
                {
                    UIWindow.Instance.UpdateUIBase(this);
                }
            }
        }
        public abstract string UIName { get; }
        public int Order { get; set; }
        public bool IsShow { get; private set; }
        public bool IsOnShow { get; private set; }
        public bool IsOnHide { get; private set; }
        
        private Dictionary<string, HashSet<Action<UIAction>>> _uiActionDic;

        public virtual void Initialize()
        {
            _uiActionDic = new Dictionary<string, HashSet<Action<UIAction>>>();
            Register(UIActionTypes.Open, Open);
            Register(UIActionTypes.Show, Show);
            Register(UIActionTypes.Render, Render);
            Register(UIActionTypes.Hide, Hide);
            Register(UIActionTypes.Close, Close);
            Register(UIActionTypes.SetOrder, SetOrder);
        }

        public void ReceiveAction(UIAction uiAction)
        {
            if (_uiActionDic.TryGetValue(uiAction.ActionType, out HashSet<Action<UIAction>> hashSet))
            {
                foreach (var callBack in hashSet)
                {
                    callBack?.Invoke(uiAction);
                }
            }
        }

        public virtual void BeforeClearDirty()
        {
        }

        public void ClearDirty()
        {
            IsOnShow = false;
            IsOnHide = false;
            IsDirty = false;
        }

        public void SetDirty()
        {
            IsDirty = true;
        }

        public virtual void DeInitialize()
        {
            foreach (HashSet<Action<UIAction>> hashSet in _uiActionDic.Values)
            {
                hashSet.Clear();
            }
            _uiActionDic.Clear();
            _uiActionDic = null;
        }

        protected void Register(string uiAction, Action<UIAction> callBack)
        {
            if (!_uiActionDic.TryGetValue(uiAction, out HashSet<Action<UIAction>> hashSet))
            {
                hashSet = new HashSet<Action<UIAction>>();
                _uiActionDic.Add(uiAction, hashSet);
            }
            hashSet.Add(callBack);
        }

        protected void UnRegister(string uiAction, Action<UIAction> callBack)
        {
            if (_uiActionDic.TryGetValue(uiAction, out HashSet<Action<UIAction>> hashSet))
            {
                hashSet.Remove(callBack);
            }
        }

        protected virtual void Open(UIAction uiAction)
        {
            IsShow = true;
            IsOnShow = true;
            IsDirty = true;
        }

        protected virtual void Show(UIAction uiAction)
        {
            IsShow = true;
            IsOnShow = true;
            IsDirty = true;
        }
        
        protected virtual void Render(UIAction uiAction)
        {
            IsDirty = true;
        }

        protected virtual void Hide(UIAction uiAction)
        {
            IsOnHide = true;
            IsShow = false;
            IsDirty = true;
        }

        protected virtual void Close(UIAction uiAction)
        {
            IsShow = false;
            IsDirty = true;
        }

        protected virtual void SetOrder(UIAction uiAction)
        {
            Order = uiAction.GetData1<int>();
            IsDirty = true;
        }
    }
}