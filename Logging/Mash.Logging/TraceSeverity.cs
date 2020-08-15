namespace Mash.Logging
{
    /// <summary>
    /// Identifies the severity level of the trace.
    /// </summary>
    public enum TraceSeverity
    {
        /// <summary>
        /// Debugging-level traces.
        /// </summary>
        Verbose,

        /// <summary>
        /// Traces of noteworthy mention.
        /// </summary>
        Information,

        /// <summary>
        /// Unexpected but not erroneous events.
        /// </summary>
        Warning,

        /// <summary>
        /// Erroneous events.
        /// </summary>
        Error,

        /// <summary>
        /// Application critical events.
        /// </summary>
        Critical,
    }
}
