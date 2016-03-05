namespace Mash.AppSettings
{
    internal class PocoSettingTypeLoader : SettingTypeLoaderBase
    {
        /// <summary>
        /// Handles plain, old CLR objects
        /// </summary>
        internal override bool DoWork(SettingTypeModel model)
        {
            // POCO setting type loader has no pre-conditions
            dynamic typedValue = TypeParser.GetTypedValue(model.Member.PropertyType, model.LoadedValue);
            model.Member.SetValue(model.SettingsClass, typedValue);
            return true;
        }
    }
}
