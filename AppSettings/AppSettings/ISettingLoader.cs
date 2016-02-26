using System.Collections;
using System.Collections.Generic;

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
        string GetSetting(string settingKey);

        /// <summary>
        /// Loads a connection string from the source
        /// </summary>
        /// <param name="connectionStringKey">The key of the connection string to load</param>
        /// <returns>The connection string</returns>
        string GetConnectionString(string connectionStringKey);

        /// <summary>
        /// Loads all connection strings from the source
        /// </summary>
        /// <returns>
        /// A dictionary of connection strings where key is the name and the value is the connection string
        /// </returns>
        IDictionary<string, string> GetConnectionStrings();
    }
}