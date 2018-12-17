using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Mash.AppSettings
{
    /// <summary>
    /// Loads application settings into your own data class
    /// </summary>
    public sealed class AppSettingsLoader
    {
        private static SettingTypeLoaderBase _settingTypeLoaders = BuildSettingTypeLoaders();

        /// <summary>
        /// Set this property to override in the Load any settings it can get instead
        /// </summary>
        public static ISettingLoader DevSettings { get; set; }

        /// <summary>
        /// Loads settings for the public properties in the specified class using the specified settings loader
        /// </summary>
        /// <typeparam name="T">The type of settings class being loaded</typeparam>
        /// <param name="settingLoader">The specified setting loader to use</param>
        /// <param name="settingsClass">The settings class to save settings into</param>
        /// <returns>True if successful</returns>
        /// <exception cref="ArgumentNullException">The parameters must be valid</exception>
        /// <exception cref="AggregateException">Any mismatch in setting name or type loading will be reported</exception>
        /// <remarks>Check trace statements for any additional issues encountered during loading</remarks>
        public static bool Load<T>(ISettingLoader settingLoader, ref T settingsClass) where T : class
        {
            if (settingLoader == null)
            {
                throw new ArgumentNullException(nameof(settingLoader));
            }

            if (settingsClass == null)
            {
                throw new ArgumentNullException(nameof(settingsClass));
            }

            MemberInfo[] members = typeof(T).FindMembers(
                MemberTypes.Property,
                BindingFlags.Instance | BindingFlags.Public,
                HasAttribute,
                null);

            var exceptions = new List<Exception>();

            foreach (PropertyInfo member in members)
            {
                AppSettingAttribute attr = member.GetCustomAttribute<AppSettingAttribute>();
                string settingName = attr?.Key ?? member.Name;

                Trace.TraceInformation($"Mash.AppSettings: Loading class member [{member.Name}] as [{settingName}].");

                if (!member.CanWrite)
                {
                    Trace.TraceWarning($"Mash.AppSettings: Property [{settingsClass.GetType()}.{member.Name}] is not writeable; skipping.");
                    continue;
                }

                try
                {
                    var model = new SettingTypeModel
                    {
                        SettingLoader = settingLoader,
                        DevLoader = DevSettings,
                        SettingsClass = settingsClass,
                        Member = member,
                        SettingName = settingName,
                    };

                    _settingTypeLoaders.DoWork(model);
                }
                catch (Exception ex)
                {
                    Trace.TraceError($"Mash.AppSettings: Loading of setting [{settingName}] failed with:\r\n{ex}.");
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Any())
            {
                throw new AggregateException(
                    $"Mash.AppSettings: {exceptions.Count} errors loading settings.",
                    exceptions);
            }

            return true;
        }

        private static SettingTypeLoaderBase BuildSettingTypeLoaders()
        {
            var poco = new PocoSettingTypeLoader();
            var coll = new CollectionTypeLoader { Next = poco };
            var cs = new ConnectionStringTypeLoader { Next = coll };
            var conns = new ConnectionStringsTypeLoader { Next = cs };

            var loaders = new List<SettingTypeLoaderBase>
            {
                conns,
                cs,
                coll,
                poco,
            };

            return loaders.First();
        }

        private static bool HasAttribute(MemberInfo mi, object o)
        {
            if (mi.DeclaringType.GetCustomAttribute<AppSettingAttribute>() != null)
            {
                return true;
            }

            return mi.GetCustomAttribute<AppSettingAttribute>() != null;
        }
    }
}