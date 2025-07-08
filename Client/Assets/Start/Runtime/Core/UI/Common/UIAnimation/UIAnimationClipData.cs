

using UnityEngine;

namespace Start
{
    public class UIAnimationClipData : IReference
    {
        /// <summary>
        /// 索引
        /// </summary>
        public int Index { get; private set; }
        
        /// <summary>
        /// UIAction 的 ActionType 类型
        /// </summary>
        public string ActionType { get; private set; }

        /// <summary>
        /// 动画
        /// </summary>
        public AnimationClip AnimationClip { get; private set; }

        /// <summary>
        /// 播放速度
        /// </summary>
        public float Speed { get; private set; }

        /// <summary>
        /// 延迟多少秒播放
        /// </summary>
        public float Delay { get; private set; }

        /// <summary>
        /// 是否循环播放
        /// </summary>
        public bool Loop { get; private set; }

        /// <summary>
        /// 是否等待动画播放完成
        /// </summary>
        public bool Await { get; private set; }

        public static UIAnimationClipData Create(int index,UIAnimationClip clip)
        {
            UIAnimationClipData uiAnimationClipData = ReferencePool.Acquire<UIAnimationClipData>();
            uiAnimationClipData.Index = index;
            uiAnimationClipData.ActionType = clip.ActionType;
            uiAnimationClipData.AnimationClip = clip.Clip;
            uiAnimationClipData.Speed = clip.Speed;
            uiAnimationClipData.Delay = clip.Delay;
            uiAnimationClipData.Loop = clip.Loop;
            uiAnimationClipData.Await = clip.Await;
            return uiAnimationClipData;
        }

        public void Clear()
        {
            Index = default;
            ActionType = default;
            AnimationClip = default;
            Speed = default;
            Delay = default;
            Loop = default;
            Await = default;
        }
    }
}