using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Mash.AppSettings.DevSettings
{
    public class DevSettingLoader : ISettingLoader
    {
        public string DevSettingFile { get; private set; }
        private Dictionary<string, dynamic> _devSettings = new Dictionary<string, dynamic>();
        private Dictionary<string, string> _connectionStrings = new Dictionary<string, string>();

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

        private void LoadSettings(string json)
        {
            if (String.IsNullOrWhiteSpace(json))
            {
                return;
            }
            
            _devSettings = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(json);

            dynamic connectionStrings;
            if (_devSettings.TryGetValue("ConnectionStrings", out connectionStrings))
            {
                _connectionStrings = JsonConvert.DeserializeObject<Dictionary<string, string>>(connectionStrings.ToString());
            }
            else
            {
                _connectionStrings = new Dictionary<string, string>();
            }
        }

        public string GetConnectionString(string connectionStringKey)
        {
            string result;
            _connectionStrings.TryGetValue(connectionStringKey, out result);

            return result;
        }

        public IDictionary<string, string> GetConnectionStrings()
        {
            return _connectionStrings;
        }

        public string GetSetting(string settingKey)
        {
            dynamic value;
            if (!_devSettings.TryGetValue(settingKey, out value))
            {
                return null;
            }

            return value.ToString();
        }
    }
}
