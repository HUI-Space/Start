﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Start.Editor
{
    public class ObjectPool<T>
    {
        private readonly Stack<T> m_Stack = new Stack<T>();

        private readonly Action<T> m_ActionOnGet;

        private readonly Action<T> m_ActionOnRelease;

        public int countAll
        {
            get;
            private set;
        }

        public int countActive
        {
            get
            {
                return countAll - countInactive;
            }
        }

        public int countInactive
        {
            get
            {
                return m_Stack.Count;
            }
        }

        public ObjectPool(Action<T> actionOnGet = null, Action<T> actionOnRelease = null)
        {
            m_ActionOnGet = actionOnGet;
            m_ActionOnRelease = actionOnRelease;
        }

        public T Get()
        {
            T val;
            if (m_Stack.Count == 0)
            {
                val = (T)Activator.CreateInstance(typeof(T));
                countAll++;
            }
            else
            {
                val = m_Stack.Pop();
            }
            if (m_ActionOnGet != null)
            {
                m_ActionOnGet(val);
            }
            return val;
        }

        public void Release(T element)
        {
            if (m_Stack.Count > 0 && object.ReferenceEquals(m_Stack.Peek(), element))
            {
                Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
            }
            if (m_ActionOnRelease != null)
            {
                m_ActionOnRelease(element);
            }
            m_Stack.Push(element);
        }

        public void Clear()
        {
            if (m_Stack !=null)
            {
                m_Stack.Clear(); 
            }
        }
    }
}