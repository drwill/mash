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

                    // Load normal setting types
                    var loadedSetting = settingLoader.GetSetting(settingName);
                    if (!CheckIfSettingIsValid(loadedSetting, settingName))
                    {
                        if (IsSettingRequired(member))
                        {
                            exceptions.Add(new ArgumentException("The setting could not be found.", settingName));
                        }

                        continue;
                    }

                    if (IsCollection(member))
                    {
                        var args = member.PropertyType.GetGenericArguments();
                        if (args.Length > 1)
                        {
                            Trace.TraceWarning($"Unsupported property type with {args.Length} generic parameters.");
                        }
                        Type itemType = args.First();

                        // Check to see if an instance already exists
                        dynamic property = member.GetGetMethod().Invoke(settingsClass, null);
                        if (property == null)
                        {
                            // No instance exists so create a List<T>
                            Type listType = typeof(List<>)
                                .GetGenericTypeDefinition()
                                .MakeGenericType(itemType);

                            dynamic instance = Activator.CreateInstance(listType);

                            // Assign the instance to the class property
                            member.SetValue(settingsClass, instance);
                            property = instance;
                        }

                        foreach (var item in loadedSetting.Split(','))
                        {
                            // There's a dynamic binding issue with non-public types. One fix is to cast to IList to ensure the call to Add succeeds
                            // but that requires basing this feature off of IList<T> and not ICollection<T>.
                            // This does not work for ICollection because it does not include the Add method (only ICollection<T> does)
                            // http://stackoverflow.com/questions/15920844/system-collections-generic-ilistobject-does-not-contain-a-definition-for-ad
                            property.Add(TypeParser.GetTypedValue(itemType, item));
                        }
                    }
                    else
                    {
                        var parsedSetting = TypeParser.GetTypedValue(member.PropertyType, loadedSetting);
                        member.SetValue(settingsClass, parsedSetting);
                    }
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

        private static bool IsCollection(PropertyInfo member)
        {
            var interfaces = member.PropertyType.FindInterfaces(
                (Type t, object o) => t.ToString().StartsWith(o.ToString()),
                "System.Collections.Generic.ICollection`1");

            return member.PropertyType.Name == "ICollection`1" || interfaces.Any();
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
                Trace.TraceWarning($"No value found for [{settingName}].");
                return false;
            }

            return true;
        }
    }
}
