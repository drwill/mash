using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Mash.AppSettings
{
    internal class JsonSettingLoader : ISettingLoader
    {
        private Dictionary<string, string> _connectionStrings = new Dictionary<string, string>();
        private Dictionary<string, dynamic> _settings = new Dictionary<string, dynamic>();

        /// <summary>
        /// Creates an instance of DevSettingLoader
        /// </summary>
        /// <param name="pathToJsonSettingsFile">The path to developer's file to load holding developer-specific settings, defaults to CurrentDirectory\%username%.json</param>
        public JsonSettingLoader(string pathToJsonSettingsFile)
        {
            if (String.IsNullOrEmpty(pathToJsonSettingsFile) ||
                !File.Exists(pathToJsonSettingsFile))
            {
                throw new ArgumentException($"Unable to find file {pathToJsonSettingsFile}", nameof(pathToJsonSettingsFile));
            }

            string fileContents = File.ReadAllText(pathToJsonSettingsFile);

            LoadSettings(fileContents);
        }

        /// <summary>
        /// Gets the specified connection string
        /// </summary>
        /// <param name="connectionStringKey">The name of the connection string</param>
        /// <returns>The connection string</returns>
        public string GetConnectionString(string connectionStringKey)
        {
            _connectionStrings.TryGetValue(connectionStringKey, out string result);

            return result;
        }

        /// <summary>
        /// Gets all connection strings
        /// </summary>
        /// <returns>A dictionary of all connection strings</returns>
        public IDictionary<string, string> GetConnectionStrings()
        {
            return _connectionStrings;
        }

        /// <summary>
        /// Gets the specified setting by name
        /// </summary>
        /// <param name="settingKey">The name of the setting</param>
        /// <returns>The setting value</returns>
        public string GetSetting(string settingKey)
        {
            if (!_settings.TryGetValue(settingKey, out dynamic value))
            {
                return null;
            }

            return value.ToString();
        }

        private void LoadSettings(string json)
        {
            _settings = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(json);

            dynamic connectionStrings;
            if (_settings.TryGetValue("ConnectionStrings", out connectionStrings))
            {
                _connectionStrings = JsonConvert.DeserializeObject<Dictionary<string, string>>(connectionStrings.ToString());
            }
            else
            {
                _connectionStrings = new Dictionary<string, string>();
            }
        }
    }
}