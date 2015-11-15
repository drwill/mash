﻿using System;
using System.Collections.Generic;
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
        public string GetSetting(string settingKey)
        {
            if (String.IsNullOrWhiteSpace(settingKey))
            {
                throw new ArgumentNullException("settingName", "The parameter cannot be null or empty");
            }

            return ConfigurationManager.AppSettings[settingKey];
        }

        /// <summary>
        /// Loads all connection strings from the config file
        /// </summary>
        /// <returns>
        /// A dictionary of connection strings where key is the name and the value is the connection string
        /// </returns>
        public IDictionary<string, string> GetConnectionStrings()
        {
            var connectionStrings = new Dictionary<string, string>();
            foreach (ConnectionStringSettings item in ConfigurationManager.ConnectionStrings)
            {
                connectionStrings.Add(item.Name, item.ConnectionString);
            }

            return connectionStrings;
        }
    }
}