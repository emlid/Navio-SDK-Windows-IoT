using CodeForDevices.WindowsUniversal.Hardware.Buses;
using CodeForDotNet;
using System;
using System.Diagnostics.CodeAnalysis;
using Windows.Devices.Gpio;

namespace Emlid.WindowsIot.Hardware.Boards.Navio.Internal
{
    /// <summary>
    /// Navio 2 LED device, three GPIO pins for RGB components of an LED.
    /// </summary>
    [SuppressMessage("Microsoft.Usage", "CA2208", Justification = "Exception member names should be named like the property to better describe the fault.")]
    public sealed class Navio2LedDevice : DisposableObject, INavioLedDevice
    {
        #region Constants

        /// <summary>
        /// GPIO controller index for the LED pins on the Navio board.
        /// </summary>
        public const int GpioControllerIndex = 0;

        /// <summary>
        /// GPIO pin number of the red LED component.
        /// </summary>
        public const int RedGpioPin = 4;

        /// <summary>
        /// GPIO pin number of the green LED component.
        /// </summary>
        public const int GreenGpioPin = 27;

        /// <summary>
        /// GPIO pin number of the blue LED component.
        /// </summary>
        public const int BlueGpioPin = 6;

        #endregion Constants

        #region Lifetime

        /// <summary>
        /// Creates an instance and read current values.
        /// </summary>
        public Navio2LedDevice()
        {
            try
            {
                // Open pins
                _redPin = GpioExtensions.Connect(GpioControllerIndex, 4, GpioPinDriveMode.Output, GpioSharingMode.Exclusive);
                _greenPin = GpioExtensions.Connect(GpioControllerIndex, 27, GpioPinDriveMode.Output, GpioSharingMode.Exclusive);
                _bluePin = GpioExtensions.Connect(GpioControllerIndex, 6, GpioPinDriveMode.Output, GpioSharingMode.Exclusive);

                // Read current values
                Read();
            }
            catch
            {
                // Close devices in case partially intialized
                _redPin?.Dispose();
                _greenPin?.Dispose();
                _bluePin?.Dispose();

                // Continue error
                throw;
            }
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

            // Close devices
            _redPin?.Dispose();
            _greenPin?.Dispose();
            _bluePin?.Dispose();
        }

        #endregion IDisposable

        #endregion Lifetime

        #region Private Fields

        /// <summary>
        /// Thread synchronization.
        /// </summary>
        private static readonly object _lock = new object();

        /// <summary>
        /// LED red component GPIO pin.
        /// </summary>
        private GpioPin _redPin;

        /// <summary>
        /// LED green component GPIO pin.
        /// </summary>
        private GpioPin _greenPin;

        /// <summary>
        /// LED blue component GPIO pin.
        /// </summary>
        private GpioPin _bluePin;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Returns true because even though the Navio 2 GPIO based LED has no controller to disable,
        /// we simulate the disabled state by setting all pins high (black = off) without updating
        /// the local RGB values, so they are restored when enabled.
        /// </summary>
        public bool CanDisable => true;

        /// <summary>
        /// Simulates enabling or disabling the LED by setting it to black (RGB components all off).
        /// The Navio 2 GPIO based LED has no controller to disable.
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                // Thread-safe lock
                lock (_lock)
                {
                    // Do nothing when value not changed
                    if (value == _enabled) return;

                    // Enable or disable
                    if (value)
                    {
                        // Enable: reset all pins to their current (last read) RGB values
                        _redPin.Write(ConvertToGpioValue(_red));
                        _greenPin.Write(ConvertToGpioValue(_green));
                        _bluePin.Write(ConvertToGpioValue(_blue));
                    }
                    else
                    {   // Disable: set all pins high (off)
                        _redPin.Write(GpioPinValue.High);
                        _greenPin.Write(GpioPinValue.High);
                        _bluePin.Write(GpioPinValue.High);
                    }
                    _enabled = value;
                }
            }
        }

        private bool _enabled;

        /// <summary>
        /// Returns 1 because the Navio 2 GPIO based LED can has no hardware PWM capability
        /// so can only be switched on (1) or off (0).
        /// </summary>
        public int MaximumValue => 1;

        /// <summary>
        /// Gets or sets the red LED component (1 = on, 0 = off).
        /// </summary>
        public int Red
        {
            get { return _red; }
            set
            {
                // Thread-safe lock
                lock (_lock)
                {
                    // Validate
                    if (value > MaximumValue) throw new ArgumentOutOfRangeException(nameof(Red));

                    // Set pin value
                    var gpioValue = ConvertToGpioValue(value);
                    _redPin.Write(gpioValue);

                    // Update property
                    _red = value;
                }
            }
        }

        private int _red;

        /// <summary>
        /// Gets or sets the green LED component (1 = on, 0 = off).
        /// </summary>
        public int Green
        {
            get { return _green; }
            set
            {
                // Thread-safe lock
                lock (_lock)
                {
                    // Validate
                    if (value > MaximumValue) throw new ArgumentOutOfRangeException(nameof(Green));

                    // Set pin value
                    var gpioValue = ConvertToGpioValue(value);
                    _greenPin.Write(gpioValue);

                    // Update property
                    _green = value;
                }
            }
        }

        private int _green;

        /// <summary>
        /// Gets or sets the blue LED component (1 = on, 0 = off).
        /// </summary>
        public int Blue
        {
            get { return _blue; }
            set
            {
                // Thread-safe lock
                lock (_lock)
                {
                    // Validate
                    if (value > MaximumValue) throw new ArgumentOutOfRangeException(nameof(Blue));

                    // Set pin value
                    var gpioValue = ConvertToGpioValue(value);
                    _bluePin.Write(gpioValue);

                    // Update property
                    _blue = value;
                }
            }
        }

        private int _blue;

        #endregion Public Properties

        #region Public Methods

        #region Conversion

        /// <summary>
        /// Returns the GPIO pin value for an LED component value.
        /// </summary>
        /// <param name="value">LED component value.</param>
        /// <returns>GPIO pin value.</returns>
        [CLSCompliant(false)]
        public static GpioPinValue ConvertToGpioValue(int value)
        {
            return value > 0 ? GpioPinValue.Low : GpioPinValue.High;
        }

        /// <summary>
        /// Returns the LED component value for a GPIO change event edge.
        /// </summary>
        /// <param name="edge">GPIO event edge.</param>
        /// <returns>LED component value.</returns>
        [CLSCompliant(false)]
        public static int ConvertToLedValue(GpioPinEdge edge)
        {
            return edge == GpioPinEdge.RisingEdge ? 0 : 1;
        }

        /// <summary>
        /// Returns the LED component value for a GPIO pin value.
        /// </summary>
        /// <param name="value">GPIO pin value.</param>
        /// <returns>LED component value.</returns>
        [CLSCompliant(false)]
        public static int ConvertToLedValue(GpioPinValue value)
        {
            return value == GpioPinValue.High ? 0 : 1;
        }

        #endregion Conversion

        #region LED Interface

        /// <summary>
        /// Clears all values.
        /// </summary>
        public void Reset()
        {
            SetRgb(0, 0, 0);
        }

        /// <summary>
        /// Reads the LED values from the device then updates the related properties.
        /// </summary>
        public void Read()
        {
            // Thread-safe lock
            lock (_lock)
            {
                _red = ConvertToLedValue(_redPin.Read());
                _green = ConvertToLedValue(_greenPin.Read());
                _blue = ConvertToLedValue(_bluePin.Read());
            }
        }

        /// <summary>
        /// Sets the LED <see cref="Red"/>, <see cref="Green"/> and <see cref="Blue"/> values together (in one operation).
        /// </summary>
        /// <param name="red">Red value in the range 0-<see cref="MaximumValue"/>.</param>
        /// <param name="green">Green value in the range 0-<see cref="MaximumValue"/>.</param>
        /// <param name="blue">Blue value in the range 0-<see cref="MaximumValue"/>.</param>
        public void SetRgb(int red, int green, int blue)
        {
            // Thread-safe lock
            lock (_lock)
            {
                // Write GPIO pin values
                _redPin.Write(ConvertToGpioValue(red));
                _greenPin.Write(ConvertToGpioValue(green));
                _bluePin.Write(ConvertToGpioValue(blue));

                // Update local values
                _red = red;
                _green = green;
                _blue = blue;
            }
        }

        #endregion LED Interface

        #endregion Public Methods
    }
}
