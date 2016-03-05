using System;
using System.Diagnostics;

namespace Mash.AppSettings
{
    internal class ConnectionStringTypeLoader : SettingTypeLoaderBase
    {
        internal override bool DoWork(SettingTypeModel model)
        {
            // Check if the property is meant to load a specific connection string
            if (!IsConnectionStringSettingType(model.Member))
            {
                return base.DoWork(model);
            }

            Trace.TraceInformation($"Loading connection string into [{model.Member.Name}].");
            var loadedConnectionString = model.SettingLoader.GetConnectionString(model.SettingName);
            if (!CheckIfSettingIsValid(loadedConnectionString, model.SettingName))
            {
                if (IsSettingRequired(model.Member))
                {
                    throw new ArgumentException("The connection string could not be found.", model.SettingName);
                }

                return true;
            }

            var parsedConnectionString = TypeParser.GetTypedValue(model.Member.PropertyType, loadedConnectionString);
            model.Member.SetValue(model.SettingsClass, parsedConnectionString);

            return true;
        }
    }
}
