using System;
using System.Diagnostics;

namespace Mash.Timekeeper
{
    /// <summary>
    /// An implementation of ITimekeeper based on the System.Diagnostics.Stopwatch
    /// </summary>
    internal class StopwatchTimekeeper : ITimekeeper
    {
        public string Name { get; set; }

        public TimekeeperSnapshot TimerStatistics { get; private set; }

        public bool IsRunning { get { return _stopwatch.IsRunning; } }

        public void StartCapture()
        {
            lock (_lockObject)
            {
                if (IsRunning)
                {
                    throw new InvalidOperationException("The timer is currenting running so it cannot be started.");
                }

                _stopwatch.Restart();
            }
        }

        public void StopCapture()
        {
            lock(_lockObject)
            {
                if (!IsRunning)
                {
                    throw new InvalidOperationException("The timer is not currenting running so it cannot be stopped.");
                }

                _stopwatch.Stop();
                TimerStatistics.Capture((ulong)_stopwatch.ElapsedMilliseconds);
            }
        }

        public void Capture(Action theAction)
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            theAction();
            stopwatch.Stop();

            TimerStatistics.Capture((ulong)stopwatch.ElapsedMilliseconds);
        }

        public TimekeeperSnapshot Snapshot()
        {
            lock (_lockObject)
            {
                var snapshot = TimerStatistics;
                TimerStatistics = new TimekeeperSnapshot(Name);
                snapshot.EndCapture();

                return snapshot;
            }
        }

        internal StopwatchTimekeeper(string name)
        {
            Name = name;
            TimerStatistics = new TimekeeperSnapshot(Name);
        }

        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly object _lockObject = new object();
    }
}
