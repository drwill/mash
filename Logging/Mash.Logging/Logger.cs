using System.Collections.Generic;

namespace Mash.Logging
{
    /// <summary>
    /// Logger interface
    /// </summary>
    public class Logger
    {
        private readonly CentralLogger _centralLogger;

        /// <summary>
        /// Contextual properties to include with each log from this logger.
        /// </summary>
        public IDictionary<string, string> LoggerContext { get; } = new Dictionary<string, string>();

        internal Logger(CentralLogger commonLogger, IDictionary<string, string> loggerContext)
        {
            _centralLogger = commonLogger;
            if (loggerContext != null)
            {
                foreach (var kvp in loggerContext)
                {
                    LoggerContext.Add(kvp.Key, kvp.Value);
                }
            }
        }

        /// <summary>
        /// Clones the current logger and all properties.
        /// </summary>
        public virtual Logger Clone()
        {
            return new Logger(_centralLogger, LoggerContext);
        }

        /// <summary>
        /// Log a trace message.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="severity">The severity level</param>
        /// <param name="extraProperties">Additional properties to include</param>
        public virtual void Trace(string message, TraceSeverity severity = TraceSeverity.Verbose, IDictionary<string, string> extraProperties = null)
        {
            _centralLogger.Trace(message, severity, _centralLogger.MergeProperties(LoggerContext, extraProperties));
        }

        /// <summary>
        /// Logs a metric.
        /// </summary>
        /// <param name="name">The name of the metric</param>
        /// <param name="value">The metric value</param>
        /// <param name="extraProperties">Additional properties to include</param>
        public virtual void Metric(string name, double value, IDictionary<string, string> extraProperties = null)
        {
            _centralLogger.Metric(name, value, _centralLogger.MergeProperties(LoggerContext, extraProperties));
        }

        /// <summary>
        /// Logs an event.
        /// </summary>
        /// <param name="name">The name of the event</param>
        /// <param name="extraProperties">Additional properties to include</param>
        public virtual void Event(string name, IDictionary<string, string> extraProperties = null)
        {
            _centralLogger.Event(name, _centralLogger.MergeProperties(LoggerContext, extraProperties));
        }

        /// <summary>
        /// Flushes any logs. Use before closing the process to ensure all information has been logged.
        /// </summary>
        public virtual void Flush()
        {
            _centralLogger.Flush();
        }
    }
}
