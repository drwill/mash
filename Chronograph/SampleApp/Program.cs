using System;
using System.Threading;

namespace Mash.Chronograph.SampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var chrono = ChronographFactory.GetChronograph("test");

            // Use Start/StopCapture methods
            chrono.Start();
            TheOperation();
            chrono.Stop();
            PrintChronoStatistics(chrono.ActiveSession);

            // Use MeasureAction method
            chrono.MeasureAction(() => { TheOperation(); });
            PrintChronoStatistics(chrono.ActiveSession);

            // Measure a bunch more actions
            for (int i = 0; i < 10; ++i)
            {
                TimeSpan elapsed = chrono.MeasureAction(TheOperation);
                Console.WriteLine($"\tAction measured at {elapsed.TotalMilliseconds:N0} ms");
            }

            var snapshot = chrono.Restart();
            PrintChronoStatistics(snapshot);

            Console.WriteLine("After reset, the new stats should be zeroes");
            PrintChronoStatistics(chrono.ActiveSession);
        }

        static void TheOperation()
        {
            int randValue = _rand.Next(10, 501);

            Console.WriteLine($"\tRunning an operation; sleeping for {randValue} ms");

            Thread.Sleep(randValue);
        }

        static void PrintChronoStatistics(Session snapshot)
        {
            Console.WriteLine("Statistics:");
            Console.WriteLine($"\t{snapshot.Name} from {snapshot.SessionStartUtc} to {snapshot.SessionEndUtc}");
            Console.WriteLine($"\tCount: {snapshot.Laps}");
            Console.WriteLine($"\tTotal: {snapshot.Sum}");
            Console.WriteLine($"\tMax: {snapshot.Max}");
            Console.WriteLine($"\tMean: {snapshot.Mean}");
            Console.WriteLine();
        }

        static readonly Random _rand = new Random();
    }
}
