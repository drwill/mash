namespace Mash.AppSettings
{
    /// <summary>
    /// Public factory for AppSettings objects
    /// </summary>
    public static class Factory
    {
        /// <summary>
        /// Gets an object which supports loading a value from the .NET application's configuration file
        /// </summary>
        /// <returns>An object which supports ISettingLoader interface</returns>
        public static ISettingLoader GetAppConfigSettingLoader()
        {
            return new AppConfigSettingLoader();
        }
    }
}
