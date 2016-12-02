using Mash.AppSettings;
using Mash.AppSettings.DevSettings;
using System;
using System.Collections.Generic;

namespace SampleWebApp.Models
{
    /// <summary>
    /// Settings required for the running of this application
    /// </summary>
    /// <remarks>
    /// This example does not use SingletonSettings as a base class because it wants to optionally load developer-specific
    /// settings.
    /// </remarks>
    [AppSetting]
    internal class Settings
    {
        public static Settings Instance { get; } = Initialize();

        private static Settings Initialize()
        {
            bool isDevMode = true; // Consider using an environment variable to indicate this
            if (isDevMode)
            {
                // Normally we'd load a file based on the username so each developer would get their own settings,
                // but for this example we'll specify the file name so everyone can see how this override works
                AppSettingsLoader.DevSettings = new DevSettingLoader(
                    dir: AppDomain.CurrentDomain.BaseDirectory,
                    fileName: "common.json");
            }

            var settings = new Settings();
            AppSettingsLoader.Load(Factory.GetAppConfigSettingLoader(), ref settings);

            return settings;
        }

        public string StringSetting { get; set; }

        [AppSetting(Key = "StringSettingOverride")]
        public string OverridenSetting { get; set; }

        public int IntSetting { get; set; }

        public uint UintSetting { get; set; }

        public DateTime DateTimeSetting { get; set; }

        public Guid GuidSetting { get; set; }

        public float FloatSetting { get; set; }

        public decimal DecimalSetting { get; set; }

        public EnumValues EnumSetting { get; set; }

        public EnumValues EnumSettingInt { get; set; }

        [AppSetting(SettingType = SettingType.ConnectionString)]
        public IReadOnlyDictionary<string, string> ConnectionStrings { get; set; }
    }

    public enum EnumValues
    {
        Value1 = 1,
        Value2 = 2
    }
}