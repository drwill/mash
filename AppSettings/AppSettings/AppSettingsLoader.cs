using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Mash.AppSettings
{
    /// <summary>
    /// Loads application settings into your own custom data class
    /// </summary>
    public sealed class AppSettingsLoader
    {
        /// <summary>
        /// Loads settings for the public properties in the specified class using the specified settings loader
        /// </summary>
        /// <typeparam name="T">The type of settings class being loaded</typeparam>
        /// <param name="settingLoader">The specified setting loader to use</param>
        /// <param name="settingsClass">The settings class to save settings into</param>
        /// <returns>True if successful</returns>
        /// <exception cref="ArgumentNullException">The parameters must be valid</exception>
        /// <exception cref=AggregateException">Any mismatch in setting name or type loading will be reported</exception>
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
                var attr = member.GetCustomAttribute<AppSettingAttribute>();
                string settingName = attr?.Key ?? member.Name;

                Trace.TraceInformation($"Mash.AppSettings: Loading class member [{member.Name}] as [{settingName}].");

                if (!member.CanWrite)
                {
                    Trace.TraceWarning($"Mash.AppSettings: Property [{settingsClass.GetType()}.{member.Name}] is not writeable; skipping.");
                    continue;
                }

                try
                {
                    // Check if the property is meant to load all of the connection strings
                    if (IsSupportedConnectionStringsType(member))
                    {
                        Trace.TraceInformation($"Mash.AppSettings: Loading all connection strings into [{member.Name}].");
                        member.SetValue(settingsClass, settingLoader.GetConnectionStrings());
                        continue;
                    }

                    // Check if the property is meant to load a specific connection string
                    if (IsConnectionStringSettingType(member))
                    {
                        Trace.TraceInformation($"Loading connection string into [{member.Name}].");
                        var loadedConnectionString = settingLoader.GetConnectionString(settingName);
                        if (!CheckIfSettingIsValid(loadedConnectionString, settingName))
                        {
                            if (IsSettingRequired(member))
                            {
                                exceptions.Add(new ArgumentException("The connection string could not be found.", settingName));
                            }

                            continue;
                        }

                        var parsedConnectionString = TypeParser.GetTypedValue(member.PropertyType, loadedConnectionString);
                        member.SetValue(settingsClass, parsedConnectionString);

                        continue;
                    }

                    var model = new SettingTypeModel
                    {
                        SettingLoader = settingLoader,
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

        private static bool HasAttribute(MemberInfo mi, object o)
        {
            if (mi.DeclaringType.GetCustomAttribute<AppSettingAttribute>() != null)
            {
                return true;
            }

            return mi.GetCustomAttribute<AppSettingAttribute>() != null;
        }

        private static bool IsSupportedConnectionStringsType(PropertyInfo member)
        {
            if (IsConnectionStringSettingType(member) &&
                member.PropertyType == typeof(IReadOnlyDictionary<string, string>))
            {
                return true;
            }

            return false;
        }

        private static bool IsConnectionStringSettingType(PropertyInfo member)
        {
            var customAttribute = member.GetCustomAttribute<AppSettingAttribute>();

            return customAttribute?.SettingType == SettingType.Connectionstring;
        }

        private static bool IsSettingRequired(PropertyInfo member)
        {
            bool? isOptionalOnMember = member.GetCustomAttribute<AppSettingAttribute>()?.Optional;
            bool? isOptionalOnClass = member.DeclaringType.GetCustomAttribute<AppSettingAttribute>()?.Optional;

            return !(isOptionalOnMember.HasValue && isOptionalOnMember.Value == true) &&
                !(isOptionalOnClass.HasValue && isOptionalOnClass.Value == true);
        }

        private static bool CheckIfSettingIsValid(string loadedValue, string settingName)
        {
            if (String.IsNullOrEmpty(loadedValue))
            {
                Trace.TraceWarning($"Mash.AppSettings: No value found for [{settingName}].");
                return false;
            }

            return true;
        }

        private static SettingTypeLoaderBase AddSettingTypeLoaders()
        {

            var poco = new PocoSettingTypeLoader();
            var coll = new CollectionTypeLoader { Next = poco };

            var loaders = new List<SettingTypeLoaderBase>
            {
                coll,
                poco,
            };

            return loaders.First();
        }

        private static SettingTypeLoaderBase _settingTypeLoaders = AddSettingTypeLoaders();
    }
}
