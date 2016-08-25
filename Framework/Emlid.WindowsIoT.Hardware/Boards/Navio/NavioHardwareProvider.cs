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
        /// Connects to a GPIO pin if it exists.
        /// </summary>
        /// <param name="controllerIndex">Controller index.</param>
        /// <param name="pinNumber">Pin number.</param>
        /// <param name="driveMode">Drive mode.</param>
        /// <param name="sharingMode">Sharing mode.</param>
        /// <returns>Pin when controller and device exist, otherwise null.</returns>
        public static GpioPin ConnectGpio(int controllerIndex, int pinNumber, 
            GpioPinDriveMode driveMode = GpioPinDriveMode.Input, GpioSharingMode sharingMode = GpioSharingMode.Exclusive)
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
            var pin = controller.OpenPin(pinNumber, sharingMode);
            if (pin == null)
                return null;

            // Configure and return pin
            if (pin.GetDriveMode() != driveMode)
                pin.SetDriveMode(driveMode);
            return pin;
        }

        /// <summary>
        /// Connects to an I2C device if it exists.
        /// </summary>
        /// <param name="controllerIndex">Controller index.</param>
        /// <param name="address">Slave address.</param>
        /// <param name="speed">Bus speed.</param>
        /// <param name="sharingMode">Sharing mode.</param>
        /// <returns>Device when controller and device exist, otherwise null.</returns>
        public static I2cDevice ConnectI2c(int controllerIndex, int address, 
            I2cBusSpeed speed = I2cBusSpeed.FastMode, I2cSharingMode sharingMode = I2cSharingMode.Exclusive)
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
                BusSpeed = speed,
                SharingMode = sharingMode
            };
            return controller.GetDevice(settings);
        }

        /// <summary>
        /// Connects to an SPI device if it exists.
        /// </summary>
        /// <param name="controllerIndex">Controller index.</param>
        /// <param name="chipSelectLine">Slave Chip Select Line.</param>
        /// <param name="bits">Data length in bits.</param>
        /// <param name="frequency">Frequency in Hz.</param>
        /// <param name="mode">Communication mode, i.e. clock polarity.</param>
        /// <param name="sharingMode">Sharing mode.</param>
        /// <returns>Device when controller and device exist, otherwise null.</returns>
        public static SpiDevice ConnectSpi(int controllerIndex, int chipSelectLine, int frequency, int bits, 
            SpiMode mode, SpiSharingMode sharingMode = SpiSharingMode.Exclusive)
        {
            // Validate
            if (controllerIndex < 0) throw new ArgumentOutOfRangeException(nameof(controllerIndex));

            // Initialize
            Initialize();

            // Get controller (return null when doesn't exist)
            if (Spi.Count < controllerIndex + 1)
                return null;
            var controller = Spi[controllerIndex];

            // Connect to device and return (if exists)
            var settings = new SpiConnectionSettings(chipSelectLine)
            {
                ClockFrequency = frequency,
                DataBitLength = bits,
                Mode = mode,
                SharingMode = sharingMode
            };
            return controller.GetDevice(settings);
        }

        #endregion
    }
}
