namespace Mash.Timekeeper
{
    /// <summary>
    /// The summary statistics for a set of time keeper captures
    /// </summary>
    public interface ITimekeeperSnapshot
    {
        /// <summary>
        /// The name of the timer
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The number of times the timer has been started and stopped
        /// </summary>
        uint CaptureCount { get; }

        /// <summary>
        /// The sum of all the captures, in milliseconds
        /// </summary>
        ulong TotalCaptureDuration { get; }

        /// <summary>
        /// The average capture duration, in milliseconds
        /// </summary>
        ulong AverageCaptureDuration { get; }

        /// <summary>
        /// The maximum duration of any of the captures, in milliseconds
        /// </summary>
        ulong MaxCaptureDuration { get; }

        /// <summary>
        /// The latest capture, in milliseconds
        /// </summary>
        ulong LatestCaptureDuration { get; }
    }
}
