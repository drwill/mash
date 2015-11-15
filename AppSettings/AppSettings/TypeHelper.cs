using System;
using System.Reflection;

namespace Mash.AppSettings
{
    internal class TypeHelper
    {
        /// <summary>
        /// Gets the type of the class member
        /// </summary>
        /// <param name="member">The MemberInfo object</param>
        /// <returns>The type of the class member</returns>
        /// <remarks>
        /// Supplied by StackOverflow user nawfal:
        /// http://stackoverflow.com/questions/15921608/getting-the-type-of-a-memberinfo-with-reflection
        /// </remarks>
        public static Type GetUnderlyingType(MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;

                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;

                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;

                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;

                default:
                    throw new ArgumentException("Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo");
            }
        }
    }
}