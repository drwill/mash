# Mash.AppSettings

Tired of littering your code with ConfigurationManager.AppSettings["TheSetting"] and parsing the string the type you want?
Let's make loading settings easy with very little code investment.

Just create a data class which holds properties representing the settings you wish to load, and then call Load().
Mash.AppSettings use reflection on your data class to find public properties with our attribute and find a setting with that key in your app.config, and then set the value.

Now your unit tests won't have problems if they call code which directly loaded from the app.config.
Instead they can set your settings class with any values they want.
This makes your code a lot more cohesive, and prevent unnecessary coupling to System.Configuration.

Don't want to store settings in your app.config file? No problem!
The loader uses an interface to determine where it gets its settings from so you can replace it later with another implementation.
This will prevent code changes of how you load settings from impacting the rest of your code base.

Your code will look like this:

<pre><code>var settings = new Settings.Instance;</code></pre>

Your settings class will look something like this:
<pre><code>[AppSetting]
class MySettings : SingletonSettings&lt;MySettings&gt;
{
    public int MyIntValue { get; set; }

    [AppSetting(Key = "StringSettingOverride")]
    public string OverridenSetting { get; set; }

    [AppSetting(SettingType = SettingType.ConnectionString)]
    public string SpecificConnectionStringToLoadByKey { get; set; }

    [AppSetting(SettingType = SettingType.ConnectionString)]
    public IReadOnlyDictionary&lt;string, string&gt; ConnectionStrings { get; set; }
}</code></pre>

Or if you want to opt-in all public properties, you can just decorate the class with the AppSetting attribute.

There are two options to load connection strings:
1. A single connection string can be loaded into a named property.
2. All connection strings can be loaded into a dictionary.

Use the AppSetting "SettingType" attribute property and set it to "SettingType.ConnectionString".
When loading all connection strings, the property type must be IReadOnlyDictionary&lt;string, string&gt;.
In the dictionary, the key will hold the connection string's name, and the value will be connection string.

## App.Config
Included is support for loading settings from your app.config or web.config file.

## Developer support
Useful information will be traced during loading. Watch your output window for any issues encountered.
Any problems loading values will be returned in an aggregate exception.

## What's New?

###Feb 20, 2016
Added a singleton base class to derive your settings class from.
Because the base class creates an instance of your class you must specify the type, and it must have a parameterless constructor.

Initialization went from newing up your class and calling LoadSettings to one line: var settings = YourSettings.Instance.

If you wish to override the default ISettingsLoader, you may do so with the SettingLoader property, just do so before accessing the Instance property.