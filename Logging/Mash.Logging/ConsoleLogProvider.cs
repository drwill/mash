using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mash.Logging
{
    /// <summary>
    /// A simple console logger.
    /// </summary>
    public class ConsoleLogProvider : ILoggingProvider
    {
        private static readonly IReadOnlyDictionary<TraceSeverity, ConsoleColor> _traceColors = new Dictionary<TraceSeverity, ConsoleColor>
        {
            { TraceSeverity.Critical, ConsoleColor.Red },
            { TraceSeverity.Error, ConsoleColor.Red },
            { TraceSeverity.Warning, ConsoleColor.Yellow },
        };

        /// <summary>
        /// Whether the contextual properties should be logged to the console.
        /// </summary>
        public bool ShouldLogContext { get; set; }

        /// <summary>
        /// Whether the traces should use color to emphasize the severity level.
        /// </summary>
        public bool ShouldUseColor { get; set; } = true;

        /// <inheritdoc />
        public void Trace(string message, TraceSeverity traceSev, IDictionary<string, string> context)
        {
            ConsoleColor? color = null;
            if (ShouldUseColor
                && _traceColors.TryGetValue(traceSev, out ConsoleColor outColor))
            {
                color = outColor;
            }

            WriteConsoleText($"{traceSev}: {message}", context, color);
        }

        /// <inheritdoc />
        public void Event(string name, IDictionary<string, string> context)
        {
            WriteConsoleText(name, context);
        }

        /// <inheritdoc />
        public void Metric(string name, double value, IDictionary<string, string> context)
        {
            WriteConsoleText($"{name}: {value}", context);
        }

        /// <inheritdoc />
        public void Flush() { }

        private void WriteConsoleText(string message, IDictionary<string, string> context, ConsoleColor? color = null)
        {
            string content = BuildConsoleText(message, context);

            if (!ShouldUseColor)
            {
                Console.WriteLine(content);
                return;
            }

            ConsoleColor originalColor = Console.ForegroundColor;
            if (color.HasValue)
            {
                Console.ForegroundColor = color.Value;
            }

            Console.WriteLine(content);

            if (color.HasValue)
            {
                Console.ForegroundColor = originalColor;
            }
        }

        private string BuildConsoleText(string message, IDictionary<string, string> context)
        {
            if (!ShouldLogContext || !context.Any())
            {
                return message;
            }

            var sb = new StringBuilder(message);
            sb.AppendLine();
            foreach (KeyValuePair<string, string> kvp in context)
            {
                sb.AppendLine($"\t{kvp.Key}: {kvp.Value}");
            }

            return sb.ToString();
        }
    }
}
