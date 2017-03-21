using Emlid.UniversalWindows;
using Emlid.WindowsIot.Hardware.Components.Pca9685;
using Emlid.WindowsIot.Hardware.Protocols.Pwm;
using Emlid.WindowsIot.HardwarePlus.Buses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Devices.Gpio;

namespace Emlid.WindowsIot.Hardware.Boards.Navio.Internal
{
    /// <summary>
    /// Navio and Navio+ LED &amp; PWM servo device, a PCA9685 chip connected via I2C.
    /// </summary>
    /// <remarks>
    /// Navio uses the <see cref="Pca9685Device"/> as a dual-purpose PWM and LED driver,
    /// i.e. for servo control and the high intensity RGB status LED. It is connected via the I2C bus.
    /// See http://docs.emlid.com/Navio-dev/servo-and-rgb-led/ for more information.
    /// <seealso cref="Pca9685Device"/>
    /// </remarks>
    public sealed class Navio1LedPwmDevice : DisposableObject, INavioLedDevice, INavioPwmDevice
    {
        #region Constants

        /// <summary>
        /// I2C controller index of the chip on the Navio board.
        /// </summary>
        public const int I2cControllerIndex = 0;

        /// <summary>
        /// Chip number on the Navio board.
        /// </summary>
        public const byte ChipNumber = 0;

        /// <summary>
        /// GPIO controller index of the <see cref="OutputEnableGpioPin"/> on the Navio board.
        /// </summary>
        public const int GpioControllerIndex = 0;

        /// <summary>
        /// Raspberry Pi GPIO pin which enables PCA output.
        /// </summary>
        public const int OutputEnableGpioPin = 27;

        /// <summary>
        /// External clock speed in Hz generated from an TCXO oscillator.
        /// </summary>
        public const int ExternalClockSpeed = 24576000;

        /// <summary>
        /// I2C channel index of the red value in the high intensity RGB LED.
        /// </summary>
        public const int LedRedChannelIndex = 2;

        /// <summary>
        /// I2C channel index of the green value in the high intensity RGB LED.
        /// </summary>
        public const int LedGreenChannelIndex = 1;

        /// <summary>
        /// I2C channel index of the blue value in the high intensity RGB LED.
        /// </summary>
        public const int LedBlueChannelIndex = 0;

        /// <summary>
        /// I2C channel index of the first PWM channel.
        /// </summary>
        public const int PwmChannelIndex = 3;

        /// <summary>
        /// Number of PWM channels.
        /// </summary>
        public const int PwmChannelCount = 13;

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates an instance and reads current values.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The required mode bits are set, but the device state and PWM values unchanged (supporting recovery).
        /// Initializing without restart allows the caller decide whether to recover or reset the device.
        /// Before starting any output be sure to check the <see cref="INavioPwmDevice.Frequency"/>.
        /// </para>
        /// <para>
        /// To start with new settings, call <see cref="Reset"/> then set <see cref="Enabled"/>.
        /// </para>
        /// </remarks>
        public Navio1LedPwmDevice()
        {
            // Initialize members
            _pwmChannels = new PwmPulse[PwmChannelCount];
            _pwmChannelsReadOnly = new ReadOnlyCollection<PwmPulse>(_pwmChannels);

            // Connect to hardware
            _device = new Pca9685Device(I2cControllerIndex, ChipNumber, ExternalClockSpeed);
            _enablePin = GpioExtensions.Connect(GpioControllerIndex, OutputEnableGpioPin,
                GpioPinDriveMode.Output, GpioSharingMode.Exclusive);

            // Read properties
            Read();
        }

        #region IDisposable

        /// <summary>
        /// Frees resources owned by this instance.
        /// </summary>
        /// <param name="disposing">
        /// True when called via <see cref="IDisposable.Dispose()"/>, false when called from the finalizer.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            // Only managed resources to dispose
            if (!disposing)
                return;

            // Disable output
            _enablePin.Write(GpioPinValue.High);

            // Sleep oscillator
            _device.Sleep();

            // Close devices
            _device?.Dispose();
            _enablePin?.Dispose();
        }

        #endregion

        #endregion

        #region Private Fields

        /// <summary>
        /// Thread synchronization.
        /// </summary>
        private static object _lock = new object();

        /// <summary>
        /// LED/PWM device.
        /// </summary>
        private Pca9685Device _device;

        /// <summary>
        /// GPIO pin which enables PWM output.
        /// </summary>
        private readonly GpioPin _enablePin;

        #endregion

        #region Common Properties

        /// <summary>
        /// LED interface.
        /// </summary>
        public INavioLedDevice Led => this;

        /// <summary>
        /// PWM interface.
        /// </summary>
        public INavioPwmDevice Pwm => this;

        /// <summary>
        /// Indicates whether the device can be disabled, i.e. must be enabled before it will generate output.
        /// </summary>
        public bool CanDisable => true;

        /// <summary>
        /// Controls output by driving the <see cref="OutputEnableGpioPin"/>
        /// low (enabled) or high (disabled).
        /// </summary>
        /// <remarks>
        /// It's extremely important to set the frequency to a known value before starting output.
        /// Use <see cref="PwmPulse.ServoSafeFrequency"/> for most analog servos. Digital servos support
        /// much faster update speeds, so if you have one read their specification and choose a
        /// sensible value (perhaps not maximum) for best performance without overheating.
        /// </remarks>
        public bool Enabled
        {
            get
            {
                // Thread-safe lock
                lock (_lock)
                {
                    // Enabled when not sleeping and output enable pin low
                    return
                        ((_device.Mode1Register & Pca9685Mode1Bits.Sleep) == 0) &&
                        (_enablePin.Read() == GpioPinValue.Low);
                }
            }
            set
            {
                // Thread-safe lock
                lock (_lock)
                {
                    // Wake or sleep
                    if (value)
                    {
                        // Wake oscillator
                        _device.Wake();

                        // Enable output
                        _enablePin.Write(GpioPinValue.Low);
                    }
                    else
                    {
                        // Disable output
                        _enablePin.Write(GpioPinValue.High);

                        // Sleep oscillator
                        _device.Sleep();
                    }
                }
            }
        }

        #endregion

        #region Common Methods

        /// <summary>
        /// Clears all values and resets the device state to default (disabled).
        /// </summary>
        public void Reset()
        {
            // Thread-safe lock
            lock (_lock)
            {
                // Disable output if necessary
                if (_enablePin.Read() == GpioPinValue.Low)
                    _enablePin.Write(GpioPinValue.High);

                // Disable oscillator
                _device.Sleep();

                // Execute device clear sequence
                _device.Clear();

                // Set default frequency
                _device.WriteFrequency(_device.FrequencyDefault);
            }
        }

        /// <summary>
        /// Reads the LED &amp; PWM channels from the device then updates the related properties.
        /// </summary>
        public void Read()
        {
            // Thread-safe lock
            lock (_lock)
            {
                // Read all PWM and LED channels
                _device.ReadAll();

                // Update LED properties
                _ledRed = ~_device.Channels[LedRedChannelIndex].Width & 0xfff;
                _ledGreen = ~_device.Channels[LedGreenChannelIndex].Width & 0xfff;
                _ledBlue = ~_device.Channels[LedGreenChannelIndex].Width & 0xfff;

                // Update PWM properties
                var frequency = _device.Frequency;
                for (int index = 0, pwmIndex = PwmChannelIndex; index < PwmChannelCount; index++, pwmIndex++)
                {
                    var widthTicks = _device.Channels[pwmIndex].Width;
                    var widthMs = Pca9685ChannelValue.CalculateWidthMs(frequency, widthTicks);
                    _pwmChannels[index] = PwmPulse.FromWidth(frequency, widthMs);
                }
            }
        }

        #endregion

        #region LED Interface Specific

        #region Properties

        /// <summary>
        /// Maximum value of any color component.
        /// </summary>
        /// <remarks>
        /// The color range is calculated as <see cref="INavioLedDevice.MaximumValue"/> + 1 ^3.
        /// </remarks>
        int INavioLedDevice.MaximumValue => Pca9685ChannelValue.Maximum;

        /// <summary>
        /// Intensity of the red LED component.
        /// </summary>
        /// <remarks>
        /// Value in the range 0-<see cref="INavioLedDevice.MaximumValue"/>.
        /// </remarks>
        int INavioLedDevice.Red
        {
            get
            {
                // Return cached value
                return _ledRed;
            }
            set
            {
                // Thread-safe lock
                lock (_lock)
                {
                    // Validate
                    if (value < 0 || value > Pca9685ChannelValue.Maximum)
                        throw new ArgumentOutOfRangeException(nameof(value));

                    // Calculate inverse red channel value
                    var inverseValue = ~value & 0x0fff;
                    var ledValue = Pca9685ChannelValue.FromWidth(inverseValue);

                    // Set inverse value
                    _device.WriteChannel(LedRedChannelIndex, ledValue);

                    // Cache value
                    _ledRed = value;
                }
            }
        }
        private int _ledRed;

        /// <summary>
        /// Intensity of the green LED component.
        /// </summary>
        /// <remarks>
        /// Value in the range 0-<see cref="INavioLedDevice.MaximumValue"/>.
        /// </remarks>
        int INavioLedDevice.Green
        {
            get
            {
                // Return cached value
                return _ledGreen;
            }
            set
            {
                // Thread-safe lock
                lock (_lock)
                {
                    // Validate
                    if (value < 0 || value > Pca9685ChannelValue.Maximum)
                        throw new ArgumentOutOfRangeException(nameof(value));

                    // Calculate inverse green channel value
                    var inverseValue = ~value & 0x0fff;
                    var ledValue = Pca9685ChannelValue.FromWidth(inverseValue);

                    // Set inverse value
                    _device.WriteChannel(LedGreenChannelIndex, ledValue);

                    // Cache value
                    _ledGreen = value;
                }
            }
        }
        private int _ledGreen;

        /// <summary>
        /// Intensity of the green LED component.
        /// </summary>
        /// <remarks>
        /// Value in the range 0-<see cref="INavioLedDevice.MaximumValue"/>.
        /// </remarks>
        int INavioLedDevice.Blue
        {
            get
            {
                // Return cached value
                return _ledBlue;
            }
            set
            {
                // Thread-safe lock
                lock (_lock)
                {
                    // Validate
                    if (value < 0 || value > Pca9685ChannelValue.Maximum)
                        throw new ArgumentOutOfRangeException(nameof(value));

                    // Calculate inverse blue channel value
                    var inverseValue = ~value & 0x0fff;
                    var ledValue = Pca9685ChannelValue.FromWidth(inverseValue);

                    // Set inverse value
                    _device.WriteChannel(LedBlueChannelIndex, ledValue);

                    // Cache value
                    _ledBlue = value;
                }
            }
        }
        private int _ledBlue;

        #endregion

        #region Methods

        /// <summary>
        /// Clears all LED values.
        /// </summary>
        void INavioLedDevice.Reset()
        {
            // Thread-safe lock
            lock (_lock)
            {
                // Clear channels
                _device.WriteChannels(0, new Pca9685ChannelValue[3]);

                // Update properties
                Led.Read();
            }
        }

        /// <summary>
        /// Reads the LED values from the device then updates the related properties.
        /// </summary>
        void INavioLedDevice.Read()
        {
            // Thread-safe lock
            lock (_lock)
            {
                // Read channels together (BGR ordered)
                var channels = _device.ReadChannels(0, 3);

                // Update channel properties
                _ledBlue = ~channels[0].Width & 0xfff;
                _ledGreen = ~channels[1].Width & 0xfff;
                _ledRed = ~channels[2].Width & 0xfff;
            }
        }

        /// <summary>
        /// Sets the LED <see cref="INavioLedDevice.Red"/>, <see cref="INavioLedDevice.Green"/> and
        /// <see cref="INavioLedDevice.Blue"/> values together (in one operation).
        /// </summary>
        /// <param name="red">Red value in the range 0-<see cref="INavioLedDevice.MaximumValue"/>.</param>
        /// <param name="green">Green value in the range 0-<see cref="INavioLedDevice.MaximumValue"/>.</param>
        /// <param name="blue">Blue value in the range 0-<see cref="INavioLedDevice.MaximumValue"/>.</param>
        void INavioLedDevice.SetRgb(int red, int green, int blue)
        {
            // Thread-safe lock
            lock (_lock)
            {
                // Validate
                if (red < 0 || red > Pca9685ChannelValue.Maximum) throw new ArgumentOutOfRangeException(nameof(red));
                if (green < 0 || green > Pca9685ChannelValue.Maximum) throw new ArgumentOutOfRangeException(nameof(green));
                if (blue < 0 || blue > Pca9685ChannelValue.Maximum) throw new ArgumentOutOfRangeException(nameof(blue));

                // Invert values
                var inverseRed = ~red & 0x0fff;
                var inverseGreen = ~green & 0x0fff;
                var inverseBlue = ~blue & 0x0fff;

                // Set all three LED channels together (BGR order)
                var values = new Pca9685ChannelValue[]
                {
                Pca9685ChannelValue.FromWidth(inverseBlue),
                Pca9685ChannelValue.FromWidth(inverseGreen),
                Pca9685ChannelValue.FromWidth(inverseRed)
                };
                _device.WriteChannels(0, values);

                // Update cached values
                _ledBlue = blue;
                _ledGreen = green;
                _ledRed = red;
            }
        }

        #endregion

        #endregion

        #region PWM Interface Specific

        #region Properties

        /// <summary>
        /// Frequency for all channels in Hz.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When <see cref="INavioPwmDevice.FrequencyPerChannel"/> is true, reading this value returns the highest of all frequencies
        /// and writing it sets all channels to the same frequency.
        /// When <see cref="INavioPwmDevice.FrequencyPerChannel"/> is false, the device does not support multiple frequencies so
        /// all values are tied together.
        /// </para>
        /// <para>
        /// When setting the frequency, the device clock and oscillator characteristics may cause the resulting
        /// frequency to be different to what was set. Read back the value after setting to get the actual value.
        /// </para>
        /// <para>
        /// Some PWM devices do not tolerate high values and could be damaged if this is set too high,
        /// e.g. analog servos operate at much lower frequencies than digital servos.
        /// See <see cref="PwmPulse.ServoSafeFrequency"/> for more information.
        /// </para>
        /// </remarks>
        int INavioPwmDevice.Frequency
        {
            get { return _device.Frequency; }
            set
            {
                // Thread-safe lock
                lock (_lock)
                {
                    // Do nothing when same
                    if (value == _device.Frequency)
                        return;

                    // Disable output if necessary
                    var wasEnabled = _enablePin.Read() == GpioPinValue.Low;
                    if (wasEnabled)
                        _enablePin.Write(GpioPinValue.High);

                    // Set the hardware frequency
                    _device.WriteFrequency(value);

                    // Update properties
                    Read();

                    // Re-enable when disabled (and not disabled before)
                    if (wasEnabled)
                        _enablePin.Write(GpioPinValue.Low);
                }
            }
        }

        /// <summary>
        /// Indicates whether the frequency can be controlled individually for each channel.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When true, the <see cref="PwmPulse.Frequency"/> of each <see cref="PwmPulse"/> in <see cref="INavioPwmDevice.Channels"/>
        /// can be used to change the frequency of individual channels. The <see cref="INavioPwmDevice.Frequency"/> can still be
        /// used to set all channel frequencies at once, or get the highest frequency of all channels.
        /// </para>
        /// <para>
        /// When false, only the <see cref="INavioPwmDevice.Frequency"/> value can be used
        /// and any attempt to change the frequency of individual channels will throw an
        /// error.
        /// </para>
        /// </remarks>
        bool INavioPwmDevice.FrequencyPerChannel => false;

        /// <summary>
        /// Minimum frequency in Hz.
        /// </summary>
        int INavioPwmDevice.FrequencyMinimum => _device.FrequencyMinimum;

        /// <summary>
        /// Maximum frequency in Hz.
        /// </summary>
        int INavioPwmDevice.FrequencyMaximum => _device.FrequencyMaximum;

        /// <summary>
        /// Minimum pulse width in fractions of milliseconds, based on the current <see cref="INavioPwmDevice.Frequency"/>.
        /// </summary>
        decimal INavioPwmDevice.WidthMinimum => _device.PwmMsMinimum;

        /// <summary>
        /// Maximum pulse width in fractions of milliseconds, based on the current <see cref="INavioPwmDevice.Frequency"/>.
        /// </summary>
        decimal INavioPwmDevice.WidthMaximum => _device.PwmMsMaximum;

        /// <summary>
        /// PWM channels, used to get or set the PWM duty cycle or width.
        /// </summary>
        /// <remarks>
        /// The <see cref="PwmPulse.Frequency"/> cannot be changed as this device does not
        /// support independent frequencies per channel.
        /// </remarks>
        ReadOnlyCollection<PwmPulse> INavioPwmDevice.Channels => _pwmChannelsReadOnly;
        private ReadOnlyCollection<PwmPulse> _pwmChannelsReadOnly;
        private PwmPulse[] _pwmChannels;

        #endregion

        #region Methods

        /// <summary>
        /// Clears all PWM channel values.
        /// </summary>
        void INavioPwmDevice.Reset()
        {
            // Thread-safe lock
            lock (_lock)
            {
                // Clear channels
                _device.WriteChannels(PwmChannelIndex, new Pca9685ChannelValue[PwmChannelCount]);

                // Update properties
                Pwm.Read();
            }
        }

        /// <summary>
        /// Reads the PWM channels from the device then updates the related properties.
        /// </summary>
        void INavioPwmDevice.Read()
        {
            // Thread-safe lock
            lock (_lock)
            {
                // Read channels together (BGR ordered)
                var channels = _device.ReadChannels(PwmChannelIndex, PwmChannelCount);

                // Update channel properties
                var frequency = _device.Frequency;
                for (int index = 0; index < PwmChannelCount; index++)
                {
                    var widthTicks = channels[index].Width;
                    var widthMs = Pca9685ChannelValue.CalculateWidthMs(frequency, widthTicks);
                    _pwmChannels[index] = PwmPulse.FromWidth(frequency, widthMs);
                }
            }
        }

        /// <summary>
        /// Sets a single channel value.
        /// </summary>
        void INavioPwmDevice.SetChannel(int number, PwmPulse value)
        {
            // Thread-safe lock
            lock (_lock)
            {
                // Validate
                if (number < 1 || number > PwmChannelCount)
                    throw new ArgumentOutOfRangeException(nameof(number));
                var index = number - 1;

                // Check frequency matches (no per-channel frequency on this device)
                if (value.Frequency != _device.Frequency)
                    throw new ArgumentOutOfRangeException(nameof(value.Frequency));

                // Do nothing when unchanged
                var oldValue = _pwmChannels[index];
                if (value == oldValue)
                    return;

                // Write new value
                _device.WriteChannelMs(PwmChannelIndex + index, value.Width);

                // Update property
                _pwmChannels[index] = value;
            }
        }

        /// <summary>
        /// Sets multiple channel values at once.
        /// </summary>
        void INavioPwmDevice.SetChannels(int number, IList<PwmPulse> values, int count)
        {
            // Thread-safe lock
            lock (_lock)
            {
                // Validate
                if (number < 1 || number > PwmChannelCount)
                    throw new ArgumentOutOfRangeException(nameof(number));
                if (values == null)
                    throw new ArgumentNullException(nameof(values));
                if (count < 1 || number + count > PwmChannelCount)
                    throw new ArgumentOutOfRangeException(nameof(count));

                // Build device values, check frequency and detect change
                var changed = false;
                var channelValues = new List<Pca9685ChannelValue>();
                for (var index = 0; index < count; index++)
                {
                    // Get value and convert to channel value
                    var value = values[index];
                    channelValues.Add(Pca9685ChannelValue.FromWidthMs(value.Width, value.Frequency, 0));

                    // Check frequency
                    if (value.Frequency != _device.Frequency)
                        throw new ArgumentOutOfRangeException(nameof(value.Frequency));

                    // Check value
                    var oldValue = _pwmChannels[number + index - 1];
                    if (value != oldValue)
                        changed = true;
                }

                // Do nothing when unchanged
                if (!changed)
                    return;

                // Write new value
                _device.WriteChannels(PwmChannelIndex + number - 1, channelValues);

                // Update properties
                for (var index = 0; index < count; index++)
                    _pwmChannels[index] = values[index];
            }
        }

        #endregion

        #endregion
    }
}
