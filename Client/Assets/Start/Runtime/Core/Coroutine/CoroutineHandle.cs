using UnityEngine;

namespace Start
{
    public struct CoroutineHandle
    {
        public long Id { get; internal set; }

        /// <summary>
        /// 表示该Coroutine状态可用，可以调用开始/暂停/恢复/停止
        /// 停止后则不可用
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (CoroutineController.TryGetCoroutineState(Id, out _))
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// 是否暂停
        /// </summary>
        public bool IsPause
        {
            get
            {
                if (CoroutineController.TryGetCoroutineState(Id, out CoroutineState state))
                {
                    return state.IsPaused;
                }

                return false;
            }
        }

        /// <summary>
        /// 是否停止
        /// </summary>
        public bool IsRunning
        {
            get
            {
                if (CoroutineController.TryGetCoroutineState(Id, out CoroutineState state))
                {
                    return state.IsRunning;
                }

                return false;
            }
        }
        
        /// <summary>
        /// 开始执行coroutine (直接调用，不需要判断当前状态）
        /// </summary>
        public void Start()
        {
            if (CoroutineController.TryGetCoroutineState(Id, out CoroutineState state))
            {
                state.Start();
                return;
            }
            Debug.LogError("尝试执行过期的coroutine,请获取新的CoroutineHandle");
        }
        
        /// <summary>
        /// 暂停coroutine (直接调用，不需要判断当前状态）
        /// </summary>
        public void Pause()
        {
            if (CoroutineController.TryGetCoroutineState(Id, out CoroutineState state))
            {
                state.Pause();
            }
        }
        
        /// <summary>
        /// 恢复coroutine (直接调用，不需要判断当前状态）
        /// </summary>
        public void Resume()
        {
            if (CoroutineController.TryGetCoroutineState(Id, out CoroutineState state))
            {
                state.Resume();
            }
        }

        /// <summary>
        /// 停止coroutine (直接调用，不需要判断当前状态）
        /// </summary>
        public void Stop()
        {
            if (CoroutineController.TryGetCoroutineState(Id, out CoroutineState state))
            {
                state.Stop();
            }
        }

        public CoroutineHandle(long Id)
        {
            this.Id = Id;
        }
    }
}