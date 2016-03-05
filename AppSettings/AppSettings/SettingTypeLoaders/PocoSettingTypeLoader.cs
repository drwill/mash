namespace Mash.AppSettings
{
    internal class PocoSettingTypeLoader : SettingTypeLoaderBase
    {
        /// <summary>
        /// Handles plain, old CLR objects
        /// </summary>
        internal override bool DoWork(SettingTypeModel model)
        {
            var loadedValue = LoadValue(model);
            if (loadedValue == null)
            {
                return false;
            }

            dynamic typedValue = TypeParser.GetTypedValue(model.Member.PropertyType, loadedValue);
            model.Member.SetValue(model.SettingsClass, typedValue);

            return true;
        }
    }
}
