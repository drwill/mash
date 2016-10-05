using System;

namespace Mash.Chronograph
{
    /// <summary>
    /// Keeps track of elapsed time, iteration count, max, and average time spent for a given operation
    /// </summary>
    public interface IChronograph
    {
        /// <summary>
        /// Whether or not the chrono is running
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// The statistics for the current session
        /// </summary>
        Session ActiveSession { get; }

        /// <summary>
        /// Starts the time keeper
        /// </summary>
        /// <remarks>For threadsafe starting and stopping of the chrono, use the Capture method instead</remarks>
        void Start();

        /// <summary>
        /// Stops the time keeper
        /// </summary>
        /// <returns>The duration of the measured interval</returns>
        /// <remarks>For threadsafe starting and stopping of the chrono, use the Capture method instead</remarks>
        TimeSpan Stop();

        /// <summary>
        /// Measures the duration of the specified action, and updates the chrono statistics in a threadsafe way
        /// </summary>
        /// <param name="theAction">The action to capture</param>
        /// <returns>The duration of the measured action</returns>
        TimeSpan MeasureAction(Action theAction);

        /// <summary>
        /// Takes a snapshot of the chrono statistics and starts a fresh session
        /// </summary>
        /// <returns>A snapshot of the time statistics</returns>
        Session Restart();
    }
}
