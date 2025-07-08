using System;

namespace Start
{
    [Serializable]
    public abstract class ConfigItemBase : IConfigItem
    {
        public long Id;
    }
}