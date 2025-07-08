using System;

namespace Start
{
    [Serializable]
    public class AudioConfigItem : ConfigItemBase
    {
        /// <summary>
        /// 音频名字
        /// </summary>
        public string AudioName;

        public EAudioType AudioType;
    }

    [Serializable]
    public class AudioConfig : ConfigBase<AudioConfigItem>
    {
        
    }
}