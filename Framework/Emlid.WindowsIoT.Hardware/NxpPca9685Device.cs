using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using Windows.Devices.I2c;

namespace Emlid.WindowsIot.Hardware
{
    /// <summary>
    /// PCA9685 hardware device.
    /// </summary>
    /// <remarks>
    /// The PCA9685 is a 16 channel, 12-bit PWM LED controller with I2C bus connection.
    /// <para>
    /// This class enables direct control of the device intended for internal use.
    /// If exposed to other users, it should wrapped by a thread safe class which
    /// coordinates access and deals with object lifetime (dispose).
    /// See http://www.nxp.com/documents/data_sheet/PCA9685.pdf for more information.
    /// </para>
    /// </remarks>
    public class NxpPca9685Device : IDisposable
    {
        #region Constants

        /// <summary>
        /// Offset of the first channel register, <see cref="NxpPca9685Register.Channel0OnLow"/>.
        /// </summary>
        public const byte ChannelStartAddress = 0x06;

        /// <summary>
        /// Size of the channel register groups, added to <see cref="ChannelStartAddress"/> to calculate the address of specific channels.
        /// </summary>
        public const byte ChannelSize = 4;

        /// <summary>
        /// Total number of channels.
        /// </summary>
        public const int ChannelCount = 16;

        /// <summary>
        /// Internal clock speed in Hz.
        /// </summary>
        public const int InternalClockSpeed = 25000000;

        /// <summary>
        /// Maximum clock speed in Hz.
        /// </summary>
        public const int ClockSpeedMaximum = 50000000;

        /// <summary>
        /// Minimum value of the <see cref="NxpPca9685Register.PreScale"/> register.
        /// </summary>
        public const byte PreScaleMinimum = 0x03;

        /// <summary>
        /// Maximum value of the <see cref="NxpPca9685Register.PreScale"/> register.
        /// </summary>
        public const byte PreScaleMaximum = 0xff;

        /// <summary>
        /// Default value of the <see cref="NxpPca9685Register.PreScale"/> register.
        /// </summary>
        public const byte PreScaleDefault = 0x30;

        /// <summary>
        /// Time to wait in milliseconds after switching modes, stopping or starting the oscillator.
        /// </summary>
        public const int ModeSwitchDelay = 1;

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates an instance at the specified I2C <paramref name="address"/> with custom settings.
        /// </summary>
        /// <param name="deviceId">
        /// Device ID of the I2C master.
        /// </param>
        /// <param name="address">
        /// I2C slave address of the chip.
        /// This is a physical property, not a software option.
        /// </param>
        /// <param name="clockSpeed">
        /// Optional external clock speed in Hz. Otherwise the <see cref="InternalClockSpeed"/> is used.
        /// This is a physical property, not a software option.
        /// </param>
        /// <param name="fast">
        /// Set true for I2C <see cref="I2cBusSpeed.FastMode"/> or false for <see cref="I2cBusSpeed.StandardMode"/>.
        /// </param>
        /// <param name="exclusive">
        /// Set true for I2C <see cref="I2cSharingMode.Exclusive"/> or false for <see cref="I2cSharingMode.Shared"/>.
        /// </param>
        public NxpPca9685Device(string deviceId, int address, int? clockSpeed, bool fast, bool exclusive)
        {
            // Validate
            if (clockSpeed.HasValue && (clockSpeed == 0 || clockSpeed.Value > ClockSpeedMaximum))
                throw new ArgumentOutOfRangeException(nameof(clockSpeed));

            // Initialize members
            ClockIsExternal = clockSpeed.HasValue;
            ClockSpeed = clockSpeed ?? InternalClockSpeed;
            FrequencyDefault = CalculateFrequency(PreScaleDefault, ClockSpeed);
            FrequencyMinimum = CalculateFrequency(PreScaleMaximum, ClockSpeed); // Inverse relationship (max = min)
            FrequencyMaximum = CalculateFrequency(PreScaleMinimum, ClockSpeed); // Inverse relationship (min = max)
            _channels = new Collection<NxpPca9685Channel>();
            Channels = new ReadOnlyCollection<NxpPca9685Channel>(_channels);
            for (var index = 0; index < ChannelCount; index++)
                _channels.Add(new NxpPca9685Channel(index));

            // Initialize hardware
            Hardware = Connect(deviceId, address, fast, exclusive);
            Id = Hardware.DeviceId;

            // Read current values
            ReadAll();

            // Hook change events
            foreach (var channel in Channels)
                channel.ValueChanged += OnChannelChanged;
        }

        #region IDisposable

        /// <summary>
        /// Indicates this instance has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Frees resources owned by this instance.
        /// </summary>
        /// <param name="disposing">
        /// True when called via <see cref="Dispose()"/>, false when called from the finalizer.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            // Do nothing when already disposed
            if (IsDisposed) return;

            // Dispose
            try
            {
                // Dispose managed resource during dispose
                if (disposing)
                {
                    Hardware.Dispose();
                }
            }
            finally
            {
                // Flag disposed
                IsDisposed = true;
            }
        }

        /// <summary>
        /// Finalizer which calls <see cref="Dispose(bool)"/> with false when it has not been disabled
        /// by a proactive call to <see cref="Dispose()"/>.
        /// </summary>
        ~NxpPca9685Device()
        {
            // Partial dispose
            Dispose(false);
        }

        /// <summary>
        /// Proactively frees resources owned by this instance.
        /// </summary>
        public void Dispose()
        {
            try
            {
                // Full managed dispose
                Dispose(true);
            }
            finally
            {
                // Suppress finalizer (we already cleaned-up)
                GC.SuppressFinalize(this);
            }
        }

        #endregion

        #endregion

        #region Protected Properties

        /// <summary>
        /// I2C device and connection indicator. Set during the first call to <see cref="Connect"/>.
        /// </summary>
        [CLSCompliant(false)]
        protected I2cDevice Hardware { get; private set; }

        #endregion

        #region Public Properties

        /// <summary>
        /// Channels and their values (also settable).
        /// </summary>
        public ReadOnlyCollection<NxpPca9685Channel> Channels { get; private set; }
        private Collection<NxpPca9685Channel> _channels;

        /// <summary>
        /// Indicates the PWM clock is controlled externally.
        /// </summary>
        public bool ClockIsExternal { get; private set; }

        /// <summary>
        /// PWM clock speed, either internal or external.
        /// </summary>
        public int ClockSpeed { get; private set; }

        /// <summary>
        /// Frequency in Hz.
        /// </summary>
        public float Frequency { get; private set; }

        /// <summary>
        /// Frequency when the hardware <see cref="PreScaleDefault"/> is set.
        /// </summary>
        public float FrequencyDefault { get; private set; }

        /// <summary>
        /// Minimum frequency based on <see cref="ClockSpeed"/> and <see cref="PreScaleMaximum"/> (inverse relation).
        /// </summary>
        public float FrequencyMinimum { get; private set; }

        /// <summary>
        /// Maximum frequency based on <see cref="ClockSpeed"/> and <see cref="PreScaleMinimum"/> (inverse relation).
        /// </summary>
        public float FrequencyMaximum { get; private set; }

        /// <summary>
        /// Device ID.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Last known mode 1 register bits.
        /// </summary>
        public NxpPca9685Mode1Bits Mode1Register { get; private set; }

        /// <summary>
        /// Last known mode 2 register bits.
        /// </summary>
        public NxpPca9685Mode2Bits Mode2Register { get; private set; }

        /// <summary>
        /// Minimum PWM length in milliseconds, based on the current frequency.
        /// </summary>
        public float PwmMsMinimum { get; private set; }

        /// <summary>
        /// Maximum PWM length in milliseconds, based on the current frequency.
        /// </summary>
        public float PwmMsMaximum { get; private set; }

        #endregion

        #region Public Methods

        #region General

        /// <summary>
        /// Reads all values from the device and updates properties.
        /// </summary>
        public virtual void ReadAll()
        {
            ReadMode1();
            ReadMode2();
            ReadFrequency();
            ReadAllChannels();
        }

        #endregion

        #region Mode

        /// <summary>
        /// Reads the current value of the <see cref="NxpPca9685Register.Mode1"/> register.
        /// </summary>
        /// <returns>Bit flags corresponding to the actual mode byte.</returns>
        public NxpPca9685Mode1Bits ReadMode1()
        {
            // Read register
            var value = (NxpPca9685Mode1Bits)Hardware.ReadByte((byte)NxpPca9685Register.Mode1);

            // Update property
            Mode1Register = value;

            // Return result
            return value;
        }

        /// <summary>
        /// Reads the current value of the <see cref="NxpPca9685Register.Mode2"/> register.
        /// </summary>
        /// <returns>Bit flags corresponding to the actual mode byte.</returns>
        public NxpPca9685Mode2Bits ReadMode2()
        {
            // Read register
            var value = (NxpPca9685Mode2Bits)Hardware.ReadByte((byte)NxpPca9685Register.Mode2);

            // Update property
            Mode2Register = value;

            // Return result
            return value;
        }

        #endregion

        #region Sleep

        /// <summary>
        /// Enters sleep mode.
        /// </summary>
        /// <remarks>
        /// Sets the <see cref="NxpPca9685Register.Mode1"/> register <see cref="NxpPca9685Mode1Bits.Sleep"/> bit
        /// then waits for <see cref="ModeSwitchDelay"/> to allow the oscillator to stop.
        /// </remarks>
        /// <returns>
        /// True when mode was changed, false when already set.
        /// </returns>
        public virtual bool Sleep()
        {
            // Read sleep bit (do nothing when already sleeping)
            var sleeping = Hardware.ReadBit((byte)NxpPca9685Register.Mode1, (byte)NxpPca9685Mode1Bits.Sleep);
            if (sleeping)
                return false;

            // Set sleep bit
            Hardware.WriteBit((byte)NxpPca9685Register.Mode1, (byte)NxpPca9685Mode1Bits.Sleep, true);

            // Wait for completion
            Task.Delay(ModeSwitchDelay).Wait();

            // Return changed
            return true;
        }

        /// <summary>
        /// Leaves sleep mode.
        /// </summary>
        /// <remarks>
        /// Clears the <see cref="NxpPca9685Register.Mode1"/> register <see cref="NxpPca9685Mode1Bits.Sleep"/> bit
        /// then waits for <see cref="ModeSwitchDelay"/> to allow the oscillator to start.
        /// </remarks>
        /// <returns>
        /// True when mode was changed, false when not sleeping.
        /// </returns>
        public virtual bool Wake()
        {
            // Read sleep bit (do nothing when already sleeping)
            var sleeping = Hardware.ReadBit((byte)NxpPca9685Register.Mode1, (byte)NxpPca9685Mode1Bits.Sleep);
            if (!sleeping)
                return false;

            // Clear sleep bit
            Hardware.WriteBit((byte)NxpPca9685Register.Mode1, (byte)NxpPca9685Mode1Bits.Sleep, false);

            // Wait for completion
            Task.Delay(ModeSwitchDelay).Wait();

            // Return changed
            return true;
        }

        #endregion

        #region Restart

        /// <summary>
        /// Restarts the device with default options, then updates all properties.
        /// </summary>
        public virtual void Restart()
        {
            // Call overloaded method
            Restart(NxpPca9685Mode1Bits.None);
        }

        /// <summary>
        /// Restarts the device with additional options specified, then updates all properties.
        /// </summary>
        /// <param name="options">
        /// Optional mode 1 parameters to add to the final restart sequence. A logical OR is applied to this value and
        /// the standard <see cref="NxpPca9685Mode1Bits.Restart"/>, <see cref="NxpPca9685Mode1Bits.ExternalClock"/> and
        /// <see cref="NxpPca9685Mode1Bits.AutoIncrement"/> bits.
        /// </param>
        public virtual void Restart(NxpPca9685Mode1Bits options)
        {
            // Configure according to external clock presence
            var externalClock = ClockIsExternal;
            var delay = TimeSpan.FromTicks(Convert.ToInt64(Math.Round(TimeSpan.TicksPerMillisecond * 0.5)));

            // Send I2C restart sequence...

            // Write first sleep
            var sleep = (byte)NxpPca9685Mode1Bits.Sleep;
            Hardware.WriteByte((byte)NxpPca9685Register.Mode1, sleep);

            // Write sleep again with external clock option (when present)
            if (externalClock)
            {
                sleep |= (byte)(NxpPca9685Mode1Bits.ExternalClock);
                Hardware.WriteByte((byte)NxpPca9685Register.Mode1, sleep);
            }
            else
            {
                // At least 500 nanoseconds sleep required using internal clock
                Task.Delay(delay).Wait();
            }

            // Write reset with external clock option and any additional options
            var restart = (byte)(NxpPca9685Mode1Bits.Restart | NxpPca9685Mode1Bits.AutoIncrement);
            if (externalClock)
                restart |= (byte)NxpPca9685Mode1Bits.ExternalClock;
            restart |= (byte)options;
            Hardware.WriteByte((byte)NxpPca9685Register.Mode1, restart);

            // At least 500 nanoseconds delay to allow oscillator to start
            Task.Delay(delay).Wait();

            // Update all properties
            ReadAll();
        }

        #endregion

        #region Frequency

        /// <summary>
        /// Calculates the effective frequency from a <see cref="NxpPca9685Register.PreScale"/> value and clock speed.
        /// </summary>
        /// <param name="preScale">Pre-scale value from which to calculate frequency.</param>
        /// <param name="clockSpeed">Clock speed with with to calcualte the frequency.</param>
        /// <returns>Calculated frequency.</returns>
        public static float CalculateFrequency(byte preScale, int clockSpeed)
        {
            return clockSpeed / 4096f / (preScale + 1);
        }

        /// <summary>
        /// Calculates the <see cref="NxpPca9685Register.PreScale"/> value from a desired frequency and clock speed.
        /// </summary>
        /// <remarks>
        /// Due to scaling only certain frequencies are possible. To get the resulting frequency from the desired
        /// frequency it is necessary to re-calcualte the effective frequency back from the pre-scale,
        /// i.e. call <see cref="CalculateFrequency"/>.
        /// </remarks>
        /// <param name="frequency">
        /// Desired frequency.
        /// Must be between <see cref="FrequencyMinimum"/> and <see cref="FrequencyMaximum"/> to get a valid result.
        /// </param>
        /// <param name="clockSpeed"></param>
        /// <returns>Calculated pre-scale value.</returns>
        /// <exception cref="OverflowException">
        /// Thrown when an invalid frequency is used which causes the result to overflow a byte value.
        /// </exception>
        public static byte CalculatePreScale(float frequency, int clockSpeed)
        {
            return Convert.ToByte(Math.Round(clockSpeed / 4096f / frequency) - 1);
        }

        /// <summary>
        /// Reads the pre-scale register and calculates the <see cref="Frequency"/> (and related properties)
        /// based on <see cref="ClockSpeed"/>.
        /// </summary>
        /// <returns>
        /// Frequency in Hz. Related properties are also udpated.
        /// </returns>
        public float ReadFrequency()
        {
            // Read pre-scale register
            var preScale = Hardware.ReadByte((byte)NxpPca9685Register.PreScale);

            // Calculate frequency
            var frequency = CalculateFrequency(preScale, ClockSpeed);

            // Update related properties
            Frequency = frequency;
            PwmMsMinimum = NxpPca9685ChannelValue.CalculateMilliseconds(Frequency, 0);
            PwmMsMaximum = NxpPca9685ChannelValue.CalculateMilliseconds(Frequency, NxpPca9685ChannelValue.Maximum);

            // Return result
            return frequency;
        }

        /// <summary>
        /// Calculates the pre-scale value from the frequency (according to <see cref="ClockSpeed"/>) 
        /// then writes that register, then calls <see cref="ReadFrequency"/> to update properties.
        /// Note the actual frequency may differ to the requested frequency due to clock scale (rounding).
        /// </summary>
        /// <remarks>
        /// The pre-scale can only be set during sleep mode. This method enters <see cref="Sleep"/> if necessary,
        /// then only if the device was awake before, calls <see cref="Wake"/> afterwards. It's important not to
        /// start output unexpectedly to avoid damage, i.e. if the device was sleeping before, the frequency is
        /// changed without starting the oscillator.
        /// </remarks>
        /// <param name="frequency">Frequency to convert in Hz.</param>
        /// <returns>Effective frequency in Hz, read-back and re-caculated after setting the desired frequency.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="frequency"/> is less than <see cref="FrequencyMinimum"/> or greater than
        /// <see cref="FrequencyMaximum"/>.
        /// </exception>
        /// <returns>
        /// Frequency in Hz. Related properties are also udpated.
        /// </returns>
        public float WriteFrequency(float frequency)
        {
            // Validate
            if (frequency < FrequencyMinimum || frequency > FrequencyMaximum)
                throw new ArgumentOutOfRangeException(nameof(frequency));

            // Calculate pre-scale
            var preScale = CalculatePreScale(frequency, ClockSpeed);

            // Enter sleep mode and record wake status
            var wasAwake = Sleep();

            // Write pre-scale
            Hardware.WriteByte((byte)NxpPca9685Register.PreScale, preScale);

            // Read result
            var actual = ReadFrequency();

            // Wake-up if previously running
            if (wasAwake)
                Wake();

            // Return actual frequency
            return actual;
        }

        #endregion

        #region Channels

        /// <summary>
        /// Gets the register address of a channel by index.
        /// </summary>
        /// <param name="index">Zero based channel number (0-15) or 16 for the "all call" channel.</param>
        /// <returns>Channel address.</returns>
        public static byte GetChannelAddress(int index)
        {
            // Validate
            if (index < 0 || index > ChannelCount) throw new ArgumentOutOfRangeException(nameof(index));

            // Calculate and return address
            return (byte)(ChannelStartAddress + (ChannelSize * index));
        }

        /// <summary>
        /// Clears all channels cleanly, then updates all <see cref="Channels"/>.
        /// </summary>
        /// <remarks>
        /// To "cleanly" clear the channels, it is necessary to first ensure they are not disabled,
        /// set them to zero, then disable them. Otherwise the ON value and the low OFF value
        /// remain because writes are ignored when the OFF channel bit 12 is already set.
        /// </remarks>
        public virtual void Clear()
        {
            // Enable all channels
            Hardware.WriteByte((byte)NxpPca9685Register.AllChannelsOffHigh, 0x00);

            // Zero all channels
            Hardware.WriteBytes((byte)NxpPca9685Register.AllChannelsOnLow, new byte[] { 0x00, 0x00, 0x00, 0x00 });

            // Disable all channels
            Hardware.WriteByte((byte)NxpPca9685Register.AllChannelsOffHigh, 0x10);

            // Update channels
            ReadAllChannels();
        }

        /// <summary>
        /// Reads a whole channel value (on and off), and updates it in <see cref="Channels"/>.
        /// </summary>
        /// <param name="index">Zero based channel number (0-15).</param>
        /// <returns>Channel value</returns>
        public NxpPca9685Channel ReadChannel(int index)
        {
            // Validate
            if (index < 0 | index >= ChannelCount) throw new ArgumentOutOfRangeException(nameof(index));

            // Calculate register address
            var register = GetChannelAddress(index);

            // Read value
            var bytes = Hardware.ReadBytes(register, sizeof(ushort) * 2);

            // Update channel property and return result
            var channel = _channels[index];
            channel.Value.SetBytes(bytes);
            return channel;
        }

        /// <summary>
        /// Reads the PWM "on" (rising) value of a channel, and updates it in <see cref="Channels"/>.
        /// </summary>
        /// <param name="index">Zero based channel number (0-15).</param>
        /// <returns>Channel value.</returns>
        public int ReadChannelOn(int index)
        {
            // Validate
            if (index < 0 | index >= ChannelCount) throw new ArgumentOutOfRangeException(nameof(index));

            // Calculate register address
            var register = GetChannelAddress(index);

            // Read and convert value
            var bytes = Hardware.ReadBytes(register, sizeof(ushort));
            var value = BitConverter.ToUInt16(bytes, 0);

            // Update channel property and return result
            var channel = _channels[index];
            channel.Value.On = value;
            return value;
        }

        /// <summary>
        /// Reads the PWM "off" (falling) value of a channel, and updates it in <see cref="Channels"/>.
        /// </summary>
        /// <param name="index">Zero based channel number (0-15).</param>
        /// <returns>Channel value.</returns>
        public int ReadChannelOff(int index)
        {
            // Validate
            if (index < 0 | index >= ChannelCount) throw new ArgumentOutOfRangeException(nameof(index));

            // Calculate register address of second word value
            var register = (byte)(GetChannelAddress(index) + sizeof(ushort));

            // Read and convert value
            var bytes = Hardware.ReadBytes(register, sizeof(ushort));
            var value = BitConverter.ToUInt16(bytes, 0);

            // Update channel property and return result
            var channel = _channels[index];
            channel.Value.Off = value;
            return value;
        }

        /// <summary>
        /// Calculates the "on" and "off" values of a channel from length (and optional delay),
        /// then writes them together, and updates it in <see cref="Channels"/>.
        /// </summary>
        /// <param name="index">Zero based channel number (0-15) or 16 for the "all call" channel.</param>
        /// <param name="length">Pulse length in clock ticks.</param>
        /// <param name="delay">Optional delay in clock ticks.</param>
        /// <returns>
        /// Updated channel value or null when all channels were updated.
        /// </returns>
        public NxpPca9685Channel WriteChannelLength(int index, int length, int delay = 0)
        {
            // Call overloaded method
            return WriteChannel(index, NxpPca9685ChannelValue.FromLength(length, delay));
        }

        /// <summary>
        /// Calculates the "on" and "off" values of a channel from milliseconds (and optional delay),
        /// then writes them together, and updates it in <see cref="Channels"/>.
        /// </summary>
        /// <param name="index">Zero based channel number (0-15) or 16 for the "all call" channel.</param>
        /// <param name="length">
        /// Pulse length in milliseconds. Cannot be greater than one clock interval (1000 / frequency).
        /// </param>
        /// <param name="delay">Optional delay in milliseconds. Cannot be greater than one clock interval (1000 / frequency).</param>
        /// <returns>
        /// Updated channel value or null when all channels were updated.
        /// </returns>
        public NxpPca9685Channel WriteChannelMilliseconds(int index, float length, float delay = 0)
        {
            // Read current frequency
            var frequency = ReadFrequency();

            // Call overloaded method
            return WriteChannel(index, NxpPca9685ChannelValue.FromMilliseconds(length, frequency, delay));
        }

        /// <summary>
        /// Writes the "on" and "off" values of a channel together, and updates it in <see cref="Channels"/>.
        /// </summary>
        /// <param name="index">Zero based channel number (0-15) or 16 for the "all call" channel.</param>
        /// <param name="value"><see cref="NxpPca9685ChannelValue"/> to write.</param>
        /// <returns>
        /// Updated channel value or null when all channels were updated.
        /// </returns>
        public NxpPca9685Channel WriteChannel(int index, NxpPca9685ChannelValue value)
        {
            // Validate
            if (index < 0 | index > ChannelCount) throw new ArgumentOutOfRangeException(nameof(index));

            // Validate
            if (value.On > NxpPca9685ChannelValue.Maximum + 1 ||
            value.Off > NxpPca9685ChannelValue.Maximum + 1)
                throw new ArgumentOutOfRangeException(nameof(value));

            // Calculate register address
            var register = GetChannelAddress(index);

            // Convert and write value
            var bytes = value.ToByteArray();
            Hardware.WriteBytes(register, bytes);

            // Read and return result
            if (index < ChannelCount)
                return ReadChannel(index);
            ReadAllChannels();
            return null;
        }

        /// <summary>
        /// Writes the PWM "on" (rising) value of a channel.
        /// </summary>
        /// <param name="index">Zero based channel number (0-15) or 16 for the "all call" channel.</param>
        /// <param name="value">12-bit channel value in the range 0-<see cref="NxpPca9685ChannelValue.Maximum"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="value"/> is greater than <see cref="NxpPca9685ChannelValue.Maximum"/>.</exception>
        public void WriteChannelOn(int index, int value)
        {
            // Validate
            if (value > NxpPca9685ChannelValue.Maximum) throw new ArgumentOutOfRangeException(nameof(value));

            // Calculate register address
            var register = GetChannelAddress(index);

            // Convert and write value
            var bytes = BitConverter.GetBytes(value);
            Hardware.WriteBytes(register, bytes);
        }

        /// <summary>
        /// Writes the PWM "off" (falling) value of a channel.
        /// </summary>
        /// <param name="index">Zero based channel number (0-15) or 16 for the "all call" channel.</param>
        /// <param name="value">12-bit channel value in the range 0-<see cref="NxpPca9685ChannelValue.Maximum"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="value"/> is greater then <see cref="NxpPca9685ChannelValue.Maximum"/>.</exception>
        public void WriteChannelOff(int index, int value)
        {
            // Validate
            if (value > NxpPca9685ChannelValue.Maximum) throw new ArgumentOutOfRangeException(nameof(value));

            // Calculate register address of second word value
            var register = (byte)(GetChannelAddress(index) + sizeof(ushort));

            // Convert and write value
            var bytes = BitConverter.GetBytes(value);
            Hardware.WriteBytes(register, bytes);
        }

        /// <summary>
        /// Reads all channels and updates <see cref="Channels"/>.
        /// </summary>
        protected virtual void ReadAllChannels()
        {
            // Read all channels as one block of data
            var data = Hardware.ReadBytes(ChannelStartAddress, ChannelSize * ChannelCount);

            // Update properties
            for (var index = 0; index < ChannelCount; index++)
                _channels[index].Value.SetBytes(data, ChannelSize * index);
        }

        #endregion

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates an I2C conection to the chip.
        /// </summary>
        /// <param name="deviceId">I2C master device ID.</param>
        /// <param name="address">I2C slave address of the chip.</param>
        /// <param name="fast">
        /// Set true for I2C <see cref="I2cBusSpeed.FastMode"/> or false for <see cref="I2cBusSpeed.StandardMode"/>.
        /// </param>
        /// <param name="exclusive">
        /// Set true for I2C <see cref="I2cSharingMode.Exclusive"/> or false for <see cref="I2cSharingMode.Shared"/>.
        /// </param>
        /// <returns>True when initialized, false when already initialized (but no error).</returns>
        /// <exception cref="Exception">Thrown when initialization failed.</exception>
        [CLSCompliant(false)]
        protected static I2cDevice Connect(string deviceId, int address, bool fast, bool exclusive)
        {
            // Connect to device
            var settings = new I2cConnectionSettings(address)
            {
                BusSpeed = fast ? I2cBusSpeed.FastMode : I2cBusSpeed.StandardMode,
                SharingMode = exclusive ? I2cSharingMode.Exclusive : I2cSharingMode.Shared
            };
            var device = I2cDevice.FromIdAsync(deviceId, settings).AsTask().GetAwaiter().GetResult();
            if (device == null)
            {
                // Initialization error
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                    new Resources.Strings().I2cErrorDeviceCannotOpen, settings.SlaveAddress, deviceId));
            }

            // Return result
            return device;
        }

        #endregion

        #region Events

        /// <summary>
        /// Writes channel values to the device when the <see cref="Channels"/> member changes.
        /// </summary>
        private void OnChannelChanged(object sender, EventArgs arguments)
        {
            var channel = (NxpPca9685Channel)sender;
            WriteChannel(channel.Index, channel.Value);
        }

        #endregion
    }
}
