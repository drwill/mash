using Mash.AppSettings;
using System;

namespace SampleWebApp.Models
{
    /// <summary>
    /// Settings required for the running of this application
    /// </summary>
    internal class Settings
    {
        [AppSetting]
        public string StringSetting { get; set; }

        [AppSetting(Key = "StringSettingOverride")]
        public string OverridenSetting { get; set; }

        [AppSetting]
        public int IntSetting { get; set; }

        [AppSetting]
        public uint UintSetting { get; set; }

        [AppSetting]
        public DateTime DateTimeSetting { get; set; }

        [AppSetting]
        public Guid GuidSetting { get; set; }

        [AppSetting]
        public float FloatSetting { get; set; }

        [AppSetting]
        public decimal DecimalSetting { get; set; }

        [AppSetting]
        public EnumValues EnumSetting { get; set; }

        [AppSetting]
        public EnumValues EnumSettingInt { get; set; }
    }

    internal enum EnumValues
    {
        Value1 = 1,
        Value2 = 2
    }
}