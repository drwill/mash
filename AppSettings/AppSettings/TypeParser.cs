using System;
using System.ComponentModel;

namespace AppSettings
{
    /// <summary>
    /// Converts a string value to a specified type
    /// </summary>
    internal static class TypeParser
    {
        /// <summary>
        /// Parses the supplied value into the specified type
        /// </summary>
        /// <param name="value">The value to parse</param>
        /// <returns>The parsed type</returns>
        public static dynamic GetTypedValue(Type theType, string value)
        {
            if (theType == typeof(string))
            {
                return value;
            }

            return TypeDescriptor.GetConverter(theType).ConvertFromString(value);
        }
    }
}