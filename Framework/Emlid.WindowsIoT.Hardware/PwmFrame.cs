using System;
using System.Globalization;
using System.Text;

namespace Emlid.WindowsIot.Hardware
{
    /// <summary>
    /// Contains data and definitions of an RC input PWM frame.
    /// </summary>
    /// <remarks>
    /// Used as a standard variable container for both "PWM" (single channel) and other
    /// multi-channel protocols such as CPPM (a.k.a. PPM-Sum).
    /// </remarks>
    public class PwmFrame
    {
        #region Lifetime

        /// <summary>
        /// Creates an empty instance.
        /// </summary>
        public PwmFrame()
        {
        }

        /// <summary>
        /// Creates an instance using the specified data.
        /// </summary>
        public PwmFrame(long sequence, int[] channels)
        {
            // Validate
            if (channels == null) throw new ArgumentNullException(nameof(channels));

            // Initialize
            Timestamp = sequence;
            Channels = channels;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Timestamp in microseconds, when the frame was captured.
        /// </summary>
        /// <remarks>
        /// Does not necessarily relate to any actual date and time,
        /// because the source is a high resolution timer,
        /// not the system clock.
        /// </remarks>
        public long Timestamp { get; private set; }

        /// <summary>
        /// Channel values in microseconds.
        /// </summary>
        public int[] Channels { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Return a string representation of the current values.
        /// </summary>
        public override string ToString()
        {
            var result = new StringBuilder();
            result.AppendFormat(CultureInfo.CurrentCulture, "PWM Frame @{0}", Timestamp);
            for (var index = 0; index < Channels.Length; index++)
                result.AppendFormat(CultureInfo.CurrentCulture, " #{0}={1}", index + 1, Channels[index]);
            return result.ToString();
        }

        #endregion
    }
}
