namespace BlockScanner.Helpers
{
    using System;
    using System.Diagnostics;

    // Dirty timer class to assist some basic profiling.
    public class TimerHelper : IDisposable
    {
        private readonly Stopwatch stopWatch = new Stopwatch();
        private readonly string description;

        public TimerHelper(string description = null)
        {
            this.description = description;

            stopWatch.Start();
        }

        public void Dispose()
        {
            WriteDescriptionWithMillisecondsToConsole(description, stopWatch);

            stopWatch.Stop();
        }

        private static void WriteDescriptionWithMillisecondsToConsole(string description, Stopwatch stopWatch)
        {
            // Should probably farm this off to another output.
            Console.WriteLine($"{description} took {stopWatch.Elapsed.TotalMilliseconds} ms");
        }

        public static T Profile<T>(Func<T> function, string description = "")
        {
            Stopwatch stopWatch = new Stopwatch();

            stopWatch.Start();

            T res = function();

            stopWatch.Stop();

            WriteDescriptionWithMillisecondsToConsole(description, stopWatch);

            return res;
        }
    }
}
