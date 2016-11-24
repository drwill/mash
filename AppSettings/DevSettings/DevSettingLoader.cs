using System;
using System.Collections.Generic;
using System.IO;

namespace Mash.AppSettings.DevSettings
{
    public class DevSettingLoader : ISettingLoader
    {
        public string DevSettingFile { get; private set; }

        /// <summary>
        /// Creates an instance of DevSettingLoader
        /// </summary>
        /// <param name="pathToDevSettingsFiles">The path to files holding developer-specific settings, defaults to current directory</param>
        /// <param name="devSettingsFile">The developer's file to load, defaults to %username%.json</param>
        public DevSettingLoader(string pathToDevSettingsFiles = null, string devSettingsFile = null)
        {
            if (String.IsNullOrEmpty(pathToDevSettingsFiles))
            {
                pathToDevSettingsFiles = Environment.CurrentDirectory;
            }

            if (String.IsNullOrWhiteSpace(devSettingsFile))
            {
                devSettingsFile = $"{Environment.UserName}.json";
            }

            DevSettingFile = Path.Combine(pathToDevSettingsFiles, devSettingsFile);
        }

        public string GetConnectionString(string connectionStringKey)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, string> GetConnectionStrings()
        {
            throw new NotImplementedException();
        }

        public string GetSetting(string settingKey)
        {
            throw new NotImplementedException();
        }
    }
}
