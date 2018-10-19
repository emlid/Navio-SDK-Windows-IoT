using System;
using System.Globalization;

namespace Emlid.WindowsIot.Hardware.Protocols.Pwm
{
    /// <summary>
    /// Contains information about a PWM (Pulse Width Modulation) pulse (length and polarity).
    /// </summary>
    /// <see href="https://en.wikipedia.org/wiki/Pulse-width_modulation"/>
    /// <see href="https://www.oscium.com/sites/default/files/wipry-combo/Final,%2520Tutorial%2520on%2520Pulse%2520Width%2520and%2520Duty%2520Cycle.pdf"/>
    public struct PwmPulse : IEquatable<PwmPulse>
    {
        #region Constants

        /// <summary>
        /// Frequency in Hz which many analog servos support.
        /// </summary>
        /// <remarks>
        /// Always check the specification of your servo before enabling output to avoid damage!
        /// Digital servos are capable of frequencies over 100Hz, some between 300-400Hz and higher.
        /// Some analog servos may even have trouble with 50Hz, but as most other autopilots
        /// are using 50Hz are default we choose this as an acceptable default.
        /// </remarks>
        /// <see href="http://pcbheaven.com/wikipages/How_RC_Servos_Works/"/>
        public const int ServoSafeFrequency = 50;

        #endregion Constants

        #region Operators

        /// <summary>
        /// Tests two objects of this type for equality by value.
        /// </summary>
        public static bool operator ==(PwmPulse left, PwmPulse right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Tests two objects of this type for inequality by value.
        /// </summary>
        public static bool operator !=(PwmPulse left, PwmPulse right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Compares this object with another by value.
        /// </summary>
        /// <param name="something">Object with which to compare by value.</param>
        public override bool Equals(object something)
        {
            return something is PwmPulse other && Equals(other);
        }

        /// <summary>
        /// Compares this object with another of the same type by value.
        /// </summary>
        /// <param name="other">Object with which to compare by value.</param>
        public bool Equals(PwmPulse other)
        {
            return
                other.Width == Width &&
                other.Frequency == Frequency;
        }

        /// <summary>
        /// Returns a hash-code based on the current value of this object.
        /// </summary>
        public override int GetHashCode()
        {
            return
                Width.GetHashCode() ^
                Frequency.GetHashCode();
        }

        #endregion Operators

        #region Properties

        /// <summary>
        /// Pulse Repetition Frequency (PRF) in Hz.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Setting this value calculates <see cref="DutyCycle"/>.
        /// </para>
        /// <para>
        /// Some PWM devices do not tolerate high values and could be damaged if this is set too high,
        /// e.g. analog servos operate at much lower frequencies than digital servos.
        /// See <see cref="ServoSafeFrequency"/> for more information.
        /// </para>
        /// </remarks>
        public int Frequency { get; set; }

        /// <summary>
        /// Pulse Repetition Interval (PRI) in milliseconds.
        /// </summary>
        /// <remarks>
        /// The time between sequential pulses, from the beginning of one pulse to the next.
        /// </remarks>
        public decimal Interval { get; set; }

        /// <summary>
        /// Pulse Width (PW) in fractions of a millisecond.
        /// </summary>
        /// <remarks>
        /// Cannot be greater than <see cref="Frequency"/>.
        /// Setting this value calculates <see cref="DutyCycle"/>.
        /// </remarks>
        public decimal Width { get; set; }

        /// <summary>
        /// Duty cycle, the percentage of <see cref="Width"/> over <see cref="Frequency"/>.
        /// </summary>
        /// <remarks>
        /// Setting this value calculates <see cref="Width"/>.
        /// </remarks>
        public decimal DutyCycle { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Creates a <see cref="PwmPulse"/> from frequency and width.
        /// </summary>
        public static PwmPulse FromWidth(int frequency, decimal width)
        {
            // Validate
            if (frequency < 0) throw new ArgumentOutOfRangeException(nameof(frequency));
            if (width < 0) throw new ArgumentOutOfRangeException(nameof(width));

            // Calculate interval and duty cycle
            var interval = 1000m / frequency;
            var dutyCycle = (width / interval) * 100m;

            // Initialize values
            return new PwmPulse
            {
                Frequency = frequency,
                Width = width,
                Interval = interval,
                DutyCycle = dutyCycle
            };
        }

        /// <summary>
        /// Creates a <see cref="PwmPulse"/> from frequency and duty cycle.
        /// </summary>
        public static PwmPulse FromDutyCycle(int frequency, decimal dutyCycle)
        {
            // Validate
            if (frequency < 0) throw new ArgumentOutOfRangeException(nameof(frequency));
            if (dutyCycle < 0 || dutyCycle > 100) throw new ArgumentOutOfRangeException(nameof(dutyCycle));

            // Calculate interval and duty cycle
            var interval = 1000m / frequency;
            var width = (interval / 100m) * dutyCycle;

            // Initialize values
            return new PwmPulse
            {
                Frequency = frequency,
                Width = width,
                Interval = interval,
                DutyCycle = dutyCycle
            };
        }

        /// <summary>
        /// Return a string representation of the current values.
        /// </summary>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture,
                Resources.Strings.PwmPulseFormat, Width, Frequency, DutyCycle);
        }

        #endregion Methods
    }
}