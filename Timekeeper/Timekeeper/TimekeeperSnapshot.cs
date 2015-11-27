namespace Mash.Timekeeper
{
    internal class TimekeeperSnapshot : ITimekeeperSnapshot
    {
        public string Name { get; set; }

        public uint CaptureCount { get; }

        public ulong TotalCaptureDuration { get; }

        public ulong AverageCaptureDuration { get; }

        public ulong MaxCaptureDuration { get; }

        public ulong LatestCaptureDuration { get; }

        public TimekeeperSnapshot(string name, uint captureCount, ulong totalCaptureDuration, ulong latestCaptureDuration, ulong maxCaptureDuration)
        {
            Name = name;
            CaptureCount = captureCount;
            TotalCaptureDuration = totalCaptureDuration;
            LatestCaptureDuration = latestCaptureDuration;
            MaxCaptureDuration = maxCaptureDuration;
        }
    }
}
