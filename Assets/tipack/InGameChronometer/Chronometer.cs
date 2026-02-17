using System.Diagnostics;
using System.Text;
using System;

namespace tiplay.GameToolkit
{
    public static class Chronometer
    {
        private static Stopwatch stopwatch = new Stopwatch();
        private static StringBuilder chronometerString = new StringBuilder();
        public static TimeSpan TimeSpan => stopwatch.Elapsed;
        public static bool IsRunning => stopwatch.IsRunning;
        public static long ElapsedMilliseconds => stopwatch.ElapsedMilliseconds;

        public static string GetTime()
        {
            chronometerString.Clear();
            chronometerString.AppendFormat(" {0}h", TimeSpan.Hours);
            chronometerString.AppendFormat(" {0}m", TimeSpan.Minutes);
            chronometerString.AppendFormat(" {0}s", TimeSpan.Seconds);
            chronometerString.AppendFormat(" {0}ms", TimeSpan.Milliseconds);
            return chronometerString.ToString();
        }

        public static void Start() => stopwatch.Start();
        public static void Restart() => stopwatch.Restart();
        public static void Stop() => stopwatch.Stop();
        public static void Reset() => stopwatch.Reset();
    }
}

