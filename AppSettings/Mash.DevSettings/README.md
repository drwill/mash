# Mash.AppSettings.DevSettings

New support in Mash.AppSettings allows for an override setting loader for developer-specific values.
Maybe you want a different connection string to an Azure storage account, instrumentation key for Application Insights, or service endpoint.
When each developer can specify their own options, you can quickly debug locally without having to keep files checked out with your changes, careful not to accidentally commit them.

## Dev setting file

The developer-specific files must be in JSON format, and need only specify app settings you wish to override.

Your code can do a check to see if it is in dev mode (perhaps based on an environment variable).
Ideally, each developer in the team can commit their own file to the project using their unique user name to make the files unique that way the call to construct DevSettingLoader will not need to be different per user.

````json
{
  "Setting1": "DevOverride",
  "ConnectionStrings": {
    "Cxn1": "DevOverrideCxn"
  }
}
````

## Override settings loading

To override settings loading, simply assign a DevSettingsLoader to AppSettingsLoader. I like to choose to do mine in Debug builds only.

````C#
  private static void Main(string[] args)
  {
    LoadDevSettings();

    // load/access settings
    Console.WriteLine(Settings.Instance.Setting1);
  }


  [System.Diagnostics.Conditional("DEBUG")]
  private static void LoadDevSettings()
  {
      // find the project directory
      var currentDir = Directory.GetCurrentDirectory();
      int binDirIndex = currentDir.LastIndexOf("\\bin");
      var devSettingsDir = currentDir.Remove(binDirIndex);

      // put all developer override files in a subfolder named DevSettings
      devSettingsDir = Path.Combine(devSettingsDir, "DevSettings");

      // fileName null to use the logged in user
      var devSettingLoader = new DevSettingLoader(devSettingsDir, fileName: null);

      // assigning the loader to this property makes the magic work
      AppSettingsLoader.DevSettings = devSettingLoader;
  }
````

## Developer support

Useful information will be traced during loading. Watch your output window for any issues encountered.

## What's New

### December 6, 2018

Converted project to target .NET Core 2.1.

### November 28, 2016

Initial implementation.
