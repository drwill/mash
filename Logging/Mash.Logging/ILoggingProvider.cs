using System.Collections.Generic;

namespace Mash.Logging
{
    /// <summary>
    /// A destination for logs (e.g. console, text file, SQL).
    /// </summary>
    public interface ILoggingProvider
    {
        /// <summary>
        /// Log a trace message.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="severity">The tracing severity level</param>
        /// <param name="context">Contextual properties to include</param>
        void Trace(string message, TraceSeverity severity, IDictionary<string, string> context);

        /// <summary>
        /// Logs a metric.
        /// </summary>
        /// <param name="name">The name of the metric</param>
        /// <param name="value">The metric value</param>
        /// <param name="context">Contextual properties to include</param>
        void Metric(string name, double value, IDictionary<string, string> context);

        /// <summary>
        /// Logs an event.
        /// </summary>
        /// <param name="name">The name of the event</param>
        /// <param name="context">Contextual properties to include</param>
        void Event(string name, IDictionary<string, string> context);

        /// <summary>
        /// Flushes any logs. Use before closing the process to ensure all information has been logged.
        /// </summary>
        void Flush();
    }
}
