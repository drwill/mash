using System;

namespace Mash.AppSettings
{
    /// <summary>
    /// A base class for your settings data class to reduce initialization code required
    /// </summary>
    public abstract class SingletonSettings<T> where T : class, new()
    {
        /// <summary>
        /// The singleton instance
        /// </summary>
        public static T Instance => _instance.Value;

        /// <summary>
        /// The setting loader to use
        /// </summary>
        public static ISettingLoader SettingLoader { get; set; } = Factory.GetAppConfigSettingLoader();

        private static T LoadInstance()
        {
            T settings = new T();

            AppSettingsLoader.Load(SettingLoader, ref settings);

            return settings;
        }

        private static Lazy<T> _instance = new Lazy<T>(LoadInstance);
    }
}
