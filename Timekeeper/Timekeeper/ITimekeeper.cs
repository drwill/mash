using System;

namespace Mash.Timekeeper
{
    /// <summary>
    /// Keeps track of elapsed time, iteration count, max, and average time spent for a given operation
    /// </summary>
    public interface ITimekeeper : ITimekeeperSnapshot
    {
        /// <summary>
        /// Whether or not the timer is running
        /// </summary>
        bool IsRunning { get; }

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
        /// Resets the timer statistics back to initial state
        /// </summary>
        /// <returns>A snapshot of the time statistics</returns>
        ITimekeeperSnapshot Reset();
    }
}
