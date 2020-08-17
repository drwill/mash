using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Generic;

namespace Mash.Logging.ApplicationInsights
{
    /// <summary>
    /// An adapter for Mash.Logging.ILoggingProvider for Azure Application Insights
    /// </summary>
    public class ApplicationInsightsLoggingProvider : ILoggingProvider, IDisposable
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly TelemetryConfiguration _telemetryConfiguration;

        /// <summary>
        /// Initializes an instnace of <see cref="ApplicationInsightsLoggingProvider "/>.
        /// </summary>
        /// <param name="aiKey">The application insights key.</param>
        public ApplicationInsightsLoggingProvider(string aiKey)
        {
            _telemetryConfiguration = new TelemetryConfiguration(aiKey);
            _telemetryClient = new TelemetryClient(_telemetryConfiguration)
            {
                InstrumentationKey = aiKey,
            };
        }

        /// <inheritdoc />
        public void Trace(string message, TraceSeverity severity, IDictionary<string, string> context)
        {
            _telemetryClient.TrackTrace(message, (SeverityLevel)severity, context);
        }

        /// <inheritdoc />
        public void Event(string name, IDictionary<string, string> context)
        {
            _telemetryClient.TrackEvent(name, context);
        }

        /// <inheritdoc />
        public void Metric(string name, double value, IDictionary<string, string> context)
        {
            _telemetryClient.TrackMetric(name, value, context);
        }

        /// <inheritdoc />
        public void Flush()
        {
            _telemetryClient.Flush();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _telemetryConfiguration?.Dispose();
        }
    }
}
