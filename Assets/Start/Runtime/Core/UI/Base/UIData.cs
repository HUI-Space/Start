using System;
using System.Collections.Generic;
using Start.Framework;

namespace Start.Runtime
{
    public abstract class UIData:IUIData
    {
        public abstract string UIName { get; }
        public int Order { get;  set;}
        public bool IsShow { get; private set; }
        public bool IsDirty { get; protected set; }
        public bool IsOnShow { get; private set; }
        public bool IsOnHide { get; private set; }
        
        private readonly Dictionary<string,HashSet<Action<UIAction>>>_uiActionDic = new Dictionary<string, HashSet<Action<UIAction>>>();

        public virtual void Initialize()
        {
            Register(ActionType.OpenUI,OnOpen);
            Register(ActionType.ShowUI,OnShow);
            Register(ActionType.HideUI,OnHide);
            Register(ActionType.CloseUI,OnClose);
            Register(ActionType.SetOrder,OnSetOrder);
        }
        
        public void ReceiveAction(UIAction uiAction)
        {
            if (_uiActionDic.TryGetValue(uiAction.ActionType,out HashSet<Action<UIAction>> hashSet))
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
            UnRegister(ActionType.OpenUI,OnOpen);
            UnRegister(ActionType.ShowUI,OnShow);
            UnRegister(ActionType.HideUI,OnHide);
            UnRegister(ActionType.CloseUI,OnClose);
            foreach (HashSet<Action<UIAction>> hashSet in _uiActionDic.Values)
            {
                hashSet.Clear();
            }
            _uiActionDic.Clear();
        }
        
        protected void Register(string uiAction, Action<UIAction> callBack)
        {
            if (!_uiActionDic.TryGetValue(uiAction,out HashSet<Action<UIAction>> hashSet))
            {
                hashSet = new HashSet<Action<UIAction>>();
                _uiActionDic.Add(uiAction,hashSet);
            }
            hashSet.Add(callBack);
        }

        protected void UnRegister(string uiAction, Action<UIAction> callBack)
        {
            if (_uiActionDic.TryGetValue(uiAction,out HashSet<Action<UIAction>> hashSet))
            {
                hashSet.Remove(callBack);
            }
        }
        
        protected virtual void OnOpen(UIAction uiAction)
        {
            if (UIName == uiAction.UIName)
            {
                IsShow = true;
                IsOnShow = true;
                IsDirty = true;
            }
            
        }

        protected virtual void OnShow(UIAction uiAction)
        {
            if (UIName == uiAction.UIName)
            {
                IsShow = true;
                IsOnShow = true;
                IsDirty = true;
            }
            
        }

        protected virtual void OnHide(UIAction uiAction)
        {
            if (UIName == uiAction.UIName)
            {
                IsShow = false;
                IsOnHide = true;
                IsDirty = true;
            }
            
        }

        protected virtual void OnClose(UIAction uiAction)
        {
            if (UIName == uiAction.UIName)
            {
                IsShow = false;
                IsDirty = true;
            }
        }
        
        private void OnSetOrder(UIAction uiAction)
        {
            if (UIName == uiAction.UIName)
            {
                Order = uiAction.GetData1<int>();
                IsDirty = true;
            }
        }
    }
}