using System;
using System.Collections;
using Start.Framework;
using UnityEngine;

namespace Start.Runtime
{
    public class CoroutineState : IReference
    {
        public long CoroutineId { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsPaused { get; private set; }

        public TaskStopHandle TaskStopHandle;

        private IEnumerator _coroutine;
        private IEnumerator _wrappedCoroutine;

        public static CoroutineState Create(long coroutineId, IEnumerator coroutine, TaskStopHandle onStopCallback = null)
        {
            CoroutineState state = ReferencePool.Acquire<CoroutineState>();
            state.CoroutineId = coroutineId;
            state._coroutine = coroutine;
            state.TaskStopHandle += onStopCallback;
            return state;
        }

        public void Clear()
        {
            CoroutineId = default;
            IsRunning = default;
            IsPaused = default;
            TaskStopHandle = default;
            _wrappedCoroutine = default;
            _coroutine = default;
        }

        public void Start()
        {
            IsRunning = true;
            _wrappedCoroutine = CoroutineWrapper();
            CoroutineRunner.Instance.StartCoroutine(_wrappedCoroutine);
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
                    CoroutineRunner.Instance.StopCoroutine(_wrappedCoroutine);
                    try
                    {
                        TaskStopHandle?.Invoke(true);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                    finally
                    {
                        CoroutineRunner.ReleaseCoroutineState(this);
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
                TaskStopHandle?.Invoke(false);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                CoroutineRunner.ReleaseCoroutineState(this);
            }
        }
    }
}