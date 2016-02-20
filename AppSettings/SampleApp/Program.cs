namespace SampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = Settings.Instance;

            SettingsHelper.PrintPropertyValuesToConsole(settings);
        }
    }
}
