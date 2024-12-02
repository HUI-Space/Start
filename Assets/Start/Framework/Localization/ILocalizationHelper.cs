namespace Start.Framework
{
    public interface ILocalizationHelper
    {
        bool ChangeLanguage(int languageId);
        bool HasString(string key);
        string GetString(string key, string defaultValue = default);
    }
}