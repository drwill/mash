using System;
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
            var loadedValue = model.SettingLoader.GetSetting(model.SettingName);
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
            bool? isOptionalOnMember = member.GetCustomAttribute<AppSettingAttribute>()?.Optional;
            bool? isOptionalOnClass = member.DeclaringType.GetCustomAttribute<AppSettingAttribute>()?.Optional;

            return !(isOptionalOnMember.HasValue && isOptionalOnMember.Value == true) &&
                !(isOptionalOnClass.HasValue && isOptionalOnClass.Value == true);
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
    }
}
