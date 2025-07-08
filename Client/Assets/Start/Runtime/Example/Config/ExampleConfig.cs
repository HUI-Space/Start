using System;

namespace Start
{
    [Serializable]
    public class ExampleConfigItem : ConfigItemBase
    {
        public string UIName;

        public int LocalizationId;
    }

    [Serializable]
    public class ExampleConfig : ConfigBase<ExampleConfigItem>
    {
    }
}