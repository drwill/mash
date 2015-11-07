using System;
using System.Configuration;

namespace Mash.AppSettings
{
    /// <summary>
    /// Loads the requested setting from the app configiguration file
    /// </summary>
    internal class AppConfigSettingLoader : ISettingLoader
    {
        /// <summary>
        /// Loads the specified setting
        /// </summary>
        /// <param name="settingKey">The key of the setting to load</param>
        /// <returns>The value</returns>
        public string Load(string settingKey)
        {
            if (String.IsNullOrWhiteSpace(settingKey))
            {
                throw new ArgumentNullException("settingName", "The parameter cannot be null or empty");
            }

            return ConfigurationManager.AppSettings[settingKey];
        }
    }
}