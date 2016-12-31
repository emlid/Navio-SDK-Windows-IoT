using Emlid.WindowsIot.Hardware.Components.NxpPca9685;
using Emlid.WindowsIot.Hardware.Protocols.Pwm;
using Emlid.WindowsIot.Hardware.System;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Windows.Devices.Gpio;
using Windows.Devices.I2c;

namespace Emlid.WindowsIot.Hardware.Boards.Navio
{
    /// <summary>
    /// Navio and Navio+ LED &amp; PWM servo device, a PCA9685 chip connected via I2C.
    /// </summary>
    /// <remarks>
    /// Navio uses the <see cref="NxpPca9685Device"/> as a dual-purpose PWM and LED driver,
    /// i.e. for servo control and the high intensity RGB status LED. It is connected via the I2C bus.
    /// See http://docs.emlid.com/Navio-dev/servo-and-rgb-led/ for more information.
    /// <seealso cref="NxpPca9685Device"/>
    /// </remarks>
    public sealed class NavioLedPwmDevice : NxpPca9685Device, INavioLedDevice, INavioPwmDevice
    {
        #region Constants

        /// <summary>
        /// External clock speed in Hz generated from an TCXO oscillator.
        /// </summary>
        public const int ExternalClockSpeed = 24576000;

        /// <summary>
        /// Raspberry Pi GPIO pin which enables PCA output.
        /// </summary>
        public const int OutputEnableGpioPin = 27;

        /// <summary>
        /// I2C controller index of the chip on the Navio board.
        /// </summary>
        public const int I2cControllerIndex = 0;

        /// <summary>
        /// I2C address of the PCA9685 on the Navio board.
        /// </summary>
        public const int I2cAddress = 0x0040;

        /// <summary>
        /// I2C address of the PCA9685 "all call" address on the Navio board.
        /// </summary>
        public const int I2cAllCallAddress = 0x0070;

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
        /// Before starting any output be sure to check the <see cref="NxpPca9685Device.Frequency"/>.
        /// </para>
        /// <para>
        /// To start with new settings, call <see cref="NxpPca9685Device.Clear"/>, <seealso cref="Restart"/> then set <see cref="OutputEnabled"/>.
        /// To resume, call <see cref="NxpPca9685Device.Wake"/> then set <see cref="OutputEnabled"/>.
        /// </para>
        /// </remarks>
        /// <param name="device">I2C device.</param>
        [CLSCompliant(false)]
        public NavioLedPwmDevice(I2cDevice device) : base(device, ExternalClockSpeed)
        {
            // Add PWM channel shortcuts
            Pwm = new ObservableCollection<NxpPca9685Channel>();
            for (var index = PwmChannelIndex; index < ChannelCount; index++)
                Pwm.Add(Channels[index]);

            // Initialize output pin and ensure in correct mode (but leave current state)
            _outputEnablePin = GpioController.GetDefault().OpenPin(OutputEnableGpioPin);
            if (_outputEnablePin.GetDriveMode() != GpioPinDriveMode.Output)
                _outputEnablePin.SetDriveMode(GpioPinDriveMode.Output);

            // Enable auto-increment and "all call"
            Hardware.WriteReadWriteBit((byte)NxpPca9685Register.Mode1,
                (byte)(NxpPca9685Mode1Bits.AutoIncrement | NxpPca9685Mode1Bits.AllCall), true);

            // Set "all call" address
            Hardware.WriteJoinByte((byte)NxpPca9685Register.AllCall, I2cAllCallAddress);

            // Update property
            ReadMode1();

            // Setup LED channels

            var redChannel = Channels[LedRedChannelIndex];
            redChannel.ValueChanged += OnLedRedChannelChanged;
            _ledRed = ~redChannel.Value.Length & 0xfff;

            var greenChannel = Channels[LedGreenChannelIndex];
            greenChannel.ValueChanged += OnLedGreenChannelChanged;
            _ledGreen = ~greenChannel.Value.Length & 0xfff;

            var blueChannel = Channels[LedBlueChannelIndex];
            blueChannel.ValueChanged += OnLedBlueChannelChanged;
            _ledBlue = ~blueChannel.Value.Length & 0xfff;
        }

        /// <summary>
        /// Creates an initialized instance.
        /// </summary>
        /// <param name="frequency">
        /// Some PWM devices do not tolerate high values and could be damaged if this is set too high,
        /// e.g. analog servos operate at much lower frequencies than digital servos.
        /// The default <see cref="PwmCycle.ServoSafeFrequency"/> supports most analog servos.
        /// </param>
        /// <param name="clear">Clears all LED/PWM values.</param>
        /// <param name="restart">
        /// Set true to restart the device which also enables the oscillator.
        /// When false, you have to call <see cref="NxpPca9685Device.Wake"/> or <see cref="Restart"/> later to enable the oscillator.
        /// </param>
        /// <param name="enable">
        /// Set true to enable output, or false to continue with further initialization before enabling output.
        /// When false, you have to set it later before any output can occur.
        /// Defaults to false to prevent unexpected behavior from damaging hardware.
        /// </param>
        public static NavioLedPwmDevice Initialize(float frequency = PwmCycle.ServoSafeFrequency,
            bool clear = true, bool restart = true, bool enable = false)
        {
            // Connect to I2C device
            var device = DeviceProvider.ConnectI2c(I2cControllerIndex, I2cAddress);
            if (device == null)
            {
                // Initialization error
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    Resources.Strings.I2cErrorDeviceNotFound, I2cAddress, I2cControllerIndex));
            }

            // Perform device initialization sequence...
            var navioDevice = new NavioLedPwmDevice(device)
            {
                // Start with output disabled (protect against potential damage)
                OutputEnabled = false
            };
            navioDevice.WriteFrequency(frequency);          // Set frequency
            if (clear) navioDevice.Clear();                 // Clear LED/PWM values when specified
            if (restart) navioDevice.Restart();             // Restart when specified
            if (enable) navioDevice.OutputEnabled = true;   // Enable output when specified


            // Return initialized device
            return navioDevice;
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
            try
            {
                // Only managed resources to dispose
                if (!disposing)
                    return;

                // Put chip to sleep
                Sleep();

                // Unhook events
                Channels[LedRedChannelIndex].ValueChanged -= OnLedRedChannelChanged;
                Channels[LedGreenChannelIndex].ValueChanged -= OnLedGreenChannelChanged;
                Channels[LedBlueChannelIndex].ValueChanged -= OnLedBlueChannelChanged;

                // Close device
                _outputEnablePin?.Dispose();
            }
            finally
            {
                // Dispose base class
                base.Dispose(disposing);
            }
        }

        #endregion

        #endregion

        #region Private Fields

        /// <summary>
        /// GPIO output enable pin.
        /// </summary>
        private readonly GpioPin _outputEnablePin;

        #endregion

        #region Public Properties

        /// <summary>
        /// Red component of the high intensity LED as intensity (inverse length).
        /// </summary>
        /// <remarks>
        /// Value in the range 0-<see cref="NxpPca9685ChannelValue.Maximum"/>.
        /// </remarks>
        public int LedRed
        {
            get
            {
                // Return cached value
                return _ledRed;
            }
            set
            {
                // Validate
                if (value < 0 || value > NxpPca9685ChannelValue.Maximum)
                    throw new ArgumentOutOfRangeException(nameof(value));

                // Calculate inverse red channel value
                var inverseValue = ~value & 0x0fff;
                var ledValue = NxpPca9685ChannelValue.FromLength(inverseValue);

                // Set inverse value
                WriteChannel(LedRedChannelIndex, ledValue);

                // Cache
                _ledRed = value;
            }
        }
        private int _ledRed;

        /// <summary>
        /// Green component of the high intensity LED as intensity (inverse length).
        /// </summary>
        /// <remarks>
        /// Value in the range 0-<see cref="NxpPca9685ChannelValue.Maximum"/>.
        /// </remarks>
        public int LedGreen
        {
            get
            {
                // Return cached value
                return _ledGreen;
            }
            set
            {
                // Validate
                if (value < 0 || value > NxpPca9685ChannelValue.Maximum)
                    throw new ArgumentOutOfRangeException(nameof(value));

                // Calculate inverse green channel value
                var inverseValue = ~value & 0x0fff;
                var ledValue = NxpPca9685ChannelValue.FromLength(inverseValue);

                // Set inverse value
                WriteChannel(LedGreenChannelIndex, ledValue);

                // Cache
                _ledGreen = value;
            }
        }
        private int _ledGreen;

        /// <summary>
        /// Blue component of the high intensity LED as intensity (inverse length).
        /// </summary>
        /// <remarks>
        /// Value in the range 0-<see cref="NxpPca9685ChannelValue.Maximum"/>.
        /// </remarks>
        public int LedBlue
        {
            get
            {
                // Return cached value
                return _ledBlue;
            }
            set
            {
                // Validate
                if (value < 0 || value > NxpPca9685ChannelValue.Maximum)
                    throw new ArgumentOutOfRangeException(nameof(value));

                // Calculate inverse blue channel value
                var inverseValue = ~value & 0x0fff;
                var ledValue = NxpPca9685ChannelValue.FromLength(inverseValue);

                // Set inverse value
                WriteChannel(LedBlueChannelIndex, ledValue);

                // Cache
                _ledBlue = value;
            }
        }
        private int _ledBlue;

        /// <summary>
        /// Values of all the PWM channels 1-<see cref="PwmChannelCount"/>.
        /// </summary>
        public Collection<NxpPca9685Channel> Pwm { get; private set; }

        /// <summary>
        /// Controls output by driving the <see cref="OutputEnableGpioPin"/>
        /// low (enabled) or high (disabled).
        /// </summary>
        /// <remarks>
        /// It's extremely important to set the frequency to a known value before starting output.
        /// Use <see cref="PwmCycle.ServoSafeFrequency"/> for most analog servos. Digital servos support
        /// much faster update speeds, so if you have one read their specification and choose a
        /// sensible value (perhaps not maximum) for best performance without overheating.
        /// </remarks>
        public bool OutputEnabled
        {
            get { return _outputEnablePin?.Read() == GpioPinValue.Low; }
            set { _outputEnablePin?.Write(value ? GpioPinValue.Low : GpioPinValue.High); }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Restarts the device with additional options specified, then updates all properties.
        /// </summary>
        /// <param name="options">
        /// Optional mode 1 parameters to add to the final restart sequence. A logical OR is applied to this value and
        /// the standard <see cref="NxpPca9685Mode1Bits.Restart"/>, <see cref="NxpPca9685Mode1Bits.ExternalClock"/>,
        /// <see cref="NxpPca9685Mode1Bits.AutoIncrement"/> and <see cref="NxpPca9685Mode1Bits.AllCall"/> bits.
        /// </param>
        public override void Restart(NxpPca9685Mode1Bits options)
        {
            base.Restart(options | NxpPca9685Mode1Bits.AllCall);
        }

        /// <summary>
        /// Reads all values from the device and updates properties.
        /// </summary>
        public override void ReadAll()
        {
            // Call base class method
            base.ReadAll();

            // Read LED channels
            ReadLed();
        }

        /// <summary>
        /// Sets RGB values of the high intensity LED together (in one operation).
        /// </summary>
        /// <remarks>
        /// Automatically inverts the value, providing a natural behavior where a higher number
        /// produces a higher LED intensity. Normally, when written as raw PWM values, the output
        /// is inverted due to common anode.
        /// </remarks>
        /// <param name="red">Red value in the range 0-<see cref="NxpPca9685ChannelValue.Maximum"/>.</param>
        /// <param name="green">Green value in the range 0-<see cref="NxpPca9685ChannelValue.Maximum"/>.</param>
        /// <param name="blue">Blue value in the range 0-<see cref="NxpPca9685ChannelValue.Maximum"/>.</param>
        public void SetLed(int red, int green, int blue)
        {
            // Validate
            if (red < 0 || red > NxpPca9685ChannelValue.Maximum) throw new ArgumentOutOfRangeException(nameof(red));
            if (green < 0 || green > NxpPca9685ChannelValue.Maximum) throw new ArgumentOutOfRangeException(nameof(green));
            if (blue < 0 || blue > NxpPca9685ChannelValue.Maximum) throw new ArgumentOutOfRangeException(nameof(blue));

            // Invert values
            var inverseRed = ~red & 0x0fff;
            var inverseGreen = ~green & 0x0fff;
            var inverseBlue = ~blue & 0x0fff;

            // Build block of data which sets all three register high and low values
            var redBytes = NxpPca9685ChannelValue.FromLength(inverseRed).ToByteArray();
            var greenBytes = NxpPca9685ChannelValue.FromLength(inverseGreen).ToByteArray();
            var blueBytes = NxpPca9685ChannelValue.FromLength(inverseBlue).ToByteArray();
            var data = new byte[1 + ChannelSize * 3];
            data[0] = ChannelStartAddress;
            Array.ConstrainedCopy(blueBytes, 0, data, 1, ChannelSize);
            Array.ConstrainedCopy(greenBytes, 0, data, 1 + ChannelSize, ChannelSize);
            Array.ConstrainedCopy(redBytes, 0, data, 1 + ChannelSize * 2, ChannelSize);

            // Set new value
            Hardware.Write(data);

            // Update properties
            ReadLed();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Reads the LED channels from the device, calculates then updates the related properties.
        /// </summary>
        private void ReadLed()
        {
            // Read channels together (BGR ordered)
            var data = Hardware.WriteReadBytes(ChannelStartAddress, ChannelSize * 3);

            // Update channel properties

            var blueChannel = Channels[LedBlueChannelIndex];
            blueChannel.Value.SetBytes(data);
            _ledBlue = ~blueChannel.Value.Length & 0xfff;

            var greenChannel = Channels[LedGreenChannelIndex];
            greenChannel.Value.SetBytes(data, ChannelSize);
            _ledGreen = ~greenChannel.Value.Length & 0xfff;

            var redChannel = Channels[LedRedChannelIndex];
            redChannel.Value.SetBytes(data, ChannelSize * 2);
            _ledRed = ~redChannel.Value.Length & 0xfff;
        }

        #endregion

        #region Common LED Interface

        // Map interface calls to instance specific members.
        // No XML documentation necessary because members behind explicitly implemented interfaces are not directly visible.
        bool INavioLedDevice.CanDisable => true;
        bool INavioLedDevice.CanSleep => true;
        bool INavioLedDevice.CanRestart => true;
        bool INavioLedDevice.Enabled { get { return OutputEnabled; } set { OutputEnabled = value; } }
        int INavioLedDevice.MaximumValue => NxpPca9685ChannelValue.Maximum;
        int INavioLedDevice.Red { get { return LedRed; } set { LedRed = value; } }
        int INavioLedDevice.Green { get { return LedGreen; } set { LedGreen = value; } }
        int INavioLedDevice.Blue { get { return LedBlue; } set { LedBlue = value; } }
        void INavioLedDevice.Read() { ReadLed(); }
        void INavioLedDevice.SetRgb(int red, int green, int blue) => SetLed(red, green, blue);

        #endregion

        #region Common PWM Interface

        // Map interface calls to instance specific members.
        // No XML documentation necessary because members behind explicitly implemented interfaces are not directly visible.
        bool INavioPwmDevice.CanDisable => true;
        bool INavioPwmDevice.CanSleep => true;
        bool INavioPwmDevice.CanRestart => true;
        bool INavioPwmDevice.FrequencyPerChannel => false;
        float INavioPwmDevice.FrequencyMinimum => FrequencyMaximum;
        float INavioPwmDevice.FrequencyMaximum => FrequencyMaximum;
        float INavioPwmDevice.LengthMinimum => PwmMsMinimum;
        float INavioPwmDevice.LengthMaximum => PwmMsMaximum;
        bool INavioPwmDevice.Enabled { get { return OutputEnabled; } set { OutputEnabled = value; } }
        Collection<PwmCycle> INavioPwmDevice.Channels
        {
            get
            {
                // TODO: Translate values or refactor original
                throw new NotImplementedException();
            }
        }
        void INavioPwmDevice.Read() { base.ReadAll(); }
        float INavioPwmDevice.SetFrequency(float frequency) { return WriteFrequency(frequency); }

        #endregion

        #region Events

        /// <summary>
        /// Updates the <see cref="LedRed"/> property when the related channel changed.
        /// </summary>
        /// <param name="sender">Sender, the channel which changed.</param>
        /// <param name="arguments">Standard event arguments, no specific data.</param>
        private void OnLedRedChannelChanged(object sender, EventArgs arguments)
        {
            var value = Channels[LedRedChannelIndex].Value;
            _ledRed = ~value.Length & 0xfff;
        }

        /// <summary>
        /// Updates the <see cref="LedGreen"/> property when the related channel changed.
        /// </summary>
        /// <param name="sender">Sender, the channel which changed.</param>
        /// <param name="arguments">Standard event arguments, no specific data.</param>
        private void OnLedGreenChannelChanged(object sender, EventArgs arguments)
        {
            var value = Channels[LedGreenChannelIndex].Value;
            _ledGreen = ~value.Length & 0xfff;
        }

        /// <summary>
        /// Updates the <see cref="LedBlue"/> property when the related channel changed.
        /// </summary>
        /// <param name="sender">Sender, the channel which changed.</param>
        /// <param name="arguments">Standard event arguments, no specific data.</param>
        private void OnLedBlueChannelChanged(object sender, EventArgs arguments)
        {
            var value = Channels[LedBlueChannelIndex].Value;
            _ledBlue = ~value.Length & 0xfff;
        }

        #endregion
    }
}
