using Emlid.UniversalWindows;
using Emlid.WindowsIot.HardwarePlus.Buses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Devices.I2c;

namespace Emlid.WindowsIot.Hardware.Components.Pca9685
{
    /// <summary>
    /// PCA9685 PWM LED driver (hardware device), connected via I2C.
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
    public sealed class Pca9685Device : DisposableObject
    {
        #region Constants

        /// <summary>
        /// 7-bit I2C address of the first PCA9685 on the bus.
        /// </summary>
        public const byte I2cAddress = 0x80 >> 1;

        /// <summary>
        /// 7-bit I2C address of the PCA9685 "all call" address.
        /// </summary>
        public const byte I2cAllCallAddress = 0x70 >> 1;

        /// <summary>
        /// Maximum number of devices (chip number) for this model.
        /// </summary>
        public const int MaximumDevices = 62;

        /// <summary>
        /// Offset of the first channel register, <see cref="Pca9685Register.Channel0OnLow"/>.
        /// </summary>
        public const byte ChannelStartAddress = 0x06;

        /// <summary>
        /// Size of the channel register groups, added to <see cref="ChannelStartAddress"/> to calculate the address of specific channels.
        /// </summary>
        public const byte ChannelSize = sizeof(ushort) * 2;

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
        /// Minimum value of the <see cref="Pca9685Register.Prescale"/> register.
        /// </summary>
        public const byte PrescaleMinimum = 0x03;

        /// <summary>
        /// Maximum value of the <see cref="Pca9685Register.Prescale"/> register.
        /// </summary>
        public const byte PrescaleMaximum = 0xff;

        /// <summary>
        /// Default value of the <see cref="Pca9685Register.Prescale"/> register.
        /// </summary>
        public const byte PrescaleDefault = 0x30;

        /// <summary>
        /// Time to wait in milliseconds after switching modes, stopping or starting the oscillator.
        /// </summary>
        public const int ModeSwitchDelay = 1;

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates an instance using the specified I2C device.
        /// </summary>
        /// <param name="busNumber">I2C bus controller number (zero based).</param>
        /// <param name="chipNumber">Chip number.</param>
        /// <param name="clockSpeed">
        /// Optional external clock speed in Hz. Otherwise the <see cref="InternalClockSpeed"/> is used.
        /// This is a physical property, not a software option.
        /// </param>
        /// <param name="speed">Bus speed.</param>
        /// <param name="sharingMode">Sharing mode.</param>
        [CLSCompliant(false)]
        public Pca9685Device(int busNumber, byte chipNumber, int? clockSpeed,
            I2cBusSpeed speed = I2cBusSpeed.FastMode, I2cSharingMode sharingMode = I2cSharingMode.Exclusive)
        {
            // Validate
            if (clockSpeed.HasValue && (clockSpeed == 0 || clockSpeed.Value > ClockSpeedMaximum))
                throw new ArgumentOutOfRangeException(nameof(clockSpeed));

            // Get address
            var address = GetI2cAddress(chipNumber);

            // Connect to hardware
            _hardware = I2cExtensions.Connect(busNumber, address, speed, sharingMode);

            // Initialize configuration
            ClockIsExternal = clockSpeed.HasValue;
            ClockSpeed = clockSpeed ?? InternalClockSpeed;
            FrequencyDefault = CalculateFrequency(PrescaleDefault, ClockSpeed);
            FrequencyMinimum = CalculateFrequency(PrescaleMaximum, ClockSpeed); // Inverse relationship (max = min)
            FrequencyMaximum = CalculateFrequency(PrescaleMinimum, ClockSpeed); // Inverse relationship (min = max)

            // Build channels
            _channels = new Collection<Pca9685ChannelValue>();
            Channels = new ReadOnlyCollection<Pca9685ChannelValue>(_channels);
            for (var index = 0; index < ChannelCount; index++)
                _channels.Add(new Pca9685ChannelValue(index));

            // Set "all call" address
            I2cExtensions.WriteJoinByte(_hardware, (byte)Pca9685Register.AllCall, I2cAllCallAddress);

            // Enable auto-increment and "all call"
            I2cExtensions.WriteReadWriteBit(_hardware, (byte)Pca9685Register.Mode1,
                (byte)(Pca9685Mode1Bits.AutoIncrement | Pca9685Mode1Bits.AllCall), true);

            // Read current values and update properties
            ReadAll();
        }

        #region IDisposable

        /// <summary>
        /// Frees resources owned by this instance.
        /// </summary>
        /// <param name="disposing">
        /// True when called via <see cref="IDisposable.Dispose"/>, false when called via finalizer.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            // Only managed resources to dispose
            if (!disposing)
                return;

            // Close device
            _hardware?.Dispose();
        }

        #endregion

        #endregion

        #region Private Fields

        /// <summary>
        /// I2C device.
        /// </summary>
        private I2cDevice _hardware;

        #endregion

        #region Public Properties

        /// <summary>
        /// Channels and their values (also settable).
        /// </summary>
        public ReadOnlyCollection<Pca9685ChannelValue> Channels { get; private set; }
        private readonly Collection<Pca9685ChannelValue> _channels;

        /// <summary>
        /// Indicates the PWM clock is controlled externally.
        /// </summary>
        public bool ClockIsExternal { get; private set; }

        /// <summary>
        /// PWM clock speed in Hz, either internal or external.
        /// </summary>
        public int ClockSpeed { get; private set; }

        /// <summary>
        /// Frequency in Hz.
        /// </summary>
        public int Frequency { get; private set; }

        /// <summary>
        /// Frequency when the hardware <see cref="PrescaleDefault"/> is set.
        /// </summary>
        public int FrequencyDefault { get; private set; }

        /// <summary>
        /// Minimum frequency based on <see cref="ClockSpeed"/> and <see cref="PrescaleMaximum"/> (inverse relation).
        /// </summary>
        public int FrequencyMinimum { get; private set; }

        /// <summary>
        /// Maximum frequency based on <see cref="ClockSpeed"/> and <see cref="PrescaleMinimum"/> (inverse relation).
        /// </summary>
        public int FrequencyMaximum { get; private set; }

        /// <summary>
        /// Last known mode 1 register bits.
        /// </summary>
        public Pca9685Mode1Bits Mode1Register { get; private set; }

        /// <summary>
        /// Last known mode 2 register bits.
        /// </summary>
        public Pca9685Mode2Bits Mode2Register { get; private set; }

        /// <summary>
        /// Minimum PWM length in milliseconds, based on the current frequency.
        /// </summary>
        public decimal PwmMsMinimum { get; private set; }

        /// <summary>
        /// Maximum PWM length in milliseconds, based on the current frequency.
        /// </summary>
        public decimal PwmMsMaximum { get; private set; }

        #endregion

        #region Public Methods

        #region General

        /// <summary>
        /// Gets the I2C address for communication with the specified chip number.
        /// </summary>
        /// <param name="chipNumber">
        /// Device (chip) number, from zero to the <see cref="MaximumDevices"/> supported.
        /// </param>
        /// <returns>7-bit I2C address.</returns>
        public static byte GetI2cAddress(byte chipNumber)
        {
            // Validate
            if (chipNumber < 0 || chipNumber > MaximumDevices)
                throw new ArgumentOutOfRangeException(nameof(chipNumber));

            // Calculate and return address
            return (byte)(I2cAddress + chipNumber);
        }

        /// <summary>
        /// Reads all values from the device and updates properties.
        /// </summary>
        public void ReadAll()
        {
            ReadMode1();
            ReadMode2();
            ReadFrequency();
            ReadAllChannels();
        }

        #endregion

        #region Mode

        /// <summary>
        /// Reads the current value of the <see cref="Pca9685Register.Mode1"/> register.
        /// </summary>
        /// <returns>Bit flags corresponding to the actual mode byte.</returns>
        public Pca9685Mode1Bits ReadMode1()
        {
            // Read register
            var value = (Pca9685Mode1Bits)I2cExtensions.WriteReadByte(_hardware, (byte)Pca9685Register.Mode1);

            // Update property
            Mode1Register = value;

            // Return result
            return value;
        }

        /// <summary>
        /// Reads the current value of the <see cref="Pca9685Register.Mode2"/> register.
        /// </summary>
        /// <returns>Bit flags corresponding to the actual mode byte.</returns>
        public Pca9685Mode2Bits ReadMode2()
        {
            // Read register
            var value = (Pca9685Mode2Bits)I2cExtensions.WriteReadByte(_hardware, (byte)Pca9685Register.Mode2);

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
        /// Sets the <see cref="Pca9685Register.Mode1"/> register <see cref="Pca9685Mode1Bits.Sleep"/> bit
        /// then waits for <see cref="ModeSwitchDelay"/> to allow the oscillator to stop.
        /// </remarks>
        /// <returns>
        /// True when mode was changed, false when already set.
        /// </returns>
        public bool Sleep()
        {
            // Read sleep bit (do nothing when already sleeping)
            var sleeping = I2cExtensions.WriteReadBit(_hardware, (byte)Pca9685Register.Mode1, (byte)Pca9685Mode1Bits.Sleep);
            if (sleeping)
                return false;

            // Set sleep bit
            I2cExtensions.WriteReadWriteBit(_hardware, (byte)Pca9685Register.Mode1, (byte)Pca9685Mode1Bits.Sleep, true);

            // Wait for completion
            Task.Delay(ModeSwitchDelay).Wait();

            // Update related properties
            ReadMode1();

            // Return changed
            return true;
        }

        /// <summary>
        /// Leaves sleep mode.
        /// </summary>
        /// <remarks>
        /// Clears the <see cref="Pca9685Register.Mode1"/> register <see cref="Pca9685Mode1Bits.Sleep"/> bit
        /// then waits for <see cref="ModeSwitchDelay"/> to allow the oscillator to start.
        /// </remarks>
        /// <returns>
        /// True when mode was changed, false when not sleeping.
        /// </returns>
        public bool Wake()
        {
            // Read sleep bit (do nothing when already sleeping)
            var sleeping = I2cExtensions.WriteReadBit(_hardware, (byte)Pca9685Register.Mode1, (byte)Pca9685Mode1Bits.Sleep);
            if (!sleeping)
                return false;

            // Clear sleep bit
            I2cExtensions.WriteReadWriteBit(_hardware, (byte)Pca9685Register.Mode1, (byte)Pca9685Mode1Bits.Sleep, false);

            // Wait for completion
            Task.Delay(ModeSwitchDelay).Wait();

            // Update related properties
            ReadMode1();

            // Return changed
            return true;
        }

        #endregion

        #region Restart

        /// <summary>
        /// Restarts the device with default options, then updates all properties.
        /// </summary>
        public void Restart()
        {
            // Call overloaded method
            Restart(Pca9685Mode1Bits.None);
        }

        /// <summary>
        /// Restarts the device with additional options specified, then updates all properties.
        /// </summary>
        /// <param name="options">
        /// Optional mode 1 parameters to add to the final restart sequence. A logical OR is applied to this value and
        /// the standard <see cref="Pca9685Mode1Bits.Restart"/>, <see cref="Pca9685Mode1Bits.ExternalClock"/> and
        /// <see cref="Pca9685Mode1Bits.AutoIncrement"/> bits.
        /// </param>
        public void Restart(Pca9685Mode1Bits options)
        {
            // Configure according to external clock presence
            var externalClock = ClockIsExternal;
            var delay = TimeSpan.FromTicks(Convert.ToInt64(Math.Round(TimeSpan.TicksPerMillisecond * 0.5)));

            // Send I2C restart sequence...

            // Write first sleep
            var sleep = (byte)Pca9685Mode1Bits.Sleep;
            I2cExtensions.WriteJoinByte(_hardware, (byte)Pca9685Register.Mode1, sleep);

            // Write sleep again with external clock option (when present)
            if (externalClock)
            {
                sleep |= (byte)(Pca9685Mode1Bits.ExternalClock);
                I2cExtensions.WriteJoinByte(_hardware, (byte)Pca9685Register.Mode1, sleep);
            }
            else
            {
                // At least 500 nanoseconds sleep required using internal clock
                Task.Delay(delay).Wait();
            }

            // Write reset with external clock option and any additional options
            var restart = (byte)(Pca9685Mode1Bits.Restart | Pca9685Mode1Bits.AutoIncrement);
            if (externalClock)
                restart |= (byte)Pca9685Mode1Bits.ExternalClock;
            restart |= (byte)options;
            I2cExtensions.WriteJoinByte(_hardware, (byte)Pca9685Register.Mode1, restart);

            // At least 500 nanoseconds delay to allow oscillator to start
            Task.Delay(delay).Wait();

            // Update all properties
            ReadAll();
        }

        #endregion

        #region Frequency

        /// <summary>
        /// Calculates the effective frequency from a <see cref="Pca9685Register.Prescale"/> value and clock speed.
        /// </summary>
        /// <param name="prescale">Prescale value from which to calculate frequency.</param>
        /// <param name="clockSpeed">Clock speed with which to calculate the frequency.</param>
        /// <returns>Calculated frequency in Hz.</returns>
        public static int CalculateFrequency(byte prescale, int clockSpeed)
        {
            // Validate
            if (prescale < PrescaleMinimum || prescale > PrescaleMaximum)
                throw new ArgumentOutOfRangeException(nameof(prescale));
            if (clockSpeed <= 0 || clockSpeed > ClockSpeedMaximum)
                throw new ArgumentOutOfRangeException(nameof(clockSpeed));

            // Calculate and return result
            return Convert.ToInt32(Math.Round(clockSpeed / 4096f / (prescale + 1f)));
        }

        /// <summary>
        /// Calculates the <see cref="Pca9685Register.Prescale"/> value from a desired frequency and clock speed.
        /// </summary>
        /// <remarks>
        /// Due to scaling only certain frequencies are possible. To get the resulting frequency from the desired
        /// frequency it is necessary to re-calculate the effective frequency back from the prescale,
        /// i.e. call <see cref="CalculateFrequency"/>.
        /// </remarks>
        /// <param name="frequency">
        /// Desired frequency in Hz.
        /// Must be between <see cref="FrequencyMinimum"/> and <see cref="FrequencyMaximum"/> to get a valid result.
        /// </param>
        /// <param name="clockSpeed"></param>
        /// <returns>Calculated prescale value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when an invalid parameter is passed.
        /// </exception>
        /// <exception cref="OverflowException">
        /// Thrown when parameters are passed which causes the resulting prescale to be higher than
        /// <see cref="PrescaleMaximum"/> or lower than <see cref="PrescaleMinimum"/>.
        /// </exception>
        public static byte CalculatePrescale(int frequency, int clockSpeed)
        {
            // Validate
            if (frequency <= 0)
                throw new ArgumentOutOfRangeException(nameof(frequency));
            if (clockSpeed <= 0 || clockSpeed > ClockSpeedMaximum)
                throw new ArgumentOutOfRangeException(nameof(clockSpeed));

            // Calculate and prescale
            return Convert.ToByte(Math.Round(clockSpeed / (4096f * frequency)) - 1);
        }

        /// <summary>
        /// Reads the prescale register and calculates the <see cref="Frequency"/> (and related properties)
        /// based on <see cref="ClockSpeed"/>.
        /// </summary>
        /// <returns>
        /// Frequency in Hz. Related properties are also updated.
        /// </returns>
        public int ReadFrequency()
        {
            // Read prescale register
            var prescale = I2cExtensions.WriteReadByte(_hardware, (byte)Pca9685Register.Prescale);

            // Calculate frequency
            var frequency = CalculateFrequency(prescale, ClockSpeed);

            // Update related properties
            Frequency = frequency;
            PwmMsMinimum = Pca9685ChannelValue.CalculateWidthMs(frequency, 0);
            PwmMsMaximum = Pca9685ChannelValue.CalculateWidthMs(frequency, Pca9685ChannelValue.Maximum);

            // Return result
            return frequency;
        }

        /// <summary>
        /// Calculates the prescale value from the frequency (according to <see cref="ClockSpeed"/>)
        /// then writes that register, then calls <see cref="ReadFrequency"/> to update properties.
        /// Note the actual frequency may differ to the requested frequency due to clock scale (rounding).
        /// </summary>
        /// <remarks>
        /// The prescale can only be set during sleep mode. This method enters <see cref="Sleep"/> if necessary,
        /// then only if the device was awake before, calls <see cref="Wake"/> afterwards. It's important not to
        /// start output unexpectedly to avoid damage, i.e. if the device was sleeping before, the frequency is
        /// changed without starting the oscillator.
        /// </remarks>
        /// <param name="frequency">Desired frequency to set in Hz.</param>
        /// <returns>
        /// Effective frequency in Hz, read-back and recalculated after setting the desired frequency.
        /// Frequency in Hz. Related properties are also updated.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="frequency"/> is less than <see cref="FrequencyMinimum"/> or greater than
        /// <see cref="FrequencyMaximum"/>.
        /// </exception>
        public int WriteFrequency(int frequency)
        {
            // Validate
            if (frequency < FrequencyMinimum || frequency > FrequencyMaximum)
                throw new ArgumentOutOfRangeException(nameof(frequency));

            // Calculate prescale
            var prescale = CalculatePrescale(frequency, ClockSpeed);

            // Enter sleep mode and record wake status
            var wasAwake = Sleep();

            // Write prescale
            I2cExtensions.WriteJoinByte(_hardware, (byte)Pca9685Register.Prescale, prescale);

            // Read result
            var actual = ReadFrequency();

            // Wake-up if previously running
            if (wasAwake)
                Wake();

            // Update related properties
            PwmMsMinimum = Pca9685ChannelValue.CalculateWidthMs(frequency, 0);
            PwmMsMaximum = Pca9685ChannelValue.CalculateWidthMs(frequency, Pca9685ChannelValue.Maximum);

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
        public void Clear()
        {
            // Enable all channels
            I2cExtensions.WriteJoinByte(_hardware, (byte)Pca9685Register.AllChannelsOffHigh, 0x00);

            // Zero all channels
            I2cExtensions.WriteJoinBytes(_hardware, (byte)Pca9685Register.AllChannelsOnLow, new byte[] { 0x00, 0x00, 0x00, 0x00 });

            // Disable all channels
            I2cExtensions.WriteJoinByte(_hardware, (byte)Pca9685Register.AllChannelsOffHigh, 0x10);

            // Update channels
            ReadAllChannels();
        }

        /// <summary>
        /// Reads a whole channel value (on and off), and updates it in <see cref="Channels"/>.
        /// </summary>
        /// <param name="index">Zero based channel number (0-15).</param>
        /// <returns>Channel value</returns>
        public Pca9685ChannelValue ReadChannel(int index)
        {
            // Validate
            if (index < 0 | index >= ChannelCount) throw new ArgumentOutOfRangeException(nameof(index));

            // Calculate register address
            var register = GetChannelAddress(index);

            // Read value
            var bytes = I2cExtensions.WriteReadBytes(_hardware, register, sizeof(ushort) * 2);

            // Update channel property and return result
            var value = Pca9685ChannelValue.FromByteArray(bytes);
            return _channels[index] = value;
        }

        /// <summary>
        /// Reads multiple channel values (both on and off for each), and updates it in <see cref="Channels"/>.
        /// </summary>
        /// <param name="index">Zero based channel number (0-15).</param>
        /// <param name="count">Number of channels to read.</param>
        /// <returns>Channel values</returns>
        public Collection<Pca9685ChannelValue> ReadChannels(int index, int count)
        {
            // Validate
            if (index < 0 | index >= ChannelCount)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (count < 1 || index + count > ChannelCount)
                throw new ArgumentOutOfRangeException(nameof(count));

            // Calculate register address
            var register = GetChannelAddress(index);

            // Send I2C command to read channels in one operation
            var data = I2cExtensions.WriteReadBytes(_hardware, register, ChannelSize * count);

            // Update channel properties and add to results
            var results = new Collection<Pca9685ChannelValue>();
            for (int channelIndex = index, offset = 0; count > 0; count--, channelIndex++, offset += ChannelSize)
            {
                // Calculate value
                var value = Pca9685ChannelValue.FromByteArray(data, offset);

                // Update property
                _channels[channelIndex] = value;

                // Add to results
                results.Add(value);
            }

            // Return results
            return results;
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
            var bytes = I2cExtensions.WriteReadBytes(_hardware, register, sizeof(ushort));
            var value = BitConverter.ToUInt16(bytes, 0);

            // Update channel when changed
            var oldValue = _channels[index];
            if (oldValue.On != value)
                _channels[index] = new Pca9685ChannelValue(value, oldValue.Off);

            // Return result
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
            var bytes = I2cExtensions.WriteReadBytes(_hardware, register, sizeof(ushort));
            var value = BitConverter.ToUInt16(bytes, 0);

            // Update channel when changed
            var oldValue = _channels[index];
            if (oldValue.Off != value)
                _channels[index] = new Pca9685ChannelValue(oldValue.On, value);

            // Return result
            return value;
        }

        /// <summary>
        /// Calculates the "on" and "off" values of a channel from length (and optional delay),
        /// then writes them together, and updates it in <see cref="Channels"/>.
        /// </summary>
        /// <param name="index">Zero based channel number (0-15) or 16 for the "all call" channel.</param>
        /// <param name="width">Pulse width in clock ticks.</param>
        /// <param name="delay">Optional delay in clock ticks.</param>
        /// <returns>
        /// Updated channel value or null when all channels were updated.
        /// </returns>
        public Pca9685ChannelValue? WriteChannelLength(int index, int width, int delay = 0)
        {
            // Call overloaded method
            return WriteChannel(index, Pca9685ChannelValue.FromWidth(width, delay));
        }

        /// <summary>
        /// Calculates the "on" and "off" values of a channel from milliseconds (and optional delay),
        /// then writes them together, and updates it in <see cref="Channels"/>.
        /// </summary>
        /// <param name="index">Zero based channel number (0-15) or 16 for the "all call" channel.</param>
        /// <param name="width">
        /// Pulse width in milliseconds. Cannot be greater than one clock interval (1000 / frequency).
        /// </param>
        /// <param name="delay">Optional delay in milliseconds. Cannot be greater than one clock interval (1000 / frequency).</param>
        /// <returns>
        /// Updated channel value or null when all channels were updated.
        /// </returns>
        public Pca9685ChannelValue? WriteChannelMs(int index, decimal width, int delay = 0)
        {
            // Call overloaded method
            return WriteChannel(index, Pca9685ChannelValue.FromWidthMs(width, Frequency, delay));
        }

        /// <summary>
        /// Writes the "on" and "off" values of a channel together, and updates it in <see cref="Channels"/>.
        /// </summary>
        /// <param name="index">Zero based channel number (0-15) or 16 for the "all call" channel.</param>
        /// <param name="value"><see cref="Pca9685ChannelValue"/> to write.</param>
        /// <returns>
        /// Updated channel value or null when all channels were updated.
        /// </returns>
        public Pca9685ChannelValue? WriteChannel(int index, Pca9685ChannelValue value)
        {
            // Validate
            if (index < 0 | index > ChannelCount) throw new ArgumentOutOfRangeException(nameof(index));
            if (value == null) throw new ArgumentNullException(nameof(value));

            // Calculate register address
            var register = GetChannelAddress(index);

            // Convert and write value
            var bytes = value.ToByteArray();
            I2cExtensions.WriteJoinBytes(_hardware, register, bytes);

            // Read and return result when single channel
            if (index < ChannelCount)
                return ReadChannel(index);

            // Read all channels when "all call".
            ReadAllChannels();
            return null;
        }


        /// <summary>
        /// Writes multiple channels together (both "on" and "off" values), and updates it in <see cref="Channels"/>.
        /// </summary>
        /// <param name="index">Zero based channel number (0-15) or 16 for the "all call" channel.</param>
        /// <param name="values">Collection of <see cref="Pca9685ChannelValue"/>s to write.</param>
        public void WriteChannels(int index, IList<Pca9685ChannelValue> values)
        {
            // Validate
            if (index < 0 | index > ChannelCount) throw new ArgumentOutOfRangeException(nameof(index));
            if (values == null || values.Count == 0) throw new ArgumentNullException(nameof(values));
            var count = values.Count;
            if (index + count > ChannelCount) throw new ArgumentOutOfRangeException(nameof(values));

            // Build I2C packet
            var data = new byte[1 + ChannelSize * count];

            // Calculate first register address
            var register = GetChannelAddress(index);
            data[0] = register;

            // Write channels and update properties
            for (int dataIndex = 0, dataOffset = 1; dataIndex < count; dataIndex++, dataOffset += ChannelSize)
            {
                // Get channel data
                var channelData = values[dataIndex].ToByteArray();

                // Copy to buffer
                Array.ConstrainedCopy(channelData, 0, data, dataOffset, ChannelSize);

                // Update property
                _channels[index + dataIndex] = Pca9685ChannelValue.FromByteArray(channelData);
            }

            // Send packet
            I2cExtensions.WriteBytes(_hardware, data);
        }

        /// <summary>
        /// Writes the PWM "on" (rising) value of a channel.
        /// </summary>
        /// <param name="index">Zero based channel number (0-15) or 16 for the "all call" channel.</param>
        /// <param name="value">12-bit channel value in the range 0-<see cref="Pca9685ChannelValue.Maximum"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="value"/> is greater than <see cref="Pca9685ChannelValue.Maximum"/>.</exception>
        public void WriteChannelOn(int index, int value)
        {
            // Validate
            if (value > Pca9685ChannelValue.Maximum) throw new ArgumentOutOfRangeException(nameof(value));

            // Calculate register address
            var register = GetChannelAddress(index);

            // Convert and write value
            var data = BitConverter.GetBytes(value);
            I2cExtensions.WriteJoinBytes(_hardware, register, data);
        }

        /// <summary>
        /// Writes the PWM "off" (falling) value of a channel.
        /// </summary>
        /// <param name="index">Zero based channel number (0-15) or 16 for the "all call" channel.</param>
        /// <param name="value">12-bit channel value in the range 0-<see cref="Pca9685ChannelValue.Maximum"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="value"/> is greater then <see cref="Pca9685ChannelValue.Maximum"/>.</exception>
        public void WriteChannelOff(int index, int value)
        {
            // Validate
            if (value > Pca9685ChannelValue.Maximum) throw new ArgumentOutOfRangeException(nameof(value));

            // Calculate register address of second word value
            var register = (byte)(GetChannelAddress(index) + sizeof(ushort));

            // Convert and write value
            var bytes = BitConverter.GetBytes(value);
            I2cExtensions.WriteJoinBytes(_hardware, register, bytes);
        }

        /// <summary>
        /// Reads all channels and updates <see cref="Channels"/>.
        /// </summary>
        public void ReadAllChannels()
        {
            // Read all channels as one block of data
            var data = I2cExtensions.WriteReadBytes(_hardware, ChannelStartAddress, ChannelSize * ChannelCount);

            // Update properties
            for (var index = 0; index < ChannelCount; index++)
                _channels[index] = Pca9685ChannelValue.FromByteArray(data, ChannelSize * index);
        }

        #endregion

        #endregion
    }
}
