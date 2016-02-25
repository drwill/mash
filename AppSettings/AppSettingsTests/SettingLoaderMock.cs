using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mash.AppSettings.Tests
{
    internal class SettingLoaderMock : ISettingLoader
    {
        /// <summary>
        /// Insert values into this dictionary which you want this mock setting loader to return
        /// </summary>
        public IDictionary<string, string> Settings { get; private set; }

        public IDictionary<string, string> ConnectionStrings { get; private set; }

        public IList ArrayListString { get; private set; }

        public SettingLoaderMock()
        {
            Settings = new Dictionary<string, string>();
            ConnectionStrings = new Dictionary<string, string>();
            ArrayListString = new List<string>();
        }

        public string GetSetting(string settingName)
        {
            if (!Settings.ContainsKey(settingName))
            {
                return null;
            }

            return Settings[settingName];
        }

        public string GetConnectionString(string connectionStringKey)
        {

            if (!ConnectionStrings.ContainsKey(connectionStringKey))
            {
                return null;
            }

            return ConnectionStrings[connectionStringKey];
        }

        public IDictionary<string, string> GetConnectionStrings()
        {
            return ConnectionStrings;
        }

        public IList GetArrayList(string settingKey)
        {
            var settingStringValue = GetSetting(settingKey);

            if (string.IsNullOrEmpty(settingStringValue))
            {
                return null;
            }

            List<string> retValue = new List<string>();

            return settingStringValue.Split(',').ToArray();
        }
    }
}