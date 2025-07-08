using System.Collections.Generic;

namespace Start
{
    public class AudioSetting
    {
        public float Volume;
        public bool IsLoop;
    }
    
    public class AudioController : SingletonBase<AudioController>
    {
        
        private int _index;
        private List<string> _backgrounds;
        private Dictionary<EAudioType,AudioSetting> _audioPlayers;
        
        public override void Initialize()
        {
            _index = 0;
            _backgrounds = new List<string>();
            _audioPlayers = new Dictionary<EAudioType, AudioSetting>();
            
            RuntimeEvent.AddListener((int)EMessageId.AudioEnd,OnAudioEnd);
            
            foreach (AudioConfigItem item in ConfigManager.Instance.GetConfig<AudioConfig>().DataList)
            {
                if (item.AudioType == EAudioType.BackGround)
                {
                    _backgrounds.Add(AssetConfig.GetAssetPath(EAssetType.Audio,item.AudioName));
                }
            }
            
            //可根据业务来设置
            _audioPlayers.Add(EAudioType.BackGround,new AudioSetting{Volume = 0.5f,IsLoop = false});
            _audioPlayers.Add(EAudioType.UI,new AudioSetting{Volume = 1f,IsLoop = false});
            
        }

        public override void DeInitialize()
        {
            RuntimeEvent.RemoveListener((int)EMessageId.AudioEnd,OnAudioEnd);
            _backgrounds.Clear();
            _backgrounds = default;
        }
        
        public float GetVolume(EAudioType audioType)
        {
            if (_audioPlayers.TryGetValue(audioType, out var audioSetting))
            {
                return audioSetting.Volume;
            }
            return 1F;
        }
        
        public void PlayAudio(EAudioType audioType, string audioName)
        {
            if (_audioPlayers.TryGetValue(audioType, out var audioSetting))
            {
                PlayAudio(audioType, audioName, audioSetting.IsLoop, audioSetting.Volume,0f,0f);
            }
        }

        public void PlayAudio(EAudioType audioType, string audioName, bool isLoop)
        {
            if (_audioPlayers.TryGetValue(audioType, out var audioSetting))
            {
                PlayAudio(audioType, audioName, isLoop, audioSetting.Volume,0f,0f);
            }
        }
        
        public void PlayAudio(EAudioType audioType, string audioName, bool isLoop, float volume)
        {
            PlayAudio(audioType, audioName, isLoop, volume,0f,0f);
        }
        
        public void PlayAudio(EAudioType audioType, string audioName, bool isLoop, float volume, float fadeIn)
        {
            PlayAudio(audioType, audioName, isLoop, volume, fadeIn, 0F);
        }

        public void PlayAudio(EAudioType audioType, string audioName, bool isLoop, float volume, float fadeIn, float fadeOut)
        {
            AudioManager.Instance.PlayAudio((int)audioType, audioName, isLoop, volume, fadeIn, fadeOut);
        }
        
        private void OnAudioEnd(IGenericData genericData)
        {
            EAudioType audioType = genericData.GetData1<EAudioType>();
            bool isLoop = genericData.GetData2<bool>();
            
            if (audioType == EAudioType.BackGround)
            {
                if (!isLoop)
                {
                    string audioName = genericData.GetData3<string>();
                    for (int i = 0; i < _backgrounds.Count; i++)
                    {
                        if (audioName.Equals(_backgrounds[i]))
                        {
                            _index = i;
                            if (_index == _backgrounds.Count)
                            {
                                _index = 0;
                            }
                        }
                    }
                    ChangeBackGround(_index);
                }
            }
        }
        
        public void ChangeVolume(EAudioType audioType, float volume)
        {
            if (_audioPlayers.TryGetValue(audioType, out var audioSetting))
            {
                audioSetting.Volume = volume;
            }
            AudioManager.Instance.ChangeVolume((int)audioType, volume);
        }
        
        public void ResumeAudio(EAudioType audioType)
        {
            AudioManager.Instance.ResumeAudio((int)audioType);
        }
        
        public void PauseAudio(EAudioType audioType)
        {
            AudioManager.Instance.PauseAudio((int)audioType);
        }
        
        
        public void PlayBackGround()
        {
            if (_backgrounds.Count > 0)
            {
                PlayAudio(EAudioType.BackGround, _backgrounds[_index]);
            }
        }
        
        public void ChangeBackGround()
        {
            if (_backgrounds.Count > 0)
            {
                _index++;
                if (_index == _backgrounds.Count)
                {
                    _index = 0;
                }
                PlayAudio(EAudioType.BackGround, _backgrounds[_index]);
            }
        }
        
        public void ChangeBackGround(int index)
        {
            if (_backgrounds.Count > 0)
            {
                PlayAudio(EAudioType.BackGround, _backgrounds[index]);
            }
        }
        
        public void RandomChangeBackGround()
        {
            if (_backgrounds.Count > 0)
            {
                _index = UnityEngine.Random.Range(0, _backgrounds.Count);
                PlayAudio(EAudioType.BackGround, _backgrounds[_index]);
            }
        }
    }
}