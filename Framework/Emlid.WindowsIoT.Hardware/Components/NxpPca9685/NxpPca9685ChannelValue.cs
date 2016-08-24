using System;

namespace Emlid.WindowsIot.Hardware.Components.NxpPca9685
{
    /// <summary>
    /// <see cref="NxpPca9685Device"/> channel value.
    /// </summary>
    public class NxpPca9685ChannelValue
    {
        #region Constants

        /// <summary>
        /// Maximum value (4095).
        /// </summary>
        public const int Maximum = 0x0fff;

        /// <summary>
        /// This value, one higher than <see cref="Maximum"/>, causes the the channel to be always on or off.
        /// </summary>
        public const int Always = 0x1000;

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates an instance with the specified values
        /// </summary>
        public NxpPca9685ChannelValue(int on = 0, int off = 0)
        {
            // Validate
            if (on < 0 || on > Always) throw new ArgumentOutOfRangeException(nameof(on));
            if (off < 0 || off > Always) throw new ArgumentOutOfRangeException(nameof(off));

            // Initialize values
            _on = on;
            _off = off;
            _length = CalculateLength(On, Off);
        }

        /// <summary>
        /// Creates an instance with the values from a byte array.
        /// </summary>
        public NxpPca9685ChannelValue(byte[] data, int offset = 0)
        {
            SetBytes(data, offset);
        }

        #endregion

        #region Operators

        /// <summary>
        /// Tests two objects of this type for equality by value.
        /// </summary>
        public static bool operator ==(NxpPca9685ChannelValue item1, NxpPca9685ChannelValue item2)
        {
            if (!ReferenceEquals(item1, null))
                return item1.Equals(item2);

            return ReferenceEquals(item2, null);
        }

        /// <summary>
        /// Tests two objects of this type for inequality by value.
        /// </summary>
        public static bool operator !=(NxpPca9685ChannelValue item1, NxpPca9685ChannelValue item2)
        {
            if (!ReferenceEquals(item1, null))
                return !item1.Equals(item2);

            return !ReferenceEquals(item2, null);
        }

        /// <summary>
        /// Compares this object with another by value.
        /// </summary>
        public override bool Equals(object obj)
        {
            // Compare nullability and type
            var other = obj as NxpPca9685ChannelValue;
            if (ReferenceEquals(other, null))
                return false;

            // Compare values
            return
                other._on == _on &&
                other._off == _off &&
                other._length == _length;
        }

        /// <summary>
        /// Returns a hashcode based on the current value of this object.
        /// </summary>
        public override int GetHashCode()
        {
            return
                _on.GetHashCode() ^
                _off.GetHashCode() ^
                _length.GetHashCode();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 12-bit rising "on" time in the range 0-<see cref="Maximum"/> or <see cref="Always"/> for always on mode.
        /// </summary>
        /// <remarks>
        /// When greater than <see cref="Off"/> the pulse spans two clock cycles.
        /// Set via <see cref="Length"/> to automatically set <see cref="Always"/> with a length of <see cref="Maximum"/>.
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
                if (value < 0 || value > Always) throw new ArgumentNullException(nameof(Off));

                // Set value
                _on = value;

                // Fire event
                DoChanged();
            }
        }
        private int _on;

        /// <summary>
        /// 12-bit falling "off" time in the range 0-<see cref="Maximum"/> or <see cref="Always"/> for always off mode.
        /// </summary>
        /// <remarks>
        /// When less than <see cref="On"/> the pulse spans two clock cycles.
        /// Set via <see cref="Length"/> to automatically set <see cref="Always"/> with a length of zero.
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
                if (value < 0 || value > Always) throw new ArgumentNullException(nameof(Off));

                // Set value
                _off = value;

                // Fire event
                DoChanged();
            }
        }
        private int _off;

        /// <summary>
        /// Length of the pulse, the difference between <see cref="On"/> and <see cref="Off"/>.
        /// </summary>
        public int Length
        {
            get
            {
                // Return cached value
                return _length;
            }
            set
            {
                // Do nothing when same
                if (_length == value)
                    return;

                // Validate
                if (value < 0 || value > Maximum) throw new ArgumentOutOfRangeException(nameof(Length));

                // Set value
                _length = value;

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

                // Calculate off according to length
                var offValue = _on + value;
                if (offValue > Maximum)
                    offValue -= Maximum;
                _off = offValue;

                // Fire event
                DoChanged();
            }
        }
        private int _length;

        #endregion

        #region Methods

        /// <summary>
        /// Calculates PWM length from on (rising) and off (falling) delay/tick values.
        /// </summary>
        public static int CalculateLength(int on, int off)
        {
            // Check for always off condition (takes precedence over always on)
            if (off > Maximum)
                return 0;

            // Check for always on condition
            if (on > Maximum)
                return Maximum;

            // Calculate and return pulse length
            var length = on <= off
                ? off - on                    // ON <= OFF = length within 1 cycle
                : ((Maximum + off) - on);     // ON > OFF = length across 2 cycles
            return length;
        }

        /// <summary>
        /// Calculates PWM milliseconds from a length and frequency.
        /// </summary>
        /// <param name="frequency">Frequency in Hz.</param>
        /// <param name="length">Length in clock ticks.</param>
        /// <returns>Milliseconds.</returns>
        public static float CalculateMilliseconds(float frequency, int length)
        {
            // Validate
            if (length < 0 || length > Maximum) throw new ArgumentOutOfRangeException(nameof(length));
            if (frequency <= 0) throw new ArgumentOutOfRangeException(nameof(frequency));

            // Calculate and return value
            return (1000 / 4096f / frequency) * length;
        }

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
        /// Sets the value from a byte array.
        /// </summary>
        /// <param name="data">Data buffer to read from.</param>
        /// <param name="offset">Optional offset to read from.</param>
        public void SetBytes(byte[] data, int offset = 0)
        {
            // Validate
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (data.Length < offset + (sizeof(ushort) * 2)) throw new ArgumentOutOfRangeException(nameof(data));

            // Extract values from byte array
            var on = BitConverter.ToUInt16(data, offset);
            var off = BitConverter.ToUInt16(data, offset + sizeof(ushort));

            // Do nothing when not changed
            if (_on == on && _off == off)
                return;

            // Set values
            _on = on;
            _off = off;

            // Calculate length
            _length = CalculateLength(_on, _off);

            // Fire event
            DoChanged();
        }

        /// <summary>
        /// Converts a byte array into a value of this type.
        /// </summary>
        public static NxpPca9685ChannelValue FromByteArray(byte[] data, int offset = 0)
        {
            return new NxpPca9685ChannelValue(data, offset);
        }

        /// <summary>
        /// Creates a value calculated from <paramref name="length"/> and optional <paramref name="delay"/>.
        /// </summary>
        /// <remarks>
        /// Automatically converts zero length to "always off" and maximum length to "always on".
        /// </remarks>
        /// <param name="length">Pulse length in clock ticks, between 0 and <see cref="Maximum"/>.</param>
        /// <param name="delay">Optional delay in clock ticks, beteween 0 and <see cref="Maximum"/>.</param>
        /// <returns>Calculated channel value.</returns>
        public static NxpPca9685ChannelValue FromLength(int length, int delay = 0)
        {
            // Validate
            if (length < 0 || length > Maximum) throw new ArgumentOutOfRangeException(nameof(length));
            if (delay < 0 || delay > Maximum) throw new ArgumentOutOfRangeException(nameof(delay));

            // Check for always on condition
            if (length >= Maximum)
                return new NxpPca9685ChannelValue(Always, 0);

            // Check for always off condition
            if (length == 0)
                return new NxpPca9685ChannelValue(0, Always);

            // Calculate and return value
            var onValue = delay;
            var offValue = delay + length;
            if (offValue > Maximum)
                offValue -= Maximum;
            return new NxpPca9685ChannelValue(onValue, offValue);
        }

        /// <summary>
        /// Creates a value calculated from <paramref name="length"/> (given a <paramref name="frequency"/>)
        /// and optional <paramref name="delay"/>.
        /// </summary>
        /// <param name="length">
        /// Pulse length in milliseconds. Cannot be greater than one clock interval (1000 / frequency).
        /// </param>
        /// <param name="frequency">Clock frequency of the <see cref="NxpPca9685Device"/>.</param>
        /// <param name="delay">Optional delay in milliseconds. Cannot be greater than one clock interval (1000 / frequency).</param>
        /// <returns>Value with on and off values caculated.</returns>
        public static NxpPca9685ChannelValue FromMilliseconds(float length, float frequency, float delay)
        {
            // Validate
            if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));
            if (frequency < 1) throw new ArgumentOutOfRangeException(nameof(frequency));

            // Check for always off condition
            if (length == 0)
                return new NxpPca9685ChannelValue(0, Always);

            // Calculate length according to current frequency
            var lengthTicks = Convert.ToUInt16(Math.Round((length * 4096f) / (1000f / frequency)));
            var delayTicks = Convert.ToUInt16(Math.Round((delay * 4096f) / (1000f / frequency)));

            // Call overloaded method
            return FromLength(lengthTicks, delayTicks);
        }

        #endregion

        #region Events

        /// <summary>
        /// Fired when any of the values change.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Fires the <see cref="Changed"/> event.
        /// </summary>
        protected void DoChanged()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
