namespace Start
{
    public interface IAudioHelper
    {
        /// <summary>
        /// 初始化音频系统
        /// </summary>
        void Initialize();
        
        /// <summary>
        /// 反初始化音频系统，释放相关资源
        /// </summary>
        void DeInitialize();
        
        /// <summary>
        /// 更新音频系统状态
        /// </summary>
        /// <param name="elapseSeconds">逻辑上经过的秒数</param>
        /// <param name="realElapseSeconds">实际上经过的秒数</param>
        void Update(float elapseSeconds, float realElapseSeconds);
        
        /// <summary>
        /// 播放指定类型的音频
        /// </summary>
        /// <param name="audioType">音频类型</param>
        /// <param name="audioName">音频名称</param>
        /// <param name="isLoop">是否循环播放</param>
        /// <param name="volume">音量大小，范围[0,1]</param>
        /// <param name="fadeIn">淡入时间，单位秒</param>
        /// <param name="fadeOut">淡出时间，单位秒</param>
        void PlayAudio(int audioType, string audioName, bool isLoop, float volume, float fadeIn, float fadeOut);
        
        /// <summary>
        /// 改变指定类型音频的音量
        /// </summary>
        /// <param name="audioType">音频类型</param>
        /// <param name="volume">新的音量大小，范围[0,1]</param>
        void ChangeVolume(int audioType, float volume);
        
        /// <summary>
        /// 暂停指定类型音频的播放
        /// </summary>
        /// <param name="audioType">音频类型</param>
        void PauseAudio(int audioType);
        
        /// <summary>
        /// 恢复指定类型音频的播放
        /// </summary>
        /// <param name="audioType">音频类型</param>
        void ResumeAudio(int audioType);
        
        /// <summary>
        /// 停止指定类型音频的播放
        /// </summary>
        /// <param name="audioType">音频类型</param>
        void StopAudio(int audioType);
        
        /// <summary>
        /// 停止所有类型音频的播放
        /// </summary>
        void StopAllAudio();
    }
}