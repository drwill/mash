using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Mash.AppSettings
{
    /// <summary>
    /// A base class for setting type loaders
    /// </summary>
    internal abstract class SettingTypeLoaderBase
    {
        /// <summary>
        /// The next setting type loader in the chain of responsibility
        /// </summary>
        public SettingTypeLoaderBase Next { get; set; }

        /// <summary>
        /// Indicates whether or not this setting type matches the specified member
        /// </summary>
        /// <param name="member">The property to evaluate</param>
        /// <returns>True if a match, otehrwise false</returns>
        internal virtual bool DoWork(SettingTypeModel model)
        {
            if (Next != null)
            {
                return Next.DoWork(model);
            }

            return false;
        }

        protected string LoadValue(SettingTypeModel model)

        {
            string loadedValue = model.DevLoader?.GetSetting(model.SettingName);

            if (loadedValue == null)
            {
                loadedValue = model.SettingLoader.GetSetting(model.SettingName);
            }

            if (!CheckIfSettingIsValid(loadedValue, model.SettingName))
            {
                if (IsSettingRequired(model.Member))
                {
                    throw new ArgumentException("The setting could not be found.", model.SettingName);
                }

                Trace.TraceInformation($"Skipping optional setting [{model.SettingName}] which had no value.");
            }

            return loadedValue;
        }

        protected static bool IsSettingRequired(PropertyInfo member)
        {
            // If specified on the property, then that value wins out
            bool? isOptionalOnMember = member.GetCustomAttribute<AppSettingAttribute>()?.Optional;
            if (isOptionalOnMember.HasValue)
            {
                return isOptionalOnMember.Value == false;
            }

            // Otherwise if specified on the class then that value wins out
            bool? isOptionalOnClass = member.DeclaringType.GetCustomAttribute<AppSettingAttribute>()?.Optional;
            if (isOptionalOnClass.HasValue)
            {
                return isOptionalOnClass.Value == false;
            }

            // If not specified, then we assume it is required
            return true;
        }

        protected static bool CheckIfSettingIsValid(string loadedValue, string settingName)
        {
            if (String.IsNullOrEmpty(loadedValue))
            {
                Trace.TraceWarning($"Mash.AppSettings: No value found for [{settingName}].");
                return false;
            }

            return true;
        }

        protected static bool IsSupportedConnectionStringsType(PropertyInfo member)
        {
            if (IsConnectionStringSettingType(member) &&
                member.PropertyType == typeof(IReadOnlyDictionary<string, string>))
            {
                return true;
            }

            return false;
        }

        protected static bool IsConnectionStringSettingType(PropertyInfo member)
        {
            var customAttribute = member.GetCustomAttribute<AppSettingAttribute>();

            return customAttribute?.SettingType == SettingType.ConnectionString;
        }
    }
}
