using System.Diagnostics;

namespace MdBookSharp
{
    internal static class ConsoleLog
    {
        public static long started = Stopwatch.GetTimestamp();

        private static long _lastTick = Stopwatch.GetTimestamp();

        public static bool WriteLine(string text)
        {
            long currentTick = Stopwatch.GetTimestamp();
            double elapsedMs = Stopwatch.GetElapsedTime(_lastTick, currentTick).TotalMilliseconds;

            double elapsedAll = Stopwatch.GetElapsedTime(started, currentTick).TotalMilliseconds;

            Console.WriteLine($"{text} [{elapsedMs:F2} ms] ({elapsedAll:F2} ms)");

            _lastTick = currentTick;

            return true;
        }

        public static bool Error(string text)
        {
            long currentTick = Stopwatch.GetTimestamp();
            double elapsedMs = Stopwatch.GetElapsedTime(_lastTick, currentTick).TotalMilliseconds;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{text} [{elapsedMs:F2} ms]");
            Console.ForegroundColor = ConsoleColor.White;

            _lastTick = currentTick;
            Environment.Exit(1);

            return true;
        }
    }
}
