using System;
using System.Collections.Generic;
using UnityEngine;

namespace Start
{
    /// <summary>
    /// 内部New一个，SetData即可
    /// </summary>
    public class LimitList<T> where T : UIElement
    {
        private T _tmpl;
        private Transform _parent;
        private Action<T> _onCreate;
        private Action<T, int> _onSetValue;
        private readonly List<T> _elementPool = new List<T>();

        public void Initialize(T tmpl, Transform parent, Action<T> onCreate, Action<T, int> onSetValue)
        {
            _tmpl = tmpl;
            _parent = parent;
            _onCreate = onCreate;
            _onSetValue = onSetValue;
            tmpl.gameObject.SetActive(false);
        }

        public void DeInitialize()
        {
            _tmpl = null;
            _parent = null;
            _onCreate = null;
            _onSetValue = null;
            foreach (var t in _elementPool)
            {
                t.DeInitialize();
            }
            _elementPool.Clear();
        }
        public void SetData(int dataCount)
        {
            int i = 0;
            for (; i < dataCount; i++)
            {
                if (i < _elementPool.Count)
                {
                    _elementPool[i].gameObject.SetActive(true);
                }
                else
                {
                    T element = UIController.Instance.InstantiateElement(_tmpl, _parent);
                    _onCreate?.Invoke(element);
                    _elementPool.Add(element);
                }

                _onSetValue?.Invoke(_elementPool[i], i);
            }

            for (; i < _elementPool.Count; i++)
            {
                _elementPool[i].gameObject.SetActive(false);
            }
        }

        public void ForeachActiveElements(Action<T> callback)
        {
            foreach (T element in _elementPool)
            {
                if (element.gameObject.activeSelf)
                {
                    callback(element);
                }
            }
        }

        public void ForActiveElements(Action<int, T> callback)
        {
            for (int i = 0; i < _elementPool.Count; i++)
            {
                callback(i, _elementPool[i]);
            }
        }
    }

    /// <summary>
    /// 内部New一个，SetData即可
    /// </summary>
    public class LimitList<TElement, TData> where TElement : UIElement<TData>
    {
        private TElement _tmpl;
        private Transform _parent;
        private Action<TElement> _onCreate;
        private Action<TElement, TData> _onSetValue;
        private readonly List<TElement> _elementPool = new List<TElement>();
        
        public void Initialize(TElement tmpl, Transform parent, Action<TElement> onCreate = null, Action<TElement, TData> onSetValue = null)
        {
            _tmpl = tmpl;
            _parent = parent;
            _onCreate = onCreate;
            _onSetValue = onSetValue;
            tmpl.gameObject.SetActive(false);
        }

        public void DeInitialize()
        {
            _tmpl = null;
            _parent = null;
            _onCreate = null;
            _onSetValue = null;
            foreach (var t in _elementPool)
            {
                t.DeInitialize();
            }
            _elementPool.Clear();
        }

        public void SetData(IList<TData> data)
        {
            int i = 0;
            if (data != null)
            {
                for (; i < data.Count; i++)
                {
                    if (i < _elementPool.Count)
                    {
                        _elementPool[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        TElement element = UIController.Instance.InstantiateElement(_tmpl, _parent);
                        _onCreate?.Invoke(element);
                        _elementPool.Add(element);
                    }
                    _elementPool[i].SetData(data[i]);
                    _onSetValue?.Invoke(_elementPool[i], data[i]);
                }
            }

            for (; i < _elementPool.Count; i++)
            {
                _elementPool[i].gameObject.SetActive(false);
            }
        }

        public void SetData(IEnumerable<TData> datas)
        {
            int i = 0;
            foreach (TData data in datas)
            {
                if (i < _elementPool.Count)
                {
                    _elementPool[i].gameObject.SetActive(true);
                }
                else
                {
                    TElement element = UIController.Instance.InstantiateElement(_tmpl, _parent);
                    _onCreate?.Invoke(element);
                    _elementPool.Add(element);
                }

                _onSetValue?.Invoke(_elementPool[i], data);
                i++;
            }

            for (; i < _elementPool.Count; i++)
            {
                _elementPool[i].gameObject.SetActive(false);
            }
        }

        public void ForeachActiveElements(Action<TElement> callback)
        {
            foreach (TElement element in _elementPool)
            {
                callback(element);
            }
        }

        public void ForActiveElements(Action<int, TElement> callback)
        {
            for (int i = 0; i < _elementPool.Count; i++)
            {
                callback(i, _elementPool[i]);
            }
        }
    }
    
   
}