using System;
using System.ComponentModel;

namespace Mash.AppSettings
{
    /// <summary>
    /// Converts a string value to a specified type
    /// </summary>
    internal static class TypeParser
    {
        /// <summary>
        /// Parses the supplied value into the specified type
        /// </summary>
        /// <param name="theType">The data type of the value to parse</param>
        /// <param name="value">The value to parse</param>
        /// <returns>The parsed type</returns>
        public static dynamic GetTypedValue(Type theType, string value)
        {
            if (theType == null)
            {
                throw new ArgumentNullException(nameof(theType));
            }

            if (theType == typeof(string))
            {
                return value;
            }

            return TypeDescriptor.GetConverter(theType).ConvertFromString(value);
        }
    }
}