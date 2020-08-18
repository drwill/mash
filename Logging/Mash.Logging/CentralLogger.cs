using System.Collections.Generic;
using System.Linq;

namespace Mash.Logging
{
    /// <summary>
    /// A class that all logging goes through, that hands out logs to each log provider.
    /// </summary>
    internal class CentralLogger
    {
        private readonly IList<ILoggingProvider> _logProviders;
        private readonly IDictionary<string, string> _appContext;

        internal CentralLogger(IList<ILoggingProvider> logProviders, IDictionary<string, string> appContext)
        {
            _logProviders = logProviders;
            _appContext = appContext;
        }

        /// <summary>
        /// Log a trace message.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="severity">The tracing severity level</param>
        /// <param name="extraProperties">Additional properties to include</param>
        internal virtual void Trace(string message, TraceSeverity severity = TraceSeverity.Verbose, IDictionary<string, string> extraProperties = null)
        {
            foreach (var logProvider in _logProviders)
            {
                logProvider.Trace(message, severity, MergeProperties(extraProperties));
            }
        }

        /// <summary>
        /// Logs a metric.
        /// </summary>
        /// <param name="name">The name of the metric</param>
        /// <param name="value">The metric value</param>
        /// <param name="extraProperties">Additional properties to include</param>
        internal virtual void Metric(string name, double value, IDictionary<string, string> extraProperties = null)
        {
            foreach (var logProvider in _logProviders)
            {
                logProvider.Metric(name, value, MergeProperties(extraProperties));
            }
        }

        /// <summary>
        /// Logs an event.
        /// </summary>
        /// <param name="name">The name of the event</param>
        /// <param name="extraProperties">Additional properties to include</param>
        internal virtual void Event(string name, IDictionary<string, string> extraProperties = null)
        {
            foreach (var logProvider in _logProviders)
            {
                logProvider.Event(name, MergeProperties(extraProperties));
            }
        }

        /// <summary>
        /// Flushes any logs. Use before closing the application to ensure all information has been logged.
        /// </summary>
        internal virtual void Flush()
        {
            foreach (var logProvider in _logProviders)
            {
                logProvider.Flush();
            }
        }

        internal IDictionary<string, string> MergeProperties(IDictionary<string, string> bag1, IDictionary<string, string> bag2)
        {
            if (!bag1.Any())
            {
                return bag2;
            }

            if (bag2 == null
                || !bag2.Any())
            {
                return bag1;
            }

            var tempDictionary = new Dictionary<string, string>(bag1);
            foreach (var kvp in bag2)
            {
                tempDictionary[kvp.Key] = kvp.Value;
            }

            return tempDictionary;
        }

        private IDictionary<string, string> MergeProperties(IDictionary<string, string> extraProperties)
        {
            // Small optimization
            if (extraProperties == null
                || !extraProperties.Any())
            {
                return _appContext;
            }

            return MergeProperties(extraProperties, _appContext);
        }
    }
}
