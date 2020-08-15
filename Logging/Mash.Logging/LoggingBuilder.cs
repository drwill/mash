using System.Collections.Generic;

namespace Mash.Logging
{
    /// <summary>
    /// Configures logging for the application, and builds loggers with those settings.
    /// </summary>
    public class LoggingBuilder
    {
        private readonly CentralLogger _centralLogger;

        /// <summary>
        /// The destinations for logs.
        /// </summary>
        /// <remarks>If no providers are specified, logging is a no-op.</remarks>
        public IList<ILoggingProvider> LogProviders { get; } = new List<ILoggingProvider>();

        /// <summary>
        /// A common set of logging properties to include with every trace, metric, and event call.
        /// </summary>
        public IDictionary<string, string> AppContext { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Constructs the object.
        /// </summary>
        public LoggingBuilder()
        {
            // Pass the instances of these collections so the central logger can use them, but they
            // remain here so the user can edit them.
            _centralLogger = new CentralLogger(LogProviders, AppContext);
        }

        /// <summary>
        /// Builds a logger with the configured settings.
        /// </summary>
        /// <param name="properties">Logger-specific properties to initialize on the new logger.</param>
        /// <returns>An instance of the logger.</returns>
        public Logger BuildLogger(IDictionary<string, string> properties = null)
        {
            return new Logger(_centralLogger, properties);
        }

        /// <summary>
        /// Flushes any logs. Use before closing the process to ensure all information has been logged.
        /// </summary>
        public void Flush()
        {
            _centralLogger.Flush();
        }
    }
}
