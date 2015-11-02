using AppSettings;
using System;
using System.Collections.Generic;

namespace AppSettingsTests
{
    internal class SettingLoaderMock : ISettingLoader
    {
        /// <summary>
        /// Insert values into this dictionary which you want this mock setting loader to return
        /// </summary>
        public IDictionary<string, string> Settings { get; private set; }

        public SettingLoaderMock()
        {
            Settings = new Dictionary<string, string>();
        }

        public string Load(string settingName)
        {
            if (!Settings.ContainsKey(settingName))
            {
                return null;
            }

            return Settings[settingName];
        }
    }
}