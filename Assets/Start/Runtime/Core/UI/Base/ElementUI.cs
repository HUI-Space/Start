using System;
using UnityEngine;

namespace Start.Runtime
{
    public abstract class ElementUI : UIBase
    {
       
    }

    public abstract class ElementUI<T> : ElementUI
    {
        public T Value { get; protected set; }

        public event Action<T> OnSetValue;

        public void SetData(T data)
        {
            Value = data;
            try
            {
                Render(Value);
                OnSetValue?.Invoke(Value);
            }
            catch (Exception e)
            {
                Debug.LogError($"{GetType()} 设置Value:{data}出错 {e}");
            }
        }

        protected abstract void Render(T data);
    }
}