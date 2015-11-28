using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mash.Timekeeper.SampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var timekeeper = Factory.GetTimekeeperInstance("test");

            // Use Start/StopCapture methods
            timekeeper.StartCapture();
            TheOperation();
            timekeeper.StopCapture();
            PrintTimerStatistics(timekeeper.TimerStatistics);

            // Use Capture method
            timekeeper.Capture(() => { TheOperation(); });
            PrintTimerStatistics(timekeeper.TimerStatistics);

            // Capture a bunch more times
            for (int i = 0; i < 50; ++i)
            {
                timekeeper.Capture(TheOperation);
            }

            var snapshot = timekeeper.Snapshot();
            PrintTimerStatistics(snapshot);

            Console.WriteLine("After reset, the new stats should be zeroes");
            PrintTimerStatistics(timekeeper.TimerStatistics);
        }

        static void TheOperation()
        {
            int randValue = _rand.Next(10, 501);

            Console.WriteLine($"Sleeping for {randValue}");

            Thread.Sleep(randValue);
        }

        static void PrintTimerStatistics(TimekeeperSnapshot snapshot)
        {
            Console.WriteLine($"{snapshot.Name} from {snapshot.CaptureStartUtc} to {snapshot.CaptureEndUtc}");
            Console.WriteLine($"\tCount: {snapshot.CaptureCount}");
            Console.WriteLine($"\tTotal: {snapshot.TotalCaptureSum}");
            Console.WriteLine($"\tMax: {snapshot.MaxCaptureDuration}");
            Console.WriteLine($"\tAverage: {snapshot.AverageCaptureDuration}");
        }

        static readonly Random _rand = new Random();
    }
}
