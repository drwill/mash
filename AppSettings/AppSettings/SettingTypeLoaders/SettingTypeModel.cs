using System.Reflection;

namespace Mash.AppSettings
{
    internal class SettingTypeModel
    {
        internal ISettingLoader SettingLoader { get; set; }
        internal ISettingLoader DevLoader { get; set; }
        internal dynamic SettingsClass { get; set; }
        internal PropertyInfo Member { get; set; }
        internal string SettingName { get; set; }
    }
}
