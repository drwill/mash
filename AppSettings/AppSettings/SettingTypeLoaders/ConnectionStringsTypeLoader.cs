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
            model.Member.SetValue(model.SettingsClass, model.SettingLoader.GetConnectionStrings());

            return true;
        }
    }
}
