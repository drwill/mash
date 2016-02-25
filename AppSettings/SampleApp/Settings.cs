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

        [AppSetting(Key = "StringSettingOverride")]
        public string OverridenSetting { get; set; }

        public int IntSetting { get; set; }

        public uint UintSetting { get; set; }

        public DateTime DateTimeSetting { get; set; }

        public Guid GuidSetting { get; set; }

        public float FloatSetting { get; set; }

        public decimal DecimalSetting { get; set; }

        [AppSetting(SettingType = SettingType.Connectionstring)]
        public string MyConnectionString1 { get; set; }

        public EnumValues EnumSetting { get; set; }

        public EnumValues EnumSettingInt { get; set; }

        [AppSetting(SettingType = SettingType.Connectionstring)]
        public IReadOnlyDictionary<string, string> ConnectionStrings { get; set; }

        public IList<int> ArraylistSettingInt { get; set; }

        public IList<EnumValues> ArraylistSettingEnum { get; set; }
    }

    internal enum EnumValues
    {
        Value1 = 1,
        Value2 = 2
    }
}
