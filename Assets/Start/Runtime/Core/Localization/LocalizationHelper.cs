using System.Collections.Generic;
using Start.Framework;

namespace Start.Runtime
{
    public class LocalizationHelper:ILocalizationHelper
    {
        private readonly Dictionary<string, string> _languageDict = new Dictionary<string, string>();
        
        public bool ChangeLanguage(int languageId)
        {
            
            return false;
        }

        public bool HasString(string key)
        {
            return _languageDict.ContainsKey(key);
        }

        public string GetString(string key, string defaultValue = default)
        {
            if (_languageDict.TryGetValue(key,out string value))
            {
                return value;
            }
            return defaultValue;
        }
    }
}