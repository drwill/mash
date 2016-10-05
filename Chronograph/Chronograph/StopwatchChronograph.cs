using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Mash.Chronograph
{
    /// <summary>
    /// An implementation of IChronograph based on the System.Diagnostics.Stopwatch
    /// </summary>
    internal class StopwatchChronograph : IChronograph
    {
        public string Name { get; set; }

        public Session ActiveSession { get; private set; }

        public bool IsRunning { get { return _stopwatch.IsRunning; } }

        public void Start()
        {
            lock (_lockObject)
            {
                if (IsRunning)
                {
                    throw new InvalidOperationException("The chrono is currenting running so it cannot be started.");
                }

                _stopwatch.Restart();
            }
        }

        public TimeSpan Stop()
        {
            lock(_lockObject)
            {
                if (!IsRunning)
                {
                    throw new InvalidOperationException("The chrono is not currenting running so it cannot be stopped.");
                }

                _stopwatch.Stop();
                ActiveSession.AddLap(_stopwatch.Elapsed);

                return _stopwatch.Elapsed;
            }
        }

        public TimeSpan MeasureAction(Action theAction)
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            theAction();
            stopwatch.Stop();

            ActiveSession.AddLap(stopwatch.Elapsed);

            return stopwatch.Elapsed;
        }

        public async Task<TimeSpan> MeasureActionAsync(Func<Task> theAction)
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            await theAction();
            stopwatch.Stop();

            ActiveSession.AddLap(stopwatch.Elapsed);

            return stopwatch.Elapsed;
        }

        public Session Restart()
        {
            lock (_lockObject)
            {
                var snapshot = ActiveSession;
                ActiveSession = new Session(Name);

                return snapshot;
            }
        }

        internal StopwatchChronograph(string name)
        {
            Name = name;
            ActiveSession = new Session(Name);
        }

        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly object _lockObject = new object();
    }
}
