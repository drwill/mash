# Mash.AppSettings

Tired of littering your code with ConfigurationManager.AppSettings["TheSetting"] and parsing the string the type you need?
Let's make loading settings easy with very little code investment.

Just create a data class which holds properties representing the settings you wish to load, and then call Load().
Mash.AppSettings uses reflection on your data class to find public properties with an attribute, finds a setting with that key in your app.config, and then sets the value on that property.

Also, now your unit tests won't have problems if they call code which directly loaded from the app.config.
Instead they can set your settings class with any values they want.
This makes your code a lot more cohesive, and prevents unnecessary coupling to System.Configuration.

Don't want to store settings in your app.config file? No problem!
The loader uses an interface to determine where it gets its settings from so you can replace it later with another implementation.
This will prevent code changes of how you load settings from impacting the rest of your code base.

## Settings class

Your settings class can look as simple as this:

	class MySettings : SingletonSettings<MySettings>
	{
		[AppSetting]
		public int MyIntValue { get; set; }

		[AppSetting(Key = "StringSettingOverride")]
		public string OverridenSetting { get; set; }

		[AppSetting(Optional = true)]
		public string OptionalSetting { get; set; }

		[AppSetting(SettingType = SettingType.ConnectionString)]
		public string SpecificConnectionStringToLoadByKey { get; set; }

		[AppSetting(SettingType = SettingType.ConnectionString)]
		public IReadOnlyDictionary<string, string> ConnectionStrings { get; set; }

		[AppSetting]
		public IList<int> ListOfIntegers { get; set; }
	}

And you can access your code like this:

	var numbers = Settings.Instance.ListOfIntegers;

Or if you want to opt-in all public properties, you can just decorate the class with the AppSetting attribute.

## Load your settings by hand

If you wish to load the settings by hand, the initialization could would look like:

	var settings = new MySettings();
	AppSettingsLoader.Load(AppSettingsFactory.GetAppConfigSettingLoader(), ref settings);

## Local debug dev settings

If more than one developer is working on the code base and requires different settings for each person, it is nice to avoid having to edit the default settings source before debugging.

Each developer can have their settings preset in a file with their unique user name, and you'll rewrite your initialization code to specify this.
Detecting debug flavor is a good way to do this, but you can also use the presence or value of an environment variable, existing of a file, etc.

Just set the public static property AppSettingsLoader.DevSettings to an instance of an ISettingLoader that knows what file to load.

See the [DevSettings](https://github.com/drwill/mash/tree/master/AppSettings/DevSettings) readme for more information.

## Connection strings
There are two options to load connection strings:

1 A single connection string can be loaded into a named property.

2 All connection strings can be loaded into a dictionary.

Use the AppSetting "SettingType" attribute property and set it to "SettingType.ConnectionString".
When loading all connection strings, the property type must be IReadOnlyDictionary&lt;string, string&gt;.
In the dictionary, the key will hold the connection string's name, and the value will be connection string.

## App.Config

Included is support for loading settings from your app.config or web.config file.

## Developer support

Useful information will be traced during loading. Watch your output window for any issues encountered.
Any problems loading values will be returned in an aggregate exception, unless your property is decorated as Optional.

Use the DevSettings property on the AppSettingsLoader class to override whatever settings would normally be loaded with
development settings.
If the DevSettings loader does not have the setting, the usual one will be loaded instead. See Mash.AppSettings.DevSettings for an implementation that reads developer-specific settings from a json file.

## What's New

### September 17, 2017

Add a Json setting loader after the introduction appsettings.json in ASP.NET Core projects. Yes, you can just deserialize the json into a class, or use their built in loading, but you may not want to refactor your code.

Note that although the Json setting loader will handle numbers and integers, you should format lists just as you would for the AppConfigSettingLoader, that is as a delimited string (e.g. "1, 2, 3").

### December 1, 2016

Reduced .NET version requirement to 4.5 to enable legacy clients, as the library does not require 4.61.

### November 23, 2016

Added support for loading development-specific settings by setting the AppSettingsLoader.DevSettings property with a loader
of your choice which will optionally supply developer-specific values.

### September 29, 2016

Loading a list now handles semi-colons as well as commas. It also trims whitespace around entries.

### March 8, 2016

The Optional setting can be applied to the class level and will be inherited by all members, unless otherwise specified on the property.

### March 2, 2016

Fixed a null reference exception in the app config setting loader when attempting to load a connection string which does not exist.

### Feb 28, 2016

You can now load comma-delimited settings into a property type that supports IList&lt;T&gt; where T is a public type.
If you do not initialize the property with an instance, AppSettingsLoader will create a List&lt;T&gt; for you.

Also fixed a bug with the Optional feature released a week ago. The logic was reversed. Sorry about that!

### Feb 21, 2016

In order to prevent bugs due to settings that don't exist at the source, an exception will now be thrown.
If you do not wish to be notified with an exception, mark the setting as Optional (see example above).

### Feb 20, 2016

Added a singleton base class to derive your settings class from.
Because the base class creates an instance of your class you must specify the type, and it must have a parameterless constructor.

Initialization went from newing up your class and calling LoadSettings to one line: var settings = YourSettings.Instance.

If you wish to override the default ISettingsLoader, you may do so with the SettingLoader property, just do so before accessing the Instance property.
