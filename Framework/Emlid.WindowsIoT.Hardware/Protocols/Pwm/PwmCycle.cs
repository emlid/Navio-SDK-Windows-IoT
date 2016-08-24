using System.Globalization;

namespace Emlid.WindowsIot.Hardware.Protocols.Pwm
{
    /// <summary>
    /// Contains information about a PWM cycle (low-high cycle)
    /// </summary>
    public struct PwmCycle
    {
        #region Lifetime

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
