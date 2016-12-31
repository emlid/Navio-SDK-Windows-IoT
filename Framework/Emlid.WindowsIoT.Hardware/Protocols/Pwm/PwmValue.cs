using System.Globalization;

namespace Emlid.WindowsIot.Hardware.Protocols.Pwm
{
    /// <summary>
    /// Contains information about a PWM value (time and low or high)
    /// </summary>
    public struct PwmValue
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance with the specified values.
        /// </summary>
        public PwmValue(long time, bool state)
        {
           Time = time;
           Level = state;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Tests two objects of this type for equality by value.
        /// </summary>
        public static bool operator ==(PwmValue  left, PwmValue  right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Tests two objects of this type for inequality by value.
        /// </summary>
        public static bool operator !=(PwmValue  left, PwmValue  right)
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
            if (!(value is PwmValue))
                return false;
            var other = (PwmValue)value;

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
        /// Timestamp at which the event occurred in microseconds.
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
                Resources.Strings.PwmValueStringFormat, Level ? 1 : 0, Time);
        }

        #endregion
    }
}
