using Mash.Logging;
using System;
using System.Diagnostics;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = Stopwatch.StartNew();

            var loggingBuilder = new LoggingBuilder();
            loggingBuilder.AppContext.Add("Host", Environment.MachineName);
            loggingBuilder.LogProviders.Add(new ConsoleLogProvider { ShouldLogContext = true });

            var logger = loggingBuilder.BuildLogger();
            logger.LoggerContext.Add("Operation", "Sample");

            logger.Event("SampleStarted");

            logger.Trace("Sample message 1", TraceSeverity.Verbose);
            logger.Trace("Sample message 2", TraceSeverity.Warning);
            logger.Trace("Sample message 3", TraceSeverity.Error);

            sw.Stop();
            logger.Metric("AppDuration", sw.ElapsedMilliseconds);

            logger.Flush();
        }
    }
}
