using System;

namespace Mash.ArmTools
{
    /// <summary>
    /// String constants in a resource Id
    /// </summary>
    public static class ResourceConstants
    {
        public const string Subscriptions = "subscriptions";
        public const string Providers = "providers";
        public const string ResourceGroups = "resourceGroups";
        public const char ResourceIdSplitter = '/';

        internal static readonly string ResourceIdSplitterString = $"{ResourceIdSplitter}";
        internal static readonly char[] ResourceIdSplitters = new[] { ResourceIdSplitter };

        public static string[] GetParts(this string whole)
        {
            return whole.Split(ResourceIdSplitters, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
