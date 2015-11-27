using System;
using System.Diagnostics;

namespace Mash.Timekeeper
{
    internal class StopwatchWrapper : ITimekeeper
    {
        public string Name { get; set; }

        public uint CaptureCount { get; private set; }

        public ulong TotalCaptureDuration { get; private set; }

        public ulong AverageCaptureDuration
        {
            get
            {
                return CaptureCount == 0
                    ? 0
                    : TotalCaptureDuration / CaptureCount;
            }
        }

        public ulong LatestCaptureDuration { get; private set; }

        public ulong MaxCaptureDuration { get; private set; }

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
            }

            UpdateStats((ulong)_stopwatch.ElapsedMilliseconds);
        }

        public void Capture(Action theAction)
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            theAction();
            stopwatch.Stop();

            UpdateStats((ulong)stopwatch.ElapsedMilliseconds);
        }

        public ITimekeeperSnapshot Reset()
        {
            lock (_lockObject)
            {
                var snapshot = new TimekeeperSnapshot(
                    Name,
                    CaptureCount,
                    TotalCaptureDuration,
                    LatestCaptureDuration,
                    MaxCaptureDuration);

                CaptureCount = 0;
                TotalCaptureDuration = 0;
                LatestCaptureDuration = 0;
                MaxCaptureDuration = 0;

                return snapshot;
            }
        }

        private void UpdateStats(ulong latest)
        {
            lock (_lockObject)
            {
                if (IsRunning)
                {
                    throw new InvalidOperationException("Statistics cannot be updated while the timer is running.");
                }

                ++CaptureCount;
                TotalCaptureDuration += latest;
                LatestCaptureDuration = latest;
                MaxCaptureDuration = Math.Max(latest, MaxCaptureDuration);
            }
        }

        internal StopwatchWrapper(string name)
        {
            Name = name;
        }

        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly Object _lockObject = new object();
    }
}
