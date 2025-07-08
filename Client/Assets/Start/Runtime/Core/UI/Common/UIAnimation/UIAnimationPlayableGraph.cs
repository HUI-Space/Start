using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Start
{
    public class UIAnimationPlayableGraph
    {
        public event Action<int> OnAnimationEnd;
        private PlayableGraph _graph;
        private UIAnimationPlayable _uiAnimation;
        private AnimationMixerPlayable _mixer;
        private AnimationPlayableOutput _output;
        private ScriptPlayable<UIAnimationPlayable> _playable;
        
        public void Initialize(Animator animator, List<UIAnimationClipData> clips)
        {
            _graph = PlayableGraph.Create();
            _output = AnimationPlayableOutput.Create(_graph, "UIAnimation", animator);
            _playable = ScriptPlayable<UIAnimationPlayable>.Create(_graph,1);
            _mixer = AnimationMixerPlayable.Create(_graph);
            _uiAnimation = _playable.GetBehaviour();
            _uiAnimation.OnAnimationEnd += AnimationEnd;
            _graph.Connect(_mixer, 0, _playable, 0);
            _playable.SetInputWeight(0, 1);
            _output.SetSourcePlayable(_playable);
            _mixer.SetInputCount(clips.Count);
            
            for (int i = 0; i < clips.Count; i++)
            {
                AnimationClipPlayable playable = AnimationClipPlayable.Create(_graph,clips[i].AnimationClip);
                _mixer.ConnectInput(i, playable, 0);
                _mixer.SetInputWeight(i, 0);
            }
        }

        public void Play(int index,bool loop, float speed,float length)
        {
            if (_uiAnimation.IsPlaying && _uiAnimation.Index == index)
            {
                return;
            }
            if (_uiAnimation.Index != -1)
            {
                _mixer.SetInputWeight(_uiAnimation.Index, 0);
            }
            _mixer.SetInputWeight(index, 1);
            _mixer.GetInput(index).SetSpeed(speed);
            _uiAnimation.Play(index, loop, length);
            if (!_graph.IsPlaying())
            {
                _graph.Play();
            }
        }

        
        public void Stop()
        {
            _graph.Stop();
        }
        
        private void AnimationEnd(int index)
        {
            _mixer.SetInputWeight(index, 0);
            _graph.Stop();
            OnAnimationEnd?.Invoke(index);
        }
        
        public void DeInitialize()
        {
            _uiAnimation.OnAnimationEnd -= AnimationEnd;
            _graph.Stop();
            _mixer.Destroy();
            _playable.Destroy();
            _graph.Destroy();
            _graph = default;
            _uiAnimation = default;
            _mixer = default;
            _output = default;
            _playable = default;
        }
    }
}