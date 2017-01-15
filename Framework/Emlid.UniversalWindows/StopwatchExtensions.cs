using System.Diagnostics;

namespace Emlid.UniversalWindows
{
    /// <summary>
    /// Extensions to the <see cref="Stopwatch"/> class.
    /// </summary>
    public static class StopwatchExtensions
    {
        /// <summary>
        /// Number of <see cref="Stopwatch.ElapsedTicks"/> in a microsecond.
        /// </summary>
        public static readonly double TicksPerMicrosecond = Stopwatch.Frequency / 1000000D;

        /// <summary>
        /// Gets the total elapsed time in microseconds.
        /// </summary>
        public static long ElapsedMicroseconds(this Stopwatch timer)
        {
            return (long)(timer.ElapsedTicks / TicksPerMicrosecond);
        }

        /// <summary>
        /// Gets the total elapsed time in microseconds.
        /// </summary>
        public static long GetTimestampInMicroseconds()
        {
            return (long)(Stopwatch.GetTimestamp() / TicksPerMicrosecond);
        }
    }
}
