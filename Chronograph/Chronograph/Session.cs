using System;

namespace Mash.Chronograph
{
    /// <summary>
    /// chrono capture statistics for a session of lap times
    /// </summary>
    public class Session
    {
        /// <summary>
        /// The name of the chronograph
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// The number of measures
        /// </summary>
        public uint Laps { get; internal set; }

        /// <summary>
        /// The sum of all lap times
        /// </summary>
        public TimeSpan Sum { get; internal set; }

        /// <summary>
        /// The average duration of each capture in milliseconds
        /// </summary>
        public TimeSpan Mean
        {
            get
            {
                if (Laps == 0)
                {
                    return new TimeSpan();
                }

                var ms = Sum.Ticks / Laps;
                return TimeSpan.FromTicks(ms);
            }
        }

        /// <summary>
        /// The longest capture in the set in milliseconds
        /// </summary>
        public TimeSpan Max { get; internal set; }

        /// <summary>
        /// The date and time in UTC of the start of the snapshot
        /// </summary>
        public DateTimeOffset SessionStartUtc { get; }

        /// <summary>
        /// The date and time in UTC of the end of the snapshot
        /// </summary>
        public DateTimeOffset SessionEndUtc { get; private set; }

        internal Session(string name)
        {
            SessionStartUtc = DateTimeOffset.UtcNow;
            SessionEndUtc = SessionStartUtc;
            Name = name;
        }

        internal void AddLap(TimeSpan duration)
        {
            lock (_statisticsLock)
            {
                SessionEndUtc = DateTimeOffset.UtcNow;

                ++Laps;

                Sum = Sum.Add(duration);

                if (duration > Max)
                {
                    Max = duration;
                }
            }
        }

        private object _statisticsLock = new object();
    }
}
