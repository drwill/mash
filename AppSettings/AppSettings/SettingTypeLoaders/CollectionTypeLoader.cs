using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Mash.AppSettings
{
    internal class CollectionTypeLoader : SettingTypeLoaderBase
    {
        internal override bool DoWork(SettingTypeModel model)
        {
            if (!IsCollection(model.Member))
            {
                return base.DoWork(model);
            }

            var args = model.Member.PropertyType.GetGenericArguments();
            if (args.Length > 1)
            {
                Trace.TraceWarning($"Mash.AppSettings: Unsupported property type with {args.Length} generic parameters.");
            }
            Type itemType = args.First();

            // Check to see if an instance already exists
            dynamic property = model.Member.GetGetMethod().Invoke(model.SettingsClass, null);
            if (property == null)
            {
                // No instance exists so create a List<T>
                Type listType = typeof(List<>)
                    .GetGenericTypeDefinition()
                    .MakeGenericType(itemType);

                dynamic instance = Activator.CreateInstance(listType);

                // Assign the instance to the class property
                model.Member.SetValue(model.SettingsClass, instance);
                property = instance;
            }

            var loadedValue = LoadValue(model);
            if (loadedValue == null)
            {
                return false;
            }

            foreach (var item in loadedValue.Split(','))
            {
                // There's a dynamic binding issue with non-public types. One fix is to cast to IList to ensure the call to Add succeeds
                // but that requires basing this feature off of IList<T> and not ICollection<T>.
                // This does not work for ICollection because it does not include the Add method (only ICollection<T> does)
                // http://stackoverflow.com/questions/15920844/system-collections-generic-ilistobject-does-not-contain-a-definition-for-ad
                property.Add(TypeParser.GetTypedValue(itemType, item));
            }

            return true;
        }

        private static bool IsCollection(PropertyInfo member)
        {
            var interfaces = member.PropertyType.FindInterfaces(
                (Type t, object o) => t.ToString().StartsWith(o.ToString()),
                "System.Collections.Generic.ICollection`1");

            return member.PropertyType.Name == "ICollection`1" || interfaces.Any();
        }
    }
}
