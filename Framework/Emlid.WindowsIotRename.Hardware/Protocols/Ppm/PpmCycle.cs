using System.Globalization;

namespace Emlid.WindowsIot.Hardware.Protocols.Ppm
{
    /// <summary>
    /// Contains information about a PPM cycle (low-high pulse cycle)
    /// </summary>
    public struct PpmCycle
    {
        #region Lifetime

        /// <summary>
        /// Creates an almost empty instance, with only the initial start (low) time.
        /// </summary>
        public PpmCycle(long lowTime)
        {
            LowTime = lowTime;
            HighTime = 0;
            LowLength = 0;
            HighLength = 0;
        }

        /// <summary>
        /// Sets values with <see cref="LowLength"/> calculated automatically.
        /// </summary>
        /// <param name="lowTime">Low time in microseconds.</param>
        /// <param name="highTime">High time in microseconds.</param>
        /// <param name="highLength">High length in microseconds.</param>
        public PpmCycle(long lowTime, long highTime, long highLength)
        {
            LowTime = lowTime;
            HighTime = highTime;
            LowLength = highTime - lowTime;
            HighLength = highLength;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Tests two objects of this type for equality by value.
        /// </summary>
        public static bool operator ==(PpmCycle left, PpmCycle right)
        {
            return !ReferenceEquals(left, null)
                ? left.Equals(right)
                : ReferenceEquals(right, null);
        }

        /// <summary>
        /// Tests two objects of this type for inequality by value.
        /// </summary>
        public static bool operator !=(PpmCycle left, PpmCycle right)
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
            if (!(value is PpmCycle))
                return false;
            var other = (PpmCycle)value;

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
        /// Checks if the cycle is valid.
        /// </summary>
        public bool IsValid()
        {
            return
                LowTime > 0 && LowLength > 0 &&
                HighTime > 0 && HighLength > 0 &&
                LowTime < HighTime;
        }

        /// <summary>
        /// Copies values from another instance.
        /// </summary>
        /// <param name="source">Source from which to copy values.</param>
        public void Copy(PpmCycle source)
        {
            LowTime = source.LowTime;
            LowLength = source.LowLength;
            HighTime = source.HighTime;
            HighLength = source.HighLength;
        }

        /// <summary>
        /// Return a string representation of the current values.
        /// </summary>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture,
                Resources.Strings.PpmCycleFormat,
                LowTime, LowLength, HighTime, HighLength, Length);
        }

        #endregion
    }
}
