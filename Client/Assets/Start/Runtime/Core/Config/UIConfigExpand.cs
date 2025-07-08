using System.Collections.Generic;

namespace Start
{
    public partial class UIConfig
    {
        private Dictionary<string, UIConfigItem> _uiConfigDictionary = new Dictionary<string, UIConfigItem>();
        public override void Initialize()
        {
            base.Initialize();
            foreach (var item in DataList)
            {
                _uiConfigDictionary.Add(item.UIName, item);
            }
        }

        public bool GetUIConfigItem(string uiName, out UIConfigItem uiConfigItem)
        {
            return _uiConfigDictionary.TryGetValue(uiName, out uiConfigItem);
        }

        public override void DeInitialize()
        {
            _uiConfigDictionary.Clear();
            base.DeInitialize();
        }
    }
}