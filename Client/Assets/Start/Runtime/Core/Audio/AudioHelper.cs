using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Start
{
    public class AudioHelper : IAudioHelper
    {
        private Dictionary<int, AudioPlayer> _audioPlayers = new Dictionary<int, AudioPlayer>();

        public void Initialize()
        {
            Transform transform = Main.Root.Find("Audio");
            if (transform == null)
            {
                transform = new GameObject("Audio",typeof(AudioListener)).transform;
                transform.SetParent(Main.Root);
            }

            foreach (EAudioType audioType in Enum.GetValues(typeof(EAudioType)))
            {
                Transform child = transform.Find(audioType.ToString());
                if (child == null)
                {
                    child = new GameObject(audioType.ToString()).transform;
                    child.SetParent(transform);
                }

                AudioSource audioSource = child.gameObject.AddComponent<AudioSource>();
                AudioPlayer audioPlayer = new AudioPlayer(audioSource);
                _audioPlayers[(int)audioType] = audioPlayer;
            }
        }

        public void DeInitialize()
        {
            foreach (EAudioType audioType in Enum.GetValues(typeof(EAudioType)))
            {
                Object.Destroy(_audioPlayers[(int)audioType].AudioSource.gameObject);
            }

            _audioPlayers.Clear();
            Transform transform = Main.Root.Find("Audio");
            Object.Destroy(transform);
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            foreach (AudioPlayer audioPlayer in _audioPlayers.Values)
            {
                audioPlayer.Update(elapseSeconds, realElapseSeconds);
            }
        }

        public void PlayAudio(int audioType, string audioName, bool isLoop, float volume, float fadeIn, float fadeOut)
        {
            AudioPlayer audioPlayer = GetAudioPlayer(audioType);
            audioPlayer.PlayAudio(audioType, audioName, isLoop, volume, fadeIn, fadeOut);
        }

        public void ChangeVolume(int audioType, float volume)
        {
            AudioPlayer audioPlayer = GetAudioPlayer(audioType);
            audioPlayer.ChangeVolume(volume);
        }

        public void PauseAudio(int audioType)
        {
            AudioPlayer audioPlayer = GetAudioPlayer(audioType);
            audioPlayer.PauseAudio();
        }

        public void ResumeAudio(int audioType)
        {
            AudioPlayer audioPlayer = GetAudioPlayer(audioType);
            audioPlayer.ResumeAudio();
        }

        public void StopAudio(int audioType)
        {
            AudioPlayer audioPlayer = GetAudioPlayer(audioType);
            audioPlayer.StopAudio();
        }
        
        public void StopAllAudio()
        {
            foreach (AudioPlayer audioPlayer in _audioPlayers.Values)
            {
                audioPlayer.StopAudio();
            }
        }

        private AudioPlayer GetAudioPlayer(int audioType)
        {
            return _audioPlayers[audioType];
        }
    }
}