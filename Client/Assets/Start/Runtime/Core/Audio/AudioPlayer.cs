using System;
using System.Collections.Generic;


using UnityEngine;

namespace Start
{
    public class AudioPlayer
    {
        private static readonly Dictionary<string,AsyncOperationHandle<AudioClip>> _audioClipCache = new Dictionary<string, AsyncOperationHandle<AudioClip>>();
        
        private EAudioState _state
        {
            get => _eAudioState;
            set
            {
                _lastEAudioState = _eAudioState;
                _eAudioState = value;
            }
        }

        public AudioSource AudioSource { get; private set; }
        private AudioParam _audioParam;
        private EAudioState _lastEAudioState;
        private EAudioState _eAudioState;
        private float _fadeTime;
        private float _runTime;

        private const float _updateInterval = 0.1f; // 每0.1秒更新一次
        private float _nextUpdateTime;
        
        public AudioPlayer(AudioSource audioSource)
        {
            AudioSource = audioSource;
        }
        
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (_state == EAudioState.Stop || _state == EAudioState.Pause)
            {
                return;
            }
            _runTime += elapseSeconds;
            _fadeTime += elapseSeconds;
            // 每0.1秒更新一次
            if (Time.time >= _nextUpdateTime)
            {
                _nextUpdateTime = Time.time + _updateInterval;
                
                switch (_state)
                {
                    case EAudioState.FadeIn:
                        FadeIn();
                        break;
                    case EAudioState.Running:
                        Running();
                        break;
                    case EAudioState.FadeOut:
                        FadeOut();
                        break;
                }
            }
        }

        #region API
        
        public async void PlayAudio(int audioType, string audioName, bool isLoop, float volume, float fadeIn, float fadeOut)
        {
            if (AudioSource.isPlaying)
            {
                StopAudio();
            }
            // 没有播放音频 加载音频
            if (!_audioClipCache.TryGetValue(audioName,out AsyncOperationHandle<AudioClip> handle))
            {
                handle = ResourceManager.Instance.LoadAssetAsync<AudioClip>(audioName);
                await handle.Task;
                _audioClipCache.Add(audioName,handle);
            }
            AudioClip clip = handle.Result;
            if (clip.length < fadeIn + fadeOut)
            {
                Logger.Info($"淡入淡出时间太长:{audioName}");
                fadeIn = 0;
                fadeOut = 0;
            }
            
            if (_audioParam != null)
            {
                ReferencePool.Release(_audioParam);
            }
            _audioParam = AudioParam.Create(audioType,audioName,isLoop,volume,fadeIn,fadeOut);
            AudioSource.clip = clip;
            AudioSource.loop = isLoop;
            
            Play();
        }

        private void Play()
        {
            _runTime = 0;
            _fadeTime = 0;
            AudioSource.Play();
            if (_audioParam.FadeIn > 0)
            {
                AudioSource.volume = 0;
                _state = EAudioState.FadeIn;
            }
            else
            {
                AudioSource.volume = _audioParam.Volume;
                _state = EAudioState.Running;
            }
        }

        public void ChangeVolume(float volume)
        {
            if (_audioParam != null)
            {
                _audioParam.Volume = volume;
            }
            if (_eAudioState != EAudioState.FadeIn || _eAudioState != EAudioState.FadeOut)
            {
                AudioSource.volume = volume;
            }
        }

        public void PauseAudio()
        {
            if (_eAudioState == EAudioState.Pause || _eAudioState == EAudioState.Stop)
            {
                return;
            }
            
            _state = EAudioState.Pause;
            AudioSource.Pause();
        }

        public void ResumeAudio()
        {
            if (_eAudioState != EAudioState.Pause)
            {
                return;
            }
            _state = _lastEAudioState;
            AudioSource.UnPause();
        }

        public void StopAudio()
        {
            if (_eAudioState == EAudioState.Stop)
            {
                return;
            }
            
            _state = EAudioState.Stop;
            AudioSource.Stop();
        }
        
        #endregion
        
        private void FadeIn()
        {
            AudioSource.volume = Mathf.Lerp(0, _audioParam.Volume, _fadeTime / _audioParam.FadeIn);
            if (_fadeTime >= _audioParam.FadeIn)
            {
                _fadeTime = 0;
                _state = EAudioState.Running;
                AudioSource.volume = _audioParam.Volume;
            }
        }

        private void Running()
        {
            if (_runTime + _audioParam.FadeOut < AudioSource.clip.length)
            {
                return;
            }

            if (_audioParam.FadeOut > 0)
            {
                _fadeTime = 0;
                _state = EAudioState.FadeOut;
            }
            else
            {
                SelectNextState();
            }
        }
        
        private void FadeOut()
        {
            AudioSource.volume = Mathf.Lerp(_audioParam.Volume, 0, _fadeTime / _audioParam.FadeOut);
            if (_fadeTime >= _audioParam.FadeOut)
            {
                SelectNextState();
            }
        }
        
        private void SelectNextState()
        {
            if (_audioParam.IsLoop)
            {
                Play();
            }
            else
            {
                _state = EAudioState.Stop;
                AudioSource.Stop();
            }
            IGenericData genericData = GenericData<EAudioType, bool,string>.Create
                ((EAudioType)_audioParam.AudioType,_audioParam.IsLoop,_audioParam.AudioName);
            RuntimeEvent.SendMessage((int)EMessageId.AudioEnd,genericData);
        }
    }
}