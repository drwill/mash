# Mash.AppSettings.DevSettings

New support in Mash.AppSettings allows for an override setting loader for developer-specific values.
Maybe you want a different connection string to an Azure storage account, instrumentation key for Application Insights, or service endpoint.
When each developer can specify their own options, you can quickly debug locally without having to keep files checked out with your changes, careful not to accidentally commit them.

## Dev setting file

The developer-specific files must be in JSON format, and need only specify app settings you wish to override.

Your code can do a check to see if it is in dev mode (perhaps based on an environment variable).
Ideally, each developer in the team can commit their own file to the project using their unique user name to make the files unique that way the call to construct DevSettingLoader will not need to be different per user.
<pre><code>{
  "Setting1": "DevOverride",
  "ConnectionStrings": {
    "Cxn1": "DevOverrideCxn"
  }
}</code></pre>

## Developer support

Useful information will be traced during loading. Watch your output window for any issues encountered.

## What's New

### November 28, 2016

Initial implementation.
