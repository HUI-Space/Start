using System;
using Start.Framework;

namespace Start.Script
{
    [Serializable]
    public class ParameterDataConfigItem:ConfigItemBase
    {
        public string ConfigField;
    }
    [Serializable]
    public class ParameterDataConfig:ConfigBase<ParameterDataConfigItem>
    {
        
    }
}