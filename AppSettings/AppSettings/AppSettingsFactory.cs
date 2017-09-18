using System;

namespace Mash.AppSettings
{
    /// <summary>
    /// Public factory for AppSettings objects
    /// </summary>
    public static class AppSettingsFactory
    {
        /// <summary>
        /// Gets a setting loader for .NET application's configuration file
        /// </summary>
        public static ISettingLoader GetAppConfigSettingLoader()
        {
            return new AppConfigSettingLoader();
        }

        /// <summary>
        /// Gets a setting loader for Json files
        /// </summary>
        /// <param name="pathToJsonSettingsFile">The full path to the json settings file</param>
        public static ISettingLoader GetJsonSettingLoader(string pathToJsonSettingsFile)
        {
            return new JsonSettingLoader(pathToJsonSettingsFile);
        }
    }
}