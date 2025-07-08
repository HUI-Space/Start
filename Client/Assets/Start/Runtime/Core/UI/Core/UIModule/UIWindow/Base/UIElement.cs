using System;
using UnityEngine;

namespace Start
{
    public abstract class UIElement : UIBase
    {

    }

    public abstract class UIElement<T> : UIElement
    {
        public T Value { get; protected set; }

        public event Action<T> OnSetValue;

        public void SetData(T data)
        {
            Value = data;
            try
            {
                OnSetValue?.Invoke(Value);
                Render(Value);
            }
            catch (Exception e)
            {
                Debug.LogError($"{GetType()} 设置Value:{data}出错 {e}");
            }
        }

        protected abstract void Render(T data);
    }
}