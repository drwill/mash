using System;

namespace Mash.AppSettings
{
    /// <summary>
    /// A base class for your settings data class to reduce initialization code required
    /// </summary>
    /// <remarks>
    /// Specify your dervied class for T, so the base class knows what type to instantiate
    /// </remarks>
    public abstract class SingletonSettings<T> where T : class, new()
    {
        /// <summary>
        /// The singleton instance, constructed and loaded on first evaluation.
        /// </summary>
        public static T Instance => _instance.Value;

        /// <summary>
        /// The setting loader to use, by default the AppConfigSettingLoader.
        /// </summary>
        public static ISettingLoader SettingLoader { get; set; } = AppSettingsFactory.GetAppConfigSettingLoader();

        private static T LoadInstance()
        {
            T settings = new T();

            AppSettingsLoader.Load(SettingLoader, ref settings);

            return settings;
        }

        private static Lazy<T> _instance = new Lazy<T>(LoadInstance);
    }
}
