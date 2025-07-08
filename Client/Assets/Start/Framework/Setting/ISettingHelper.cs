namespace Start
{
    /// <summary>
    /// 定义了一个接口，用于管理应用程序的设置
    /// </summary>
    public interface ISettingHelper
    {
        /// <summary>
        /// 删除指定的设置项
        /// </summary>
        /// <param name="settingName">要删除的设置项的名称</param>
        void DeleteKey(string settingName);
    
        /// <summary>
        /// 删除所有的设置项
        /// </summary>
        void DeleteAll();
    
        /// <summary>
        /// 检查是否存在指定名称的设置项
        /// </summary>
        /// <param name="settingName">要检查的设置项的名称</param>
        /// <returns>如果存在指定名称的设置项，则返回true；否则返回false</returns>
        bool HasSetting(string settingName);
    
        /// <summary>
        /// 获取指定设置项的布尔值
        /// 如果设置项不存在，则返回默认值
        /// </summary>
        /// <param name="settingName">设置项的名称</param>
        /// <param name="defaultValue">如果设置项不存在时返回的默认值</param>
        /// <returns>设置项的布尔值或默认值</returns>
        bool GetBool(string settingName, bool defaultValue);
    
        /// <summary>
        /// 获取指定设置项的整数值
        /// 如果设置项不存在，则返回默认值
        /// </summary>
        /// <param name="settingName">设置项的名称</param>
        /// <param name="defaultValue">如果设置项不存在时返回的默认值</param>
        /// <returns>设置项的整数值或默认值</returns>
        int GetInt(string settingName, int defaultValue);
    
        /// <summary>
        /// 获取指定设置项的浮点值
        /// 如果设置项不存在，则返回默认值
        /// </summary>
        /// <param name="settingName">设置项的名称</param>
        /// <param name="defaultValue">如果设置项不存在时返回的默认值</param>
        /// <returns>设置项的浮点值或默认值</returns>
        float GetFloat(string settingName, float defaultValue);
    
        /// <summary>
        /// 获取指定设置项的字符串值
        /// 如果设置项不存在，则返回默认值
        /// </summary>
        /// <param name="settingName">设置项的名称</param>
        /// <param name="defaultValue">如果设置项不存在时返回的默认值</param>
        /// <returns>设置项的字符串值或默认值</returns>
        string GetString(string settingName, string defaultValue);
    
        /// <summary>
        /// 设置指定设置项的布尔值
        /// </summary>
        /// <param name="settingName">设置项的名称</param>
        /// <param name="value">要设置的布尔值</param>
        void SetBool(string settingName, bool value);
    
        /// <summary>
        /// 设置指定设置项的整数值
        /// </summary>
        /// <param name="settingName">设置项的名称</param>
        /// <param name="value">要设置的整数值</param>
        void SetInt(string settingName, int value);
    
        /// <summary>
        /// 设置指定设置项的浮点值
        /// </summary>
        /// <param name="settingName">设置项的名称</param>
        /// <param name="value">要设置的浮点值</param>
        void SetFloat(string settingName, float value);
    
        /// <summary>
        /// 设置指定设置项的字符串值
        /// </summary>
        /// <param name="settingName">设置项的名称</param>
        /// <param name="value">要设置的字符串值</param>
        void SetString(string settingName, string value);
    }
}