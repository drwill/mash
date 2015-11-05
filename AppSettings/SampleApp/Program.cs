using AppSettings;

namespace SampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = new Settings();

            AppSettingsLoader.Load(
                Factory.GetAppConfigSettingLoader(),
                ref settings);

            SettingsHelper.PrintPropertyValuesToConsole(settings);
        }
    }
}
