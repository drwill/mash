using System.Collections.Generic;

namespace Mash.AppSettings.Tests
{
    internal class SettingLoaderMock : ISettingLoader
    {
        /// <summary>
        /// Insert values into this dictionary which you want this mock setting loader to return
        /// </summary>
        public IDictionary<string, string> Settings { get; private set; }

        public IDictionary<string, string> ConnectionStrings { get; private set; }

        public SettingLoaderMock()
        {
            Settings = new Dictionary<string, string>();
            ConnectionStrings = new Dictionary<string, string>();
        }

        public string GetSetting(string settingName)
        {
            if (!Settings.ContainsKey(settingName))
            {
                return null;
            }

            return Settings[settingName];
        }

        public IDictionary<string, string> GetConnectionStrings()
        {
            return ConnectionStrings;
        }
    }
}