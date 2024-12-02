namespace Start.Framework
{
    public interface ISettingHelper
    {
        void DeleteKey(string settingName);
        void DeleteAll();
        bool HasSetting(string settingName);
        bool GetBool(string settingName, bool defaultValue);
        int GetInt(string settingName, int defaultValue);
        float GetFloat(string settingName, float defaultValue);
        string GetString(string settingName, string defaultValue);
        void SetBool(string settingName, bool value);
        void SetInt(string settingName, int value);
        void SetFloat(string settingName, float value);
        void SetString(string settingName, string value);
    }
}