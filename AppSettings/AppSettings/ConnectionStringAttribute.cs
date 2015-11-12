using System;

namespace Mash.AppSettings
{
    // <summary>
    /// A code attribute for decorating connection string property
    /// </summary>
    /// <remarks>
    /// If a property is decorated with this attribute along with the AppSetting attribute, it will be 
    /// treated as a collection of connection strings
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public sealed class ConnectionStringAttribute : Attribute
    {
    }
}
