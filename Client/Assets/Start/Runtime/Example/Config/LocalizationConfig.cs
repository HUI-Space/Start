using System;

namespace Start
{
    [Serializable]
    public class LocalizationConfigItem : ConfigItemBase
    {
        public string Text;
    }

    [Serializable]
    public class LocalizationConfig : ConfigBase<LocalizationConfigItem>
    {
        
    }
}