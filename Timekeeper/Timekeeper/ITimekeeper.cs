using System;

namespace Mash.Timekeeper
{
    /// <summary>
    /// Keeps track of elapsed time, iteration count, max, and average time spent for a given operation
    /// </summary>
    public interface ITimekeeper
    {
        /// <summary>
        /// Whether or not the timer is running
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// The statistics for the current interval
        /// </summary>
        TimekeeperSnapshot TimerStatistics { get; }

        /// <summary>
        /// Starts the time keeper
        /// </summary>
        /// <remarks>For threadsafe starting and stopping of the timer, use the Capture method instead</remarks>
        void StartCapture();

        /// <summary>
        /// Stops the time keeper
        /// </summary>
        /// <remarks>For threadsafe starting and stopping of the timer, use the Capture method instead</remarks>
        void StopCapture();

        /// <summary>
        /// Captures the duration of the specified action, and updates the timer statistics in a threadsafe way
        /// </summary>
        /// <param name="theAction">The action to capture</param>
        void Capture(Action theAction);

        /// <summary>
        /// Takes a snapshot of the timer statistics and starts fresh for a new interval
        /// </summary>
        /// <returns>A snapshot of the time statistics</returns>
        TimekeeperSnapshot Snapshot();
    }
}
