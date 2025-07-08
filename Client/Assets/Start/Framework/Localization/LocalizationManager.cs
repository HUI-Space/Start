using System;
using System.Threading.Tasks;

namespace Start
{
    public sealed class LocalizationManager : ManagerBase<LocalizationManager>
    {
        public override int Priority => 14;
        public int LanguageId { get; private set; }
        
        public event Action LocalizationChanged;

        private ILocalizationHelper _localizationHelper;
        
        public override Task Initialize()
        {
            _localizationHelper = Helper.CreateHelper<ILocalizationHelper>();
            return base.Initialize();
        }

        public override Task DeInitialize()
        {
            _localizationHelper = default;
            return base.DeInitialize();
        }
        
        #region API

        public bool ChangeLocalization(int languageId)
        {
            if (LanguageId == languageId)
            {
                return default;
            }
            LanguageId = languageId;
            bool result = _localizationHelper.ChangeLocalization(LanguageId);
            LocalizationChanged?.Invoke();
            return result;
        }
        
        public bool HasString(long key)
        {
            return _localizationHelper.HasString(key);
        }

        public string GetString(long key,string defaultValue = default)
        {
            return _localizationHelper.GetString(key,defaultValue);
        }
        
        #endregion
    }
}