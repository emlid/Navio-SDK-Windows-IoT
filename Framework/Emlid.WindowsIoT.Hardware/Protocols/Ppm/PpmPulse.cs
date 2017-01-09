using System.Globalization;

namespace Emlid.WindowsIot.Hardware.Protocols.Ppm
{
    /// <summary>
    /// Contains information about a PPM (Pulse Position Modulation) pulse (time and logic level).
    /// </summary>
    /// <see href="https://en.wikipedia.org/wiki/Pulse-position_modulation"/>
    public struct PpmPulse
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance with the specified values.
        /// </summary>
        public PpmPulse(long time, bool state)
        {
           Time = time;
           Level = state;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Tests two objects of this type for equality by value.
        /// </summary>
        public static bool operator ==(PpmPulse  left, PpmPulse  right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Tests two objects of this type for inequality by value.
        /// </summary>
        public static bool operator !=(PpmPulse  left, PpmPulse  right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Compares this object with another by value.
        /// </summary>
        /// <param name="value">Object with which to compare by value.</param>
        public override bool Equals(object value)
        {
            // Compare type
            if (!(value is PpmPulse))
                return false;
            var other = (PpmPulse)value;

            // Compare values
            return
                other.Time == Time &&
                other.Level == Level;
        }

        /// <summary>
        /// Returns a hash-code based on the current value of this object.
        /// </summary>
        public override int GetHashCode()
        {
            return
                Time.GetHashCode() ^
                Level.GetHashCode();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Time at which the event occurred in microseconds.
        /// </summary>
        public long Time { get; set; }

        /// <summary>
        /// Logic level of the pin (true when high/1, false when low/0).
        /// </summary>
        public bool Level { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Return a string representation of the current values.
        /// </summary>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture,
                Resources.Strings.PpmPulseFormat, Level ? 1 : 0, Time);
        }

        #endregion
    }
}
