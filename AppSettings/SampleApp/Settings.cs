using Mash.AppSettings;
using System;
using System.Collections.Generic;

namespace SampleApp
{
    /// <summary>
    /// Settings required for the running of this application
    /// </summary>
    [AppSetting]
    internal class Settings : SingletonSettings<Settings>
    {
        public string StringSetting { get; set; }

        public int IntSetting { get; set; }

        public uint UintSetting { get; set; }

        public DateTime DateTimeSetting { get; set; }

        public Guid GuidSetting { get; set; }

        public float FloatSetting { get; set; }

        public decimal DecimalSetting { get; set; }

        public EnumValues EnumSetting { get; set; }

        public EnumValues EnumSettingInt { get; set; }

        [AppSetting(Key = "StringSettingOverride")]
        public string OverridenSetting { get; set; }

        public IList<string> CollectionSetting { get; set; }

        [AppSetting(SettingType = SettingType.ConnectionString)]
        public IReadOnlyDictionary<string, string> ConnectionStrings { get; set; }

        [AppSetting(SettingType = SettingType.ConnectionString)]
        public string MyConnectionString1 { get; set; }
    }

    public enum EnumValues
    {
        Value1 = 1,
        Value2 = 2,
    }
}
