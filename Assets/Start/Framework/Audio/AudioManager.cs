namespace Start.Framework
{
    [Manager]
    public class AudioManager:ManagerBase<AudioManager> ,IUpdateManger
    {
        public override int Priority => 7;
        
        private IAudioHelper _audioHelper;
        
        public static void SetHelper(IAudioHelper audioHelper)
        {
            Instance._audioHelper = audioHelper;
            Instance._audioHelper.Initialize();
        }
        
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            _audioHelper?.Update(elapseSeconds,realElapseSeconds);
        }

        public void PlayAudio(int audioType,string audioName, bool isLoop = false, float volume = 1f,float fadeIn = 0, float fadeOut = 0)
        {
            _audioHelper.PlayAudio(audioType,audioName,isLoop,volume,fadeIn,fadeOut);
        }

        public void ChangeVolume(int audioType, float volume)
        {
            _audioHelper.ChangeVolume(audioType,volume);
        }

        public void PauseAudio(int audioType)
        {
            _audioHelper.PauseAudio(audioType);
        }

        public void ResumeAudio(int audioType)
        {
            _audioHelper.ResumeAudio(audioType);
        }

        public void StopAudio(int audioType)
        {
            _audioHelper.StopAudio(audioType);
        }
        
        public void StopAllAudio()
        {
            _audioHelper.StopAllAudio();
        }
    }
}