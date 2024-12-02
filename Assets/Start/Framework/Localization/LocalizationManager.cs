namespace Start.Framework
{
    [Manager]
    public class LocalizationManager:ManagerBase<LocalizationManager>
    {
        public override int Priority => 14;
        public int LanguageId { get; private set; }

        private ILocalizationHelper _localizationHelper;
        
        public static void SetHelper(ILocalizationHelper localizationHelper)
        {
            Instance._localizationHelper = localizationHelper;
        }
        
        #region API

        public bool ChangeLanguage(int languageId)
        {
            if (LanguageId == languageId)
            {
                return default;
            }
            LanguageId = languageId;
            return _localizationHelper.ChangeLanguage(LanguageId);
        }
        
        public bool HasString(string key)
        {
            return _localizationHelper.HasString(key);
        }

        public string GetString(string key,string defaultValue = default)
        {
            return _localizationHelper.GetString(key,defaultValue);
        }
        
        #endregion
    }
}