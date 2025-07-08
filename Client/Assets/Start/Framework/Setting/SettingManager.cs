using System.Threading.Tasks;


namespace Start
{
    public class SettingManager :ManagerBase<SettingManager>
    {
        public override int Priority => 11;
        private ISettingHelper _settingHelper;

        public override Task Initialize()
        {
            _settingHelper = Helper.CreateHelper<ISettingHelper>();
            return base.Initialize();
        }

        public override Task DeInitialize()
        {
            _settingHelper = default;
            return base.DeInitialize();
        }

        #region API

        public void DeleteKey(string settingName)
        {
            _settingHelper.DeleteKey(settingName);
        }

        public void DeleteAll()
        {
            _settingHelper.DeleteAll();
        }

        public bool HasSetting(string settingName)
        {
            return _settingHelper.HasSetting(settingName);
        }

        public bool GetBool(string settingName, bool defaultValue)
        {
            return _settingHelper.GetBool(settingName,defaultValue);
        }
        
        public void SetBool(string settingName, bool value)
        {
            _settingHelper.SetBool(settingName,value);
        }

        public int GetInt(string settingName, int defaultValue)
        {
            return _settingHelper.GetInt(settingName, defaultValue);
        }
        
        public void SetInt(string settingName, int value)
        {
            _settingHelper.SetInt(settingName, value);
        }

        public float GetFloat(string settingName, float defaultValue)
        {
            return _settingHelper.GetFloat(settingName, defaultValue);
        }
        
        public void SetFloat(string settingName, float value)
        {
            _settingHelper.SetFloat(settingName, value);
        }

        public string GetString(string settingName, string defaultValue)
        {
            return _settingHelper.GetString(settingName, defaultValue);
        }

        public void SetString(string settingName, string value)
        {
            _settingHelper.SetString(settingName, value);
        }
        
        #endregion
    }
}