using System.Collections.Generic;
using System.Diagnostics;

namespace Mash.AppSettings
{
    internal class ConnectionStringsTypeLoader : SettingTypeLoaderBase
    {
        internal override bool DoWork(SettingTypeModel model)
        {
            if (!IsSupportedConnectionStringsType(model.Member))
            {
                return base.DoWork(model);
            }

            Trace.TraceInformation($"Mash.AppSettings: Loading all connection strings into [{model.Member.Name}].");

            var connectionStrings = new Dictionary<string, string>(model.SettingLoader.GetConnectionStrings());
            if (model.DevLoader != null)
            {
                foreach (var kvp in model.DevLoader.GetConnectionStrings())
                {
                    Trace.TraceInformation($"Mash.AppSettings: overriding {kvp.Key} connection string from dev settings");
                    connectionStrings[kvp.Key] = kvp.Value;
                }
            }

            model.Member.SetValue(model.SettingsClass, connectionStrings);

            return true;
        }
    }
}
