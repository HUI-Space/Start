using System.Collections.Generic;

namespace Start.Runtime
{
    public partial class UIConfig
    {
        private Dictionary<string, UIConfigItem> _uiConfigDictionary = new Dictionary<string, UIConfigItem>();
        public readonly HashSet<string> PreloadUINames = new HashSet<string>();
        public override void Initialize()
        {
            base.Initialize();
            foreach (var item in DataList)
            {
                _uiConfigDictionary.Add(item.UIName, item);
                if (item.Preload)
                {
                    PreloadUINames.Add(item.UIName);
                }
            }
        }

        public bool GetUIConfig(string uiName, out UIConfigItem uiConfigItem)
        {
            return _uiConfigDictionary.TryGetValue(uiName, out uiConfigItem);
        }

        public override void DeInitialize()
        {
            _uiConfigDictionary.Clear();
            PreloadUINames.Clear();
            base.DeInitialize();
        }
    }
}