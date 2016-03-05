using System.Reflection;

namespace Mash.AppSettings
{
    internal class SettingTypeModel
    {
        public dynamic SettingsClass { get; set; }
        public PropertyInfo Member { get; set; }
        public string SettingName { get; set; }
        public string LoadedValue { get; set; }
    }
}
