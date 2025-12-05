using System;
using System.Collections;
using UnityEngine;

namespace Start
{
    public class CoroutineState : IReusable
    {
        public long CoroutineId { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsPaused { get; private set; }
        
        public Action<bool> StopHandle;

        private IEnumerator _coroutine;
        private IEnumerator _wrappedCoroutine;

        public static CoroutineState Create(long coroutineId, IEnumerator coroutine, Action<bool> onStopCallback = null)
        {
            CoroutineState state = RecyclableObjectPool.Acquire<CoroutineState>();
            state._coroutine = coroutine;
            state.CoroutineId = coroutineId;
            state.StopHandle += onStopCallback;
            return state;
        }

        public void Reset()
        {
            CoroutineId = default;
            IsRunning = default;
            IsPaused = default;
            StopHandle = default;
            _wrappedCoroutine = default;
            _coroutine = default;
        }

        public void Start()
        {
            IsRunning = true;
            _wrappedCoroutine = CoroutineWrapper();
            CoroutineController.Instance.StartCoroutine(_wrappedCoroutine);
        }

        public void Pause()
        {
            IsPaused = true;
        }

        public void Resume()
        {
            IsPaused = false;
        }

        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                if (_wrappedCoroutine != null)
                {
                    CoroutineController.Instance.StopCoroutine(_wrappedCoroutine);
                    try
                    {
                        StopHandle?.Invoke(true);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                    finally
                    {
                        CoroutineController.ReleaseCoroutineState(this);
                    }
                }
            }
        }

        // 作用:为了知道指定的协程什么时候结束
        private IEnumerator CoroutineWrapper()
        {
            while (IsRunning)
            {
                if (IsPaused)
                {
                    yield return null;
                }
                else
                {
                    if (_coroutine != null && _coroutine.MoveNext())
                    {
                        yield return _coroutine.Current;
                    }
                    else
                    {
                        IsRunning = false;
                    }
                }
            }

            try
            {
                StopHandle?.Invoke(false);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                CoroutineController.ReleaseCoroutineState(this);
            }
        }
    }
}