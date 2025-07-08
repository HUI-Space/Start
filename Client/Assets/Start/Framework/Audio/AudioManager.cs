using System.Collections.Generic;
using System.Threading.Tasks;

namespace Start
{
    public class AudioManager : ManagerBase<AudioManager>, IUpdateManger
    {
        public override int Priority => 7;

        private IAudioHelper _audioHelper;
        

        public override Task Initialize()
        {
            _audioHelper = Helper.CreateHelper<IAudioHelper>();
            _audioHelper.Initialize();
            return base.Initialize();
        }

        public override Task DeInitialize()
        {
            _audioHelper.StopAllAudio();
            _audioHelper.DeInitialize();
            _audioHelper = default;
            return base.DeInitialize();
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            _audioHelper.Update(elapseSeconds, realElapseSeconds);
        }

        /// <summary>
        /// 播放音频。
        /// </summary>
        /// <param name="audioType">音频类型。</param>
        /// <param name="audioName">音频名称。</param>
        /// <param name="isLoop">是否循环播放，默认为false。</param>
        /// <param name="volume">音量大小，默认为1。</param>
        /// <param name="fadeIn">淡入时间，默认为0。</param>
        /// <param name="fadeOut">淡出时间，默认为0。</param>
        public void PlayAudio(int audioType, string audioName, bool isLoop = false, float volume = 1f, float fadeIn = 0,
            float fadeOut = 0)
        {
            _audioHelper.PlayAudio(audioType, audioName, isLoop, volume, fadeIn, fadeOut);
        }
        
        /// <summary>
        /// 改变指定音频类型的音量。
        /// </summary>
        /// <param name="audioType">音频类型。</param>
        /// <param name="volume">新的音量大小。</param>
        public void ChangeVolume(int audioType, float volume)
        {
            _audioHelper.ChangeVolume(audioType, volume);
        }
        
        /// <summary>
        /// 暂停指定类型的音频播放。
        /// </summary>
        /// <param name="audioType">音频类型。</param>
        public void PauseAudio(int audioType)
        {
            _audioHelper.PauseAudio(audioType);
        }
        
        /// <summary>
        /// 恢复指定类型的音频播放。
        /// </summary>
        /// <param name="audioType">音频类型。</param>
        public void ResumeAudio(int audioType)
        {
            _audioHelper.ResumeAudio(audioType);
        }
        
        /// <summary>
        /// 停止指定类型的音频播放。
        /// </summary>
        /// <param name="audioType">音频类型。</param>
        public void StopAudio(int audioType)
        {
            _audioHelper.StopAudio(audioType);
        }
        
        /// <summary>
        /// 停止所有类型的音频播放。
        /// </summary>
        public void StopAllAudio()
        {
            _audioHelper.StopAllAudio();
        }
    }
}