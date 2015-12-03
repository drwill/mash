using Mash.AppSettings;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SampleApp
{
    internal static class SettingsHelper
    {
        public static void PrintPropertyValuesToConsole(Settings settings)
        {
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
                    Console.WriteLine($"[{member.Name}] is [null]");
                }
                else if (member.PropertyType == typeof(IReadOnlyDictionary<string, string>))
                {
                    Console.WriteLine($"[{member.Name}] is a dictionary of values:");
                    foreach (var item in (IReadOnlyDictionary<string, string>)member.GetValue(settings))
                    {
                        Console.WriteLine($"\t[{item.Key}] = [{item.Value}]");
                    }
                }
                else
                {
                    Console.WriteLine($"Setting [{member.Name}] is [{member.GetValue(settings)}]");
                }
            }
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
