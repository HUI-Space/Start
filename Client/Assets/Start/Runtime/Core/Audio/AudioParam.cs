

namespace Start
{
    public class AudioParam : IReusable
    {
        public int AudioType { get; private set; }
        public string AudioName { get; private set; }
        public bool IsLoop { get; private set; }
        public float Volume { get;  set; }
        public float FadeIn { get; private set; }
        public float FadeOut { get; private set; }
        

        public static AudioParam Create(int audioType, string audioName, bool isLoop, float volume, float fadeIn, float fadeOut)
        {
            AudioParam audioParam = RecyclableObjectPool.Acquire<AudioParam>();
            audioParam.AudioType = audioType;
            audioParam.AudioName = audioName;
            audioParam.IsLoop = isLoop;
            audioParam.Volume = volume;
            audioParam.FadeIn = fadeIn;
            audioParam.FadeOut = fadeOut;
            return audioParam;
        }
        
        public void Reset()
        {
            AudioType = default;
            AudioName = default;
            IsLoop = default;
            Volume = default;
            FadeIn = default;
            FadeOut = default;
        }
    }
}