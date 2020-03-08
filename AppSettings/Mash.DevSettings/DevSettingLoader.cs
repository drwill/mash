using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Mash.AppSettings.DevSettings
{
    /// <summary>
    /// A setting loader that gets individual developer-specific settings for effortless local debugging
    /// </summary>
    public class DevSettingLoader : ISettingLoader
    {
        private Dictionary<string, dynamic> _devSettings = new Dictionary<string, dynamic>();
        private Dictionary<string, string> _connectionStrings = new Dictionary<string, string>();

        /// <summary>
        /// The path to the developer's setting file
        /// </summary>
        public string DevSettingFile { get; private set; }

        /// <summary>
        /// Creates an instance of DevSettingLoader
        /// </summary>
        /// <param name="dir">The path to files holding developer-specific settings, defaults to current directory</param>
        /// <param name="fileName">The developer's file to load, defaults to %username%.json</param>
        public DevSettingLoader(string dir = null, string fileName = null)
        {
            if (String.IsNullOrEmpty(dir))
            {
                dir = Environment.CurrentDirectory;
            }

            string fileType = ".json";
            if (String.IsNullOrWhiteSpace(fileName))
            {
                fileName = $"{Environment.UserName}{fileType}";
            }

            if (!fileName.EndsWith(fileType))
            {
                throw new ArgumentException("Mash.AppSettings.DevSettingLoader: Only JSON files are valid.", nameof(fileName));
            }

            DevSettingFile = Path.Combine(dir, fileName);
            Trace.TraceInformation($"Mash.AppSettings.DevSettingLoader: using dev setting file [{DevSettingFile}]");

            string fileContents = "";
            try
            {
                fileContents = File.ReadAllText(DevSettingFile);
            }
            catch (Exception ex)
            {
                Trace.TraceWarning($"Mash.AppSettings.DevSettingLoader: Failed to load file {DevSettingFile} due to {ex}");
            }

            LoadSettings(fileContents);
        }

        /// <summary>
        /// Creates an instance of DevSetingLoader
        /// </summary>
        /// <param name="json">The json holding developer-specific settings</param>
        public DevSettingLoader(string json)
        {
            LoadSettings(json);
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
        /// Gets the specified setting
        /// </summary>
        /// <param name="settingKey">The name of the setting</param>
        /// <returns>The string value of the setting</returns>
        public string GetSetting(string settingKey)
        {
            dynamic value;
            if (!_devSettings.TryGetValue(settingKey, out value))
            {
                return null;
            }

            return value.ToString();
        }

        private void LoadSettings(string json)
        {
            if (String.IsNullOrWhiteSpace(json))
            {
                return;
            }
            
            _devSettings = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(json);

            dynamic connectionStrings;
            if (_devSettings.TryGetValue("ConnectionStrings", out connectionStrings))
            {
                _connectionStrings = JsonSerializer.Deserialize<Dictionary<string, string>>(connectionStrings.ToString());
            }
            else
            {
                _connectionStrings = new Dictionary<string, string>();
            }
        }
    }
}
