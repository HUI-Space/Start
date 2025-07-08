using UnityEngine;

namespace Start
{
    public class SettingHelper : ISettingHelper
    {
        public void DeleteKey(string settingName)
        {
            PlayerPrefs.DeleteKey(settingName);
        }

        public void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }

        public bool HasSetting(string settingName)
        {
            return PlayerPrefs.HasKey(settingName);
        }

        public bool GetBool(string settingName, bool defaultValue)
        {
            int result = PlayerPrefs.GetInt(settingName);
            if (result == 0)
            {
                return defaultValue;
            }
            return result == 1;
        }

        public int GetInt(string settingName, int defaultValue)
        {
            return PlayerPrefs.GetInt(settingName, defaultValue);
        }

        public float GetFloat(string settingName, float defaultValue)
        {
            return PlayerPrefs.GetFloat(settingName, defaultValue);
        }

        public string GetString(string settingName, string defaultValue)
        {
            return PlayerPrefs.GetString(settingName, defaultValue);
        }

        public void SetBool(string settingName, bool value)
        {
            PlayerPrefs.SetInt(settingName, value ? 1 : 0);
        }

        public void SetInt(string settingName, int value)
        {
            PlayerPrefs.SetInt(settingName, value);
        }

        public void SetFloat(string settingName, float value)
        {
            PlayerPrefs.SetFloat(settingName, value);
        }

        public void SetString(string settingName, string value)
        {
            PlayerPrefs.SetString(settingName, value);
        }
    }
}