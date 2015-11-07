namespace Mash.AppSettings
{
    /// <summary>
    /// A provider to load a setting
    /// </summary>
    public interface ISettingLoader
    {
        /// <summary>
        /// Loads a setting from the source
        /// </summary>
        /// <param name="settingKey">The key of the setting to load</param>
        /// <returns>The value</returns>
        string Load(string settingKey);
    }
}