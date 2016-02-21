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

            var members = typeof(T).FindMembers(
                MemberTypes.Property,
                BindingFlags.Instance | BindingFlags.Public,
                HasAttribute,
                null);

            var exceptions = new List<Exception>();

            foreach (PropertyInfo member in members)
            {
                string settingName = member.Name;

                var attr = member.GetCustomAttribute<AppSettingAttribute>();
                if (attr != null &&
                    attr.Key != null)
                {
                    settingName = attr.Key;
                }

                Trace.TraceInformation($"Loading class member [{member.Name}] as [{settingName}].");

                if (!member.CanWrite)
                {
                    Trace.TraceWarning($"Property [{settingsClass.GetType()}.{member.Name}] is not writeable; skipping.");
                    continue;
                }

                try
                {
                    // Check if the property is meant to load all of the connection strings
                    if (IsSupportedConnectionStringsType(member))
                    {
                        Trace.TraceInformation($"Loading all connection strings into [{member.Name}].");
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
                            continue;
                        }

                        var parsedConnectionString = TypeParser.GetTypedValue(member.PropertyType, loadedConnectionString);
                        member.SetValue(settingsClass, parsedConnectionString);

                        continue;
                    }

                    // Load normal setting types
                    var loadedSetting = settingLoader.GetSetting(settingName);
                    if (!CheckIfSettingIsValid(loadedSetting, settingName))
                    {
                        continue;
                    }

                    var parsedSetting = TypeParser.GetTypedValue(member.PropertyType, loadedSetting);
                    member.SetValue(settingsClass, parsedSetting);
                }
                catch (Exception ex)
                {
                    Trace.TraceError($"Loading of setting [{settingName}] failed with:\r\n{ex}.");
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Any())
            {
                throw new AggregateException(
                    $"{exceptions.Count} errors loading settings.",
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
            var customAttribute = member.GetCustomAttribute<AppSettingAttribute>();

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

            if (customAttribute != null &&
                customAttribute.SettingType == SettingType.Connectionstring)
            {
                return true;
            }

            return false;
        }

        private static bool CheckIfSettingIsValid(string loadedValue, string settingName)
        {
            if (String.IsNullOrEmpty(loadedValue))
            {
                Trace.TraceWarning($"No value found for [{settingName}].");
                return false;
            }

            return true;
        }
    }
}
