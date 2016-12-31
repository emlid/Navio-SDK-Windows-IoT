using Emlid.WindowsIot.Common;
using System;
using System.Globalization;
using System.Text;

namespace Emlid.WindowsIot.Hardware.Protocols.Pwm
{
    /// <summary>
    /// PWM frame data, a sequence of one or more PWM values sent together at a time.
    /// </summary>
    /// <remarks>
    /// Used as a standard variable container for both "PWM" (single channel) and other
    /// multi-channel protocols such as CPPM (a.k.a. PPM-Sum).
    /// The value of each channel depends on the protocol, i.e. what the start and end
    /// PWM lengths are and polarity (high or low as delimiter).
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
            Time = sequence;
            Channels = channels;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Tests two objects of this type for equality by value.
        /// </summary>
        public static bool operator ==(PwmFrame left, PwmFrame right)
        {
            return !ReferenceEquals(left, null)
                ? left.Equals(right)
                : ReferenceEquals(right, null);
        }

        /// <summary>
        /// Tests two objects of this type for inequality by value.
        /// </summary>
        public static bool operator !=(PwmFrame left, PwmFrame right)
        {
            return !ReferenceEquals(left, null)
                ? !left.Equals(right)
                : !ReferenceEquals(right, null);
        }

        /// <summary>
        /// Compares this object with another by value.
        /// </summary>
        /// <param name="value">Object with which to compare by value.</param>
        public override bool Equals(object value)
        {
            // Compare type
            var other = value as PwmFrame;
            if (ReferenceEquals(other, null))
                return false;

            // Compare values
            return
                other.Time == Time &&
                ArrayExtensions.AreEqual(other.Channels, Channels);
        }

        /// <summary>
        /// Returns a hash-code based on the current value of this object.
        /// </summary>
        public override int GetHashCode()
        {
            return
                Time.GetHashCode() ^
                ArrayExtensions.GetHashCode(Channels);
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
        public long Time { get; private set; }

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
            // Start with timestamp
            var result = new StringBuilder();
            result.AppendFormat(CultureInfo.CurrentCulture, Resources.Strings.PwmFrameStringFormatStart, Time);

            // Add each channel value
            for (var index = 0; index < Channels.Length; index++)
            {
                result.AppendFormat(CultureInfo.CurrentCulture,
                    Resources.Strings.PwmFrameStringFormatChannel, index + 1, Channels[index]);
            }

            // Return whole string
            return result.ToString();
        }

        #endregion
    }
}
