using System;
using System.Collections;
using System.Collections.Generic;
using Start.Framework;
using UnityEngine;

namespace Start.Runtime
{
    public delegate void TaskStopHandle(bool isManualStopped);
    
    public class CoroutineRunner : SingletonMonoBehaviour<CoroutineRunner>
    {
        private static long _coroutineId = 0;
        private static readonly Dictionary<long, CoroutineState> _coroutineDic = new Dictionary<long, CoroutineState>();
        
        #region 对外接口
        
        internal static bool TryGetCoroutineState(long coroutineId, out CoroutineState state)
        {
            if (_coroutineDic.TryGetValue(coroutineId, out state))
            {
                return true;
            }

            return false;
        }

        public static void ReleaseCoroutineState(CoroutineState state)
        {
            _coroutineDic.Remove(state.CoroutineId);
            ReferencePool.Release(state);
        }
        
        public static CoroutineHandle Run(IEnumerator coroutine, bool autoStart = true, TaskStopHandle onStopCallback = null)
        {
            CoroutineState state = CoroutineState.Create(++_coroutineId,coroutine,onStopCallback);
            _coroutineDic.Add(state.CoroutineId, state);
            if (autoStart)
            {
                state.Start();
            }
            return new CoroutineHandle(state.CoroutineId);
        }
        
        public static CoroutineHandle RunAfterSeconds(float seconds, Action action)
        {
            CoroutineState state = CoroutineState.Create(++_coroutineId,WaitForSeconds(seconds, action));
            state.Start();
            return new CoroutineHandle(state.CoroutineId);
        }

        public static CoroutineHandle RunAtEndOfFrame(Action action)
        {
            CoroutineState state = CoroutineState.Create(++_coroutineId,WaitForEndOfFrame(action));
            state.Start();
            return new CoroutineHandle(state.CoroutineId);
        }

        public static CoroutineHandle RunAtNextFrame(Action action)
        {
            CoroutineState state = CoroutineState.Create(++_coroutineId,WaitForNextFrame(action));
            state.Start();
            return new CoroutineHandle(state.CoroutineId);
        }

        public static CoroutineHandle RunAtFixedUpdate(Action action)
        {
            CoroutineState state = CoroutineState.Create(++_coroutineId,WaitForFixedUpdate(action));
            state.Start();
            return new CoroutineHandle(state.CoroutineId);
        }
        
        #endregion

        #region 内部接口
        
        private static IEnumerator WaitForSeconds(float seconds, Action action)
        {
            yield return new WaitForSeconds(seconds);
            action?.Invoke();
        }
        
        private static IEnumerator WaitForEndOfFrame(Action action)
        {
            yield return UnityUtility.WaitForEndOfFrame;
            action?.Invoke();
        }
        
        private static IEnumerator WaitForNextFrame(Action action)
        {
            yield return null;
            action?.Invoke();
        }
        
        private static IEnumerator WaitForFixedUpdate(Action action)
        {
            yield return UnityUtility.WaitForFixedUpdate;
            action?.Invoke();
        }
        #endregion
    }
}