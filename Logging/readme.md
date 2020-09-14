# Mash.Logging

Most of my code starts running locally in a console window, so I want logs printed to the screen.
Soon that code begins running headless, and I rely on logs to know what happened after the fact.
I don't want to change my logging up, I just want it to go to a different location.
Sometimes, I want it to go to multiple locations.

This utility allows you to configure logging providers, and build loggers from that to hand off to business logic.
It also lets you specify a common property bag of data to be automatically included with each log statement.
Additionally, each logger instance can build its own context.

## Initialization

To initialize logging:

```csharp
var loggingBuilder = new LoggingBuilder();

// Add any app-level contextual properties
loggingBuilder.AppContext.Add("RunId", Guid.NewGuid().ToString());

// Add desired providers
loggingBuilder.LogProviders.Add(new ConsoleLogProvider());

var logger = loggingBuilder.BuildLogger(
    new Dictionary<string, string>
    {
        { "Operation", "ProcessOrders" },
    });

// Now pass off your logger to code to do work
ProcessOrders(logger);
```

## Logging

Logging is simple.
Code can log traces, metrics, or events.
See this [sample](./Sample/Program.cs) for examples.

If an operation branches off into more than one code path, you can clone the current logger which copies the current logger context into the new logger.
Modifications to that logger's context will not affect the current one.

## Logging providers

This utility includes a console logger, and you can build your own adapters to other destinations.

## Changelog

### 2020/09/12

#### Mash.Logging 1.0.2-preview

Fixed bug where `ConsoleLogProvider.ShouldLogContext` was ignored.

#### Mash.Logging.ApplicationInsights 1.0.1-preview

Added a persistence channel to enable sending of telemetry after coming back online.