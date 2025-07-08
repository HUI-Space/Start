using System;
using UnityEngine.Playables;

namespace Start
{
    public class UIAnimationPlayable : PlayableBehaviour
    {
        public event Action<int> OnAnimationEnd;
        public int Index { get; private set; } = -1;
        public bool IsPlaying{ get; private set; }
        
        private bool _loop;
        private float _length;
        private float _playedTime;
        
        public void Play(int index, bool loop, float length)
        {
            _loop = loop;
            _length = length;
            _playedTime = 0;
            Index = index;
            IsPlaying = true;
        }
        
        public override void PrepareFrame(Playable playable, FrameData frameData)
        {
            if (Index == -1 || _loop)
            {
                return;
            }
            _playedTime += frameData.deltaTime;
            if (_playedTime >= _length)
            {
                _loop = default;
                _length = default;
                _playedTime = default;
                IsPlaying = default;
                OnAnimationEnd?.Invoke(Index);
            }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            _loop = default;
            _length = default;
            _playedTime = default;
            Index = default;
            IsPlaying = default;
        }
    }
}