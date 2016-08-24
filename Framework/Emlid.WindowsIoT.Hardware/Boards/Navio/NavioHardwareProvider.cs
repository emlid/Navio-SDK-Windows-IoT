using Microsoft.IoT.Lightning.Providers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Devices;
using Windows.Devices.Gpio;
using Windows.Devices.I2c;
using Windows.Devices.Spi;

namespace Emlid.WindowsIot.Hardware.Boards.Navio
{
    /// <summary>
    /// Hardware configuration and device factory.
    /// </summary>
    /// <remarks>
    /// Besides ensuring we only configure the hardware once and providing generic device initialization,
    /// we are also centralizing hardware initialization code so it can easily be upgraded later.
    /// </remarks>
    [CLSCompliant(false)]
    public static class NavioHardwareProvider
    {
        #region Private Fields

        /// <summary>
        /// Thread synchronization object.
        /// </summary>
        private static object _lock = new object();

        /// <summary>
        /// Configuration flag.
        /// </summary>
        private static bool _initialized;

        /// <summary>
        /// Caches the <see cref="LightningProvider.IsLightningEnabled"/> value.
        /// </summary>
        private static bool _lightningEnabled;

        #endregion

        #region Public Properties

        /// <summary>
        /// GPIO controllers.
        /// </summary>
        public static IReadOnlyList<GpioController> Gpio { get; private set; }

        /// <summary>
        /// I2C controllers.
        /// </summary>
        public static IReadOnlyList<I2cController> I2c { get; private set; }

        /// <summary>
        /// SPI controllers.
        /// </summary>
        public static IReadOnlyList<SpiController> Spi { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Configures the <see cref="LightningProvider"/> when enabled.
        /// </summary>
        /// <remarks>
        /// Not thread safe, must be called in a thread safe context.
        /// </remarks>
        public static void Initialize()
        {
            // Do nothing when already configured
            if (_initialized)
                return;
            lock (_lock)
            {
                // Thread-safe double-check lock
                if (_initialized)
                    return;

                // Set the Lightning Provider as the default if Lightning driver is enabled on the target device
                _lightningEnabled = LightningProvider.IsLightningEnabled;
                if (_lightningEnabled)
                {
                    LowLevelDevicesController.DefaultProvider = LightningProvider.GetAggregateProvider();

                    // Add multiple controllers from new lightning provider
                    Gpio = GpioController.GetControllersAsync(LightningGpioProvider.GetGpioProvider()).AsTask().GetAwaiter().GetResult();
                    I2c = I2cController.GetControllersAsync(LightningI2cProvider.GetI2cProvider()).AsTask().GetAwaiter().GetResult();
                    Spi = SpiController.GetControllersAsync(LightningSpiProvider.GetSpiProvider()).AsTask().GetAwaiter().GetResult();
                }
                else
                {
                    // Add single instance providers from old inbox driver
                    Gpio = new ReadOnlyCollection<GpioController>(new[] { GpioController.GetDefault() });
                    I2c = new ReadOnlyCollection<I2cController>(new[] { I2cController.GetDefaultAsync().AsTask().GetAwaiter().GetResult() });
                    Spi = new ReadOnlyCollection<SpiController>(new[] { SpiController.GetDefaultAsync().AsTask().GetAwaiter().GetResult() });
                }

                // Flag initialized
                _initialized = true;
            }
        }

        /// <summary>
        /// Connects to an I2C device if it exists.
        /// </summary>
        /// <param name="controllerIndex">Controller index.</param>
        /// <param name="address">
        /// I2C slave address of the chip.
        /// This is a physical property, not a software option.
        /// </param>
        /// <param name="fast">
        /// Set true for I2C <see cref="I2cBusSpeed.FastMode"/> or false for <see cref="I2cBusSpeed.StandardMode"/>.
        /// </param>
        /// <param name="exclusive">
        /// Set true for I2C <see cref="I2cSharingMode.Exclusive"/> or false for <see cref="I2cSharingMode.Shared"/>.
        /// </param>
        /// <returns>Device when controller and device exist, otherwise null.</returns>
        public static I2cDevice ConnectI2c(int controllerIndex, int address, bool fast = true, bool exclusive = true)
        {
            // Validate
            if (controllerIndex < 0) throw new ArgumentOutOfRangeException(nameof(controllerIndex));

            // Initialize
            Initialize();

            // Get controller (return null when doesn't exist)
            if (I2c.Count < controllerIndex + 1)
                return null;
            var controller = I2c[controllerIndex];

            // Connect to device and return (if exists)
            var settings = new I2cConnectionSettings(address)
            {
                BusSpeed = fast ? I2cBusSpeed.FastMode : I2cBusSpeed.StandardMode,
                SharingMode = exclusive ? I2cSharingMode.Exclusive : I2cSharingMode.Shared
            };
            return controller.GetDevice(settings);
        }

        /// <summary>
        /// Connects to a GPIO pin if it exists.
        /// </summary>
        /// <param name="controllerIndex">Controller index.</param>
        /// <param name="pinNumber">Pin number.</param>
        /// <param name="driveMode">Drive mode.</param>
        /// <param name="exclusive">
        /// Set true for I2C <see cref="GpioSharingMode.Exclusive"/> or false for <see cref="GpioSharingMode.SharedReadOnly"/>.
        /// </param>
        /// <returns>Pin when controller and device exist, otherwise null.</returns>
        public static GpioPin ConnectGpio(int controllerIndex, int pinNumber, GpioPinDriveMode driveMode, bool exclusive)
        {
            // Validate
            if (controllerIndex < 0) throw new ArgumentOutOfRangeException(nameof(controllerIndex));

            // Initialize
            Initialize();

            // Get controller (return null when doesn't exist)
            if (Gpio.Count < controllerIndex + 1)
                return null;
            var controller = Gpio[controllerIndex];

            // Connect to device (return null when doesn't exist)
            var pin = controller.OpenPin(pinNumber, exclusive ? GpioSharingMode.Exclusive : GpioSharingMode.SharedReadOnly);
            if (pin == null)
                return null;

            // Configure and return pin
            if (pin.GetDriveMode() != driveMode)
                pin.SetDriveMode(driveMode);
            return pin;
        }

        #endregion
    }
}
