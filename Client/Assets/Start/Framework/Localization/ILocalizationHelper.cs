namespace Start
{
    /// <summary>
    /// 定义一个接口，用于处理应用程序中的语言切换和本地化字符串管理。
    /// </summary>
    public interface ILocalizationHelper
    {
        /// <summary>
        /// 切换当前应用程序的本地化设置。
        /// </summary>
        /// <param name="LocalizationId">要切换到的语言ID。</param>
        /// <returns>如果语言切换成功，则返回true；否则返回false。</returns>
        bool ChangeLocalization(int LocalizationId);
    
        /// <summary>
        /// 检查是否存在具有指定键的本地化字符串。
        /// </summary>
        /// <param name="key">要检查的字符串键。</param>
        /// <returns>如果存在具有指定键的字符串，则返回true；否则返回false。</returns>
        bool HasString(long key);
    
        /// <summary>
        /// 获取具有指定键的本地化字符串。如果不存在，则返回默认值。
        /// </summary>
        /// <param name="key">要获取的字符串键。</param>
        /// <param name="defaultValue">如果未找到键，则返回的默认字符串值。默认为null。</param>
        /// <returns>返回与键关联的本地化字符串，或默认值（如果未找到键）。</returns>
        string GetString(long key, string defaultValue = default);
    }
}