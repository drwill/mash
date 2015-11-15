using Mash.AppSettings;
using SampleWebApp.Models;
using System.Collections.Generic;
using System.Reflection;

namespace SampleWebApp
{
    internal class SettingsHelper
    {
        public static Dictionary<string, string> GetPropertyValues(Settings settings)
        {
            var dictionary = new Dictionary<string, string>();

            var members = typeof(Settings).FindMembers(
                MemberTypes.Property,
                BindingFlags.Instance | BindingFlags.Public,
                HasAttribute,
                null);

            foreach (PropertyInfo member in members)
            {
                if (member == null ||
                    member.GetValue(settings) == null)
                {
                    dictionary.Add(member.Name, null);
                }
                else if (member.PropertyType == typeof(IReadOnlyDictionary<string, string>))
                {
                    dictionary.Add(member.Name, "Connection strings");
                    foreach (var item in (IReadOnlyDictionary<string, string>)member.GetValue(settings))
                    {
                        dictionary.Add(item.Key, item.Value);
                    }
                }
                else
                {
                    dictionary.Add(member.Name, member.GetValue(settings).ToString());
                }
            }

            return dictionary;
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