using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel;
using System;
using System.Collections.Generic;
using System.IO;

namespace Mash.Logging.ApplicationInsights
{
    /// <summary>
    /// An adapter for Mash.Logging.ILoggingProvider for Azure Application Insights.
    /// </summary>
    public class ApplicationInsightsLoggingProvider : ILoggingProvider, IDisposable
    {
        private readonly TelemetryClient _telemetryClient;
        private TelemetryConfiguration _telemetryConfiguration;
        private ServerTelemetryChannel _telemetryChannel;

        private static readonly string s_storageFolder = Path.Combine(Path.GetTempPath(), "mash-ai");

        /// <summary>
        /// Initializes an instance of <see cref="ApplicationInsightsLoggingProvider "/>.
        /// </summary>
        /// <param name="aiKey">The application insights key.</param>
        public ApplicationInsightsLoggingProvider(string aiKey)
        {
            if (!Directory.Exists(s_storageFolder))
            {
                Directory.CreateDirectory(s_storageFolder);
            }

            _telemetryChannel = new ServerTelemetryChannel
            {
                StorageFolder = s_storageFolder,
            };

            _telemetryConfiguration = new TelemetryConfiguration(aiKey)
            {
                TelemetryChannel = _telemetryChannel,
            };
            _telemetryChannel.Initialize(_telemetryConfiguration);

            _telemetryClient = new TelemetryClient(_telemetryConfiguration)
            {
                InstrumentationKey = aiKey,
            };
        }

        /// <summary>
        /// Initializes an instance of <see cref="ApplicationInsightsLoggingProvider "/> with the specified <see cref="TelemetryConfiguration"/>.
        /// </summary>
        /// <param name="telemetryConfiguration">Your own telemetry configuration.</param>
        /// <remarks>
        /// Use this to customize how your <see cref="TelemetryClient"/> is configured. This library assumes ownership of the
        /// <see cref="TelemetryConfiguration"/> instance, and will dispose of it when disposed.
        /// </remarks>
        public ApplicationInsightsLoggingProvider(TelemetryConfiguration telemetryConfiguration)
        {
            if (telemetryConfiguration == null)
            {
                throw new ArgumentNullException(nameof(telemetryConfiguration));
            }
            if (string.IsNullOrWhiteSpace(telemetryConfiguration.InstrumentationKey))
            {
                throw new ArgumentException("InstrumentationKey must be specified.", nameof(telemetryConfiguration.InstrumentationKey));
            }

            _telemetryConfiguration = telemetryConfiguration;
            _telemetryClient = new TelemetryClient(_telemetryConfiguration)
            {
                InstrumentationKey = _telemetryConfiguration.InstrumentationKey,
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
            if (_telemetryClient != null)
            {
                _telemetryClient.Flush();
            }

            if (_telemetryConfiguration != null)
            {
                _telemetryConfiguration.Dispose();
                _telemetryConfiguration = null;
            }

            if (_telemetryChannel != null)
            {
                _telemetryChannel?.Dispose();
                _telemetryChannel = null;
            }
        }
    }
}
