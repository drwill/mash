using System;

namespace Mash.Timekeeper
{
    /// <summary>
    /// Timer capture statistics for a custom interval
    /// </summary>
    public class TimekeeperSnapshot
    {
        /// <summary>
        /// The name of the timer
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// The number of captures
        /// </summary>
        public uint CaptureCount { get; internal set; }

        /// <summary>
        /// The sum of all capture durations in milliseconds
        /// </summary>
        public ulong TotalCaptureSum { get; internal set; }

        /// <summary>
        /// The average duration of each capture in milliseconds
        /// </summary>
        public ulong AverageCaptureDuration
        {
            get
            {
                return CaptureCount == 0
                    ? 0
                    : TotalCaptureSum / CaptureCount;
            }
        }

        /// <summary>
        /// The longest capture in the set in milliseconds
        /// </summary>
        public ulong MaxCaptureDuration { get; internal set; }

        /// <summary>
        /// The date and time in UTC of the start of the snapshot
        /// </summary>
        public DateTime CaptureStartUtc { get; }

        /// <summary>
        /// The date and time in UTC of the end of the snapshot
        /// </summary>
        public DateTime? CaptureEndUtc { get; private set; }

        /// <summary>
        /// The interval duration of the snapshot
        /// </summary>
        public TimeSpan? TimeSpan
        {
            get
            {
                if (!CaptureEndUtc.HasValue)
                {
                    return null;
                }

                return CaptureEndUtc.Value - CaptureStartUtc;
            }
        }

        internal TimekeeperSnapshot(string name)
        {
            CaptureEndUtc = null;
            CaptureStartUtc = DateTime.UtcNow;
            Name = name;
        }

        internal void Capture(ulong duration)
        {
            lock (_statisticsLock)
            {
                if (CaptureEndUtc.HasValue)
                {
                    throw new InvalidOperationException("No more captures can be added to the snapshot after it has an end date. This might be a timekeeper bug.");
                }

                ++CaptureCount;
                TotalCaptureSum += duration;
                MaxCaptureDuration = Math.Max(duration, MaxCaptureDuration);
            }
        }

        internal void EndCapture()
        {
            lock (_statisticsLock)
            {
                CaptureEndUtc = DateTime.UtcNow;
            }
        }

        private object _statisticsLock = new object();
    }
}
