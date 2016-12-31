using System.Globalization;

namespace Emlid.WindowsIot.Hardware.Protocols.Pwm
{
    /// <summary>
    /// Contains information about a PWM cycle (low-high cycle)
    /// </summary>
    public class PwmCycle
    {
        #region Constants

        /// <summary>
        /// Frequency which many analog servos support.
        /// </summary>
        /// <remarks>
        /// Always check the specification of your servo before enabling output to avoid damage!
        /// Digital servos are capable of frequencies over 100Hz, some between 300-400Hz and higher.
        /// Some analog servos may even have trouble with 50Hz, but as most other autopilots
        /// are using 50Hz are default we choose this as an acceptable default.
        ///  See http://pcbheaven.com/wikipages/How_RC_Servos_Works/ for more information.
        /// </remarks>
        public const float ServoSafeFrequency = 50;

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates an empty instance.
        /// </summary>
        public PwmCycle()
        {
        }

        /// <summary>
        /// Creates an instance with zeros except the time of the last cycle low.
        /// </summary>
        /// <remarks>Typically used as a follow-on to a previous cycle.</remarks>
        public PwmCycle(long lowTime)
        {
            LowTime = lowTime;
            LowLength = 0;
            HighTime = 0;
            HighLength = 0;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Tests two objects of this type for equality by value.
        /// </summary>
        public static bool operator ==(PwmCycle left, PwmCycle right)
        {
            return !ReferenceEquals(left, null)
                ? left.Equals(right)
                : ReferenceEquals(right, null);
        }

        /// <summary>
        /// Tests two objects of this type for inequality by value.
        /// </summary>
        public static bool operator !=(PwmCycle left, PwmCycle right)
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
            var other = value as PwmCycle;
            if (ReferenceEquals(other, null))
                return false;

            // Compare values
            return
                other.LowTime == LowTime &&
                other.LowLength == LowLength &&
                other.HighTime == HighTime &&
                other.HighLength == HighLength;
        }

        /// <summary>
        /// Returns a hash-code based on the current value of this object.
        /// </summary>
        public override int GetHashCode()
        {
            return
                LowTime.GetHashCode() ^
                LowLength.GetHashCode() ^
                HighTime.GetHashCode() ^
                HighLength.GetHashCode();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Timestamp of the low transition in microseconds.
        /// </summary>
        public long LowTime { get; set; }

        /// <summary>
        /// Length of time remained low time in microseconds.
        /// </summary>
        public long LowLength { get; set; }

        /// <summary>
        /// Timestamp of the high transition in microseconds.
        /// </summary>
        public long HighTime { get; set; }

        /// <summary>
        /// Length of the time remained high in microseconds.
        /// </summary>
        public long HighLength { get; set; }

        /// <summary>
        /// Total length of the cycle in microseconds.
        /// </summary>
        public long Length {  get { return LowLength + HighLength; } }

        #endregion

        #region Methods

        /// <summary>
        /// Return a string representation of the current values.
        /// </summary>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture,
                Resources.Strings.PwmCycleStringFormat,
                LowTime, LowLength, HighTime, HighLength, Length);
        }

        /// <summary>
        /// Checks if the cycle is valid.
        /// </summary>
        public bool IsValid()
        {
            return
                LowTime > 0 && LowLength > 0 &&
                HighTime > 0 && HighLength > 0 &&
                LowTime < HighTime;
        }

        #endregion
    }
}
