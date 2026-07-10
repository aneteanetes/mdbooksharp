using System.Diagnostics;

namespace mdbooksharplib
{
    public static class ConsoleLog
    {
        public static bool IsLog = true;

        public static long started = Stopwatch.GetTimestamp();

        private static long _lastTick = Stopwatch.GetTimestamp();

        public static bool WriteLine(string text)
        {
            if (!IsLog)
                return false;

            long currentTick = Stopwatch.GetTimestamp();
            double elapsedMs = Stopwatch.GetElapsedTime(_lastTick, currentTick).TotalMilliseconds;

            double elapsedAll = Stopwatch.GetElapsedTime(started, currentTick).TotalMilliseconds;

            Console.WriteLine($"{text} [{elapsedMs:F2} ms] ({elapsedAll:F2} ms)");

            _lastTick = currentTick;

            return true;
        }

        public static bool Error(string text)
        {
            if (!IsLog)
                return false;

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
