using System;
using UnityEngine;

namespace Start
{
    [Serializable]
    public class UIAnimationClip
    {
        /// <summary>
        /// UIAction 的 ActionType 类型
        /// </summary>
        public string ActionType;
        
        /// <summary>
        /// 动画
        /// </summary>
        public AnimationClip Clip;

        /// <summary>
        /// 播放速度
        /// </summary>
        public float Speed = 1f;
        
        /// <summary>
        /// 延迟多少秒播放
        /// </summary>
        public float Delay;

        /// <summary>
        /// 是否循环播放
        /// </summary>
        public bool Loop;

        /// <summary>
        /// 是否等待动画播放完成
        /// </summary>
        public bool Await;
    }
}