using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Devices.I2c;

namespace Emlid.WindowsIoT.Hardware
{
    /// <summary>
    /// Navio PCA9685 hardware device.
    /// </summary>
    /// <remarks>
    /// Navio uses the <see cref="NxpPca9685Device"/> as a dual-purpose PWM and LED driver,
    /// i.e. for servo control and the high intensity RGB status LED. It is connected via the I2C bus.
    /// See http://docs.emlid.com/Navio-dev/servo-and-rgb-led/ for more information.
    /// <seealso cref="NxpPca9685Device"/>
    /// </remarks>
    public class NavioPca9685Device : NxpPca9685Device
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
        public const float ServoFrequencyDefault = 50;

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates an instance with required mode bits set, but the device state and PWM values unchanged (supporting recovery).
        /// </summary>
        /// <remarks>
        /// <para>
        /// Initializing without restart allows the caller decide whether to recover or reset the device. 
        /// Before starting any output be sure to check the <see cref="NxpPca9685Device.Frequency"/>.
        /// </para>
        /// <para>
        /// To start with new settings, call <see cref="NxpPca9685Device.Clear"/>, <seealso cref="Restart"/> then set <see cref="OutputEnabled"/>.
        /// To resume, call <see cref="NxpPca9685Device.Wake"/> then set <see cref="OutputEnabled"/>.
        /// </para>
        /// </remarks>
        public NavioPca9685Device() : base(GetDeviceId(), I2cAddress, ExternalClockSpeed, true, true)
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
            Hardware.WriteBit((byte)NxpPca9685Register.Mode1, (byte)(NxpPca9685Mode1Bits.AutoIncrement | NxpPca9685Mode1Bits.AllCall), true);

            // Set "all call" address
            Hardware.WriteByte((byte)NxpPca9685Register.AllCall, (byte)I2cAllCallAddress);

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
        /// <see cref="NxpPca9685Device.Frequency"/> for important information.
        /// Use <see cref="ServoFrequencyDefault"/> to support most analog servos.
        /// </param>
        /// <param name="clear">Clears all LED/PWM values.</param>
        /// <param name="restart">
        /// Set true to restart the device which also enables the osciallator.
        /// When false, you have to call <see cref="NxpPca9685Device.Wake"/> or <see cref="Restart"/> later to enable the oscillator.
        /// </param>
        /// <param name="enable">
        /// Set true to enable output, or false to continue with further initialization before enabling output.
        /// When false, you have to set it later before any output can occur.
        /// Defaults to false to prevent unexpected behaviour from damaging hardware.
        /// </param>
        public static NavioPca9685Device Initialize(float frequency,
            bool clear = true, bool restart = true, bool enable = false)
        {
            // Create device
            var device = new NavioPca9685Device();

            // Disable output
            device.OutputEnabled = false;

            // Set frequency
            device.WriteFrequency(frequency);

            // Clear LED/PWM values when specified
            if (clear)
                device.Clear();

            // Restart when specified
            if (restart)
                device.Restart();

            // Enable output when specified
            if (enable)
                device.OutputEnabled = true;

            // Return initialized device
            return device;
        }

        #region IDisposable

        /// <summary>
        /// Frees resources owned by this instance.
        /// </summary>
        /// <param name="disposing">
        /// True when called via <see cref="NxpPca9685Device.Dispose()"/>, false when called from the finalizer.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            // Do nothing when already disposed
            if (IsDisposed)
                return;

            // Dispose
            try
            {
                // Dispose managed resource during dispose
                if (disposing)
                {
                    Sleep();
                    _outputEnablePin.Dispose();
                }
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
        /// Caches the I2C device ID.
        /// </summary>
        /// <remarks>
        /// Besides making start-up faster it also avoids hangs at start-up.
        /// </remarks>
        private static string _deviceId;

        /// <summary>
        /// GPIO output enable pin.
        /// </summary>
        private GpioPin _outputEnablePin;

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
        /// Use <see cref="ServoFrequencyDefault"/> for most analog servos. Digital servos support
        /// much faster update speeds, so if you have one read their specification and choose a
        /// sensible value (perhaps not maximum) for best performance without overheating.
        /// </remarks>
        public bool OutputEnabled
        {
            get { return _outputEnablePin.Read() == GpioPinValue.Low; }
            set { _outputEnablePin.Write(value ? GpioPinValue.Low : GpioPinValue.High); }
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
        /// Automatically inverts the value, providing a natural behaviour where a higher number
        /// produces a higher LED intensity. Normally, when written as raw PWM values, the output
        /// is inverted due to common anode.
        /// </remarks>
        /// <param name="red">Red value in the range 0-<see cref="NxpPca9685ChannelValue.Maximum"/>.</param>
        /// <param name="green">Green value in the range 0-<see cref="NxpPca9685ChannelValue.Maximum"/>.</param>
        /// <param name="blue">Blue value in the range 0-<see cref="NxpPca9685ChannelValue.Maximum"/>.</param>
        public void SetLed(int red, int green, int blue)
        {
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
        /// Performs Plug-and-Play detection of the I2C master device.
        /// </summary>
        /// <returns>Device ID.</returns>
        private static string GetDeviceId()
        {
            // Check cache
            if (_deviceId != null)
                return _deviceId;

            // Query device
            var query = I2cDevice.GetDeviceSelector();
            var devices = DeviceInformation.FindAllAsync(query).AsTask().GetAwaiter().GetResult();
            if (devices == null || devices.Count == 0)
            {
                // Not found error
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                    new Resources.Strings().I2cErrorDeviceNotFound, query));
            }
            var device = devices[0];

            // Cache and return result
            return _deviceId = device.Id;
        }

        /// <summary>
        /// Reads the LED channels from the device, calculates then updates the related properties.
        /// </summary>
        private void ReadLed()
        {
            // Read channels together (BGR ordered)
            var data = Hardware.ReadBytes(ChannelStartAddress, ChannelSize * 3);

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

        #region Events

        /// <summary>
        /// Updates the <see cref="LedRed"/> property when the related channel changed.
        /// </summary>
        private void OnLedRedChannelChanged(object sender, EventArgs e)
        {
            var value = Channels[LedRedChannelIndex].Value;
            _ledRed = ~value.Length & 0xfff;
        }

        /// <summary>
        /// Updates the <see cref="LedGreen"/> property when the related channel changed.
        /// </summary>
        private void OnLedGreenChannelChanged(object sender, EventArgs e)
        {
            var value = Channels[LedGreenChannelIndex].Value;
            _ledGreen = ~value.Length & 0xfff;
        }

        /// <summary>
        /// Updates the <see cref="LedBlue"/> property when the related channel changed.
        /// </summary>
        private void OnLedBlueChannelChanged(object sender, EventArgs e)
        {
            var value = Channels[LedBlueChannelIndex].Value;
            _ledBlue = ~value.Length & 0xfff;
        }

        #endregion
    }
}
