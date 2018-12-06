using System;

namespace Mash.AppSettings
{
    /// <summary>
    /// A code attribute for decorating a settings class' properties
    /// </summary>
    /// <remarks>
    /// As a convenience to avoid decorating each property, you may choose to attribute the class which will opt in all class public properties.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public sealed class AppSettingAttribute : Attribute
    {
        /// <summary>
        /// The key of the feature control key to load
        /// </summary>
        /// <remarks>
        /// Use this if you wish to use a name other than the attributed property for loading the target feature control key
        /// </remarks>
        public string Key { get; set; }

        /// <summary>
        /// The type of setting to load
        /// </summary>
        public SettingType SettingType { get; set; }

        /// <summary>
        /// If the setting is optional, an exception will not be thrown if it does not exist
        /// </summary>
        public bool Optional { get; set; }
    }
}
