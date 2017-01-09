using System;

namespace Emlid.WindowsIot.Hardware.Components.Pca9685
{
    /// <summary>
    /// <see cref="Pca9685Device"/> channel value.
    /// </summary>
    public struct Pca9685ChannelValue
    {
        #region Constants

        /// <summary>
        /// Maximum value (4095).
        /// </summary>
        public const int Maximum = 0x0fff;

        /// <summary>
        /// This value, one higher than <see cref="Maximum"/>, causes the channel to be always on or off.
        /// The same as one whole clock cycle.
        /// </summary>
        /// <remarks>
        /// If both always on and always off are set, off takes precedence.
        /// </remarks>
        public const int Always = 0x1000;

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates an instance with the specified values
        /// </summary>
        public Pca9685ChannelValue(int on = 0, int off = 0) : this()
        {
            // Validate
            if (on < 0 || on > Always) throw new ArgumentOutOfRangeException(nameof(on));
            if (off < 0 || off > Always) throw new ArgumentOutOfRangeException(nameof(off));

            // Initialize values
            _on = on;
            _off = off;
            _width = CalculateWidth(On, Off);
        }

        /// <summary>
        /// Creates an instance with the values from a byte array.
        /// </summary>
        public Pca9685ChannelValue(byte[] data, int offset = 0)  : this()
        {
            // Validate
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (data.Length < offset + Pca9685Device.ChannelSize)
                throw new ArgumentOutOfRangeException(nameof(data));

            // Extract values from byte array
            var on = BitConverter.ToUInt16(data, offset);
            var off = BitConverter.ToUInt16(data, offset + sizeof(ushort));
            _width = CalculateWidth(On, Off);
        }

        #endregion

        #region Operators

        /// <summary>
        /// Tests two objects of this type for equality by value.
        /// </summary>
        public static bool operator ==(Pca9685ChannelValue left, Pca9685ChannelValue right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Tests two objects of this type for inequality by value.
        /// </summary>
        public static bool operator !=(Pca9685ChannelValue left, Pca9685ChannelValue right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Compares this object with another by value.
        /// </summary>
        /// <param name="value">Object with which to compare by value.</param>
        public override bool Equals(object value)
        {
            // Compare nullability and type
            if (!(value is Pca9685ChannelValue))
                return false;
            var other = (Pca9685ChannelValue)value;

            // Compare values
            return
                other._on == _on &&
                other._off == _off &&
                other._width == _width;
        }

        /// <summary>
        /// Returns a hash-code based on the current value of this object.
        /// </summary>
        public override int GetHashCode()
        {
            return
                _on.GetHashCode() ^
                _off.GetHashCode() ^
                _width.GetHashCode();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 12-bit rising "on" time in ticks within the range 0-<see cref="Maximum"/> or <see cref="Always"/> for always on mode.
        /// </summary>
        /// <remarks>
        /// When greater than <see cref="Off"/> the pulse spans two clock cycles.
        /// Set via <see cref="Width"/> to automatically set <see cref="Always"/> with a width of <see cref="Maximum"/>.
        /// Always off takes precedence to always on when both are set.
        /// </remarks>
        public int On
        {
            get { return _on; }
            set
            {
                // Do nothing when same
                if (_on == value)
                    return;

                // Validate
                if (value < 0 || value > Always)
                    throw new ArgumentNullException(nameof(Off));

                // Set value
                _on = value;
            }
        }
        private int _on;

        /// <summary>
        /// 12-bit falling "off" time in ticks within the range 0-<see cref="Maximum"/> or <see cref="Always"/> for always off mode.
        /// </summary>
        /// <remarks>
        /// When less than <see cref="On"/> the pulse spans two clock cycles.
        /// Set via <see cref="Width"/> to automatically set <see cref="Always"/> with a width of zero.
        /// Always off takes precedence to always on when both are set.
        /// </remarks>
        public int Off
        {
            get { return _off; }
            set
            {
                // Do nothing when same
                if (_off == value)
                    return;

                // Validate
                if (value < 0 || value > Always)
                    throw new ArgumentNullException(nameof(Off));

                // Set value
                _off = value;
            }
        }
        private int _off;

        /// <summary>
        /// Width of the pulse in ticks, the difference between <see cref="On"/> and <see cref="Off"/>.
        /// </summary>
        public int Width
        {
            get
            {
                // Return cached value
                return _width;
            }
            set
            {
                // Do nothing when same
                if (_width == value)
                    return;

                // Validate
                if (value < 0 || value > Maximum)
                    throw new ArgumentOutOfRangeException(nameof(Width));

                // Set value
                _width = value;

                // Check for and set always off condition
                if (value == 0)
                {
                    _on = 0;
                    _off = Always;
                    return;
                }

                // Check for and set always on condition
                if (value == Maximum)
                {
                    _on = Always;
                    _off = 0;
                    return;
                }

                // Calculate off according to width
                var offValue = _on + value;
                if (offValue > Maximum)
                    offValue -= Maximum;    // OFF > 1 cycle = pulse across 2 cycles
                _off = offValue;
            }
        }
        private int _width;

        #endregion

        #region Public Methods

        /// <summary>
        /// Converts all values to a byte array which can be written to the channel.
        /// </summary>
        public byte[] ToByteArray()
        {
            var onBytes = BitConverter.GetBytes(_on);
            var offBytes = BitConverter.GetBytes(_off);
            return new byte[] { onBytes[0], onBytes[1], offBytes[0], offBytes[1] };
        }

        /// <summary>
        /// Converts a byte array into a value of this type.
        /// </summary>
        /// <param name="data">Byte array to convert.</param>
        /// <param name="offset">Offset at which to start conversion.</param>
        public static Pca9685ChannelValue FromByteArray(byte[] data, int offset = 0)
        {
            return new Pca9685ChannelValue(data, offset);
        }

        /// <summary>
        /// Calculates PWM pulse width in ticks from on (rising) and off (falling) delay/tick values.
        /// </summary>
        /// <param name="on">PWM on time.</param>
        /// <param name="off">PWM off time.</param>
        /// <returns>Pulse width in ticks.</returns>
        public static int CalculateWidth(int on, int off)
        {
            // Check for always off condition (takes precedence over always on)
            if (off > Maximum)
                return 0;

            // Check for always on condition
            if (on > Maximum)
                return Maximum;

            // Calculate and return ticks
            var ticks = on <= off
                ? off - on                    // ON <= OFF = pulse within 1 cycle
                : ((Maximum + off) - on);     // ON > OFF = pulse across 2 cycles
            return ticks;
        }

        /// <summary>
        /// Creates a value calculated from <paramref name="width"/> and optional <paramref name="delay"/>.
        /// </summary>
        /// <remarks>
        /// Automatically converts zero width to "always off" and maximum width to "always on".
        /// </remarks>
        /// <param name="width">
        /// Pulse width in clock ticks, between 0 and <see cref="Maximum"/>.
        /// When zero or lower it will be set to <see cref="Always"/> off.
        /// When <see cref="Maximum"/> or greater it will be set to <see cref="Always"/> on.
        /// </param>
        /// <param name="delay">
        /// Optional delay in clock ticks, between 0 and <see cref="Maximum"/>.
        /// When zero or lower it will be set to <see cref="Always"/> off.
        /// When <see cref="Maximum"/> or greater it will be set to <see cref="Always"/> on.
        /// </param>
        /// <returns>Calculated channel value.</returns>
        public static Pca9685ChannelValue FromWidth(int width, int delay = 0)
        {
            // Check for always on condition
            if (width >= Maximum)
                return new Pca9685ChannelValue(Always, 0);

            // Check for always off condition
            if (width == 0)
                return new Pca9685ChannelValue(0, Always);

            // Calculate and return value
            var onValue = delay;
            var offValue = delay + width;
            if (offValue > Maximum)
                offValue -= Maximum;
            return new Pca9685ChannelValue(onValue, offValue);
        }

        /// <summary>
        /// Calculates pulse width in fractions of a millisecond from ticks and frequency.
        /// </summary>
        /// <param name="frequency">Frequency in Hz.</param>
        /// <param name="ticks">Length in clock ticks.</param>
        /// <returns>Milliseconds.</returns>
        public static decimal CalculateWidthMs(int frequency, int ticks)
        {
            // Validate
            if (ticks < 0 || ticks > Maximum) throw new ArgumentOutOfRangeException(nameof(ticks));
            if (frequency <= 0) throw new ArgumentOutOfRangeException(nameof(frequency));

            // Calculate and return value
            return (1000m / (4096m * frequency)) * ticks;
        }

        /// <summary>
        /// Calculates the pulse width in milliseconds based on a given frequency in Hz.
        /// </summary>
        /// <param name="frequency">Frequency in Hz for which to calculate the width.</param>
        /// <returns>Pulse width in milliseconds.</returns>
        /// <remarks>
        /// Same as calling <see cref="CalculateWidthMs(int, int)"/> with the current <see cref="Width"/>.
        /// </remarks>
        public decimal ToWidthMs(int frequency)
        {
            return CalculateWidthMs(frequency, _width);
        }

        /// <summary>
        /// Creates a value calculated from <paramref name="width"/> (given a <paramref name="frequency"/>)
        /// and optional <paramref name="delay"/>.
        /// </summary>
        /// <param name="width">
        /// Pulse width in milliseconds. Cannot be greater than one clock interval (1000 / frequency).
        /// </param>
        /// <param name="frequency">Clock frequency of the <see cref="Pca9685Device"/>.</param>
        /// <param name="delay">Optional delay in milliseconds. Cannot be greater than one clock interval (1000 / frequency).</param>
        /// <returns>Value with on and off properties calculated.</returns>
        public static Pca9685ChannelValue FromWidthMs(decimal width, int frequency, int delay)
        {
            // Validate
            if (width < 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (frequency < 1) throw new ArgumentOutOfRangeException(nameof(frequency));

            // Calculate width according to current frequency
            var widthTicks = Convert.ToUInt16(Math.Round((width * 4096m) / (1000m / frequency)));
            var delayTicks = Convert.ToUInt16(Math.Round((delay * 4096m) / (1000m / frequency)));

            // Call overloaded method
            return FromWidth(widthTicks, delayTicks);
        }

        #endregion
    }
}
