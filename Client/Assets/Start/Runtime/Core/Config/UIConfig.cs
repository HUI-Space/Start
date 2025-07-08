using System;


namespace Start
{
    [Serializable]
    public class UIConfigItem : ConfigItemBase
    {
        public string UIName;
        public EUIType UIType;
        public string UIDataName;
    }

    [Serializable]
    public partial class UIConfig : ConfigBase<UIConfigItem>
    {
    }
}