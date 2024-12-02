namespace Start.Framework
{
    public interface IAudioHelper
    {
        void Initialize();
        void DeInitialize();
        void Update(float elapseSeconds, float realElapseSeconds);
        void PlayAudio(int audioType, string audioName, bool isLoop, float volume, float fadeIn, float fadeOut);
        void ChangeVolume(int audioType, float volume);
        void PauseAudio(int audioType);
        void ResumeAudio(int audioType);
        void StopAudio(int audioType);
        void StopAllAudio();
    }
}