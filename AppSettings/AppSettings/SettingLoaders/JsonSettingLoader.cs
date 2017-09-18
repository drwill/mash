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
        /// <param name="dir">The path to files holding developer-specific settings, defaults to current directory</param>
        /// <param name="fileName">The developer's file to load, defaults to %username%.json</param>
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

        public string GetConnectionString(string connectionStringKey)
        {
            _connectionStrings.TryGetValue(connectionStringKey, out string result);

            return result;
        }

        public IDictionary<string, string> GetConnectionStrings()
        {
            return _connectionStrings;
        }

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