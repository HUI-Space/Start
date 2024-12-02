using System;
using Start.Framework;

namespace Start.Runtime
{
    [Serializable]
    public class UIConfigItem:ConfigItemBase
    {
        public string   UIName;
        public string   UIDataName;
        public EUIType   euiType;
        public bool     Preload;
    }
    
    [Serializable]
    public partial class UIConfig:ConfigBase<UIConfigItem>
    {
        
    }
}