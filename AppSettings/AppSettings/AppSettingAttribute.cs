using System;

namespace AppSettings
{
    /// <summary>
    /// A code attribute for decorating a settings class' properties
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class AppSettingAttribute : Attribute
    {
        /// <summary>
        /// The key of the feature control key to load
        /// </summary>
        /// <remarks>
        /// Use this if you wish to use a name other than the attributed property for loading the target feature control key
        /// </remarks>
        public string Key { get; set; }
    }
}