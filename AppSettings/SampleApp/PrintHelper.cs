using Mash.AppSettings;
using System;
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
                    continue;
                }

                Console.WriteLine($"Setting [{member.Name}] is [{member.GetValue(settings)}]");
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
