using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Emlid.WindowsIot.Hardware.System
{
    /// <summary>
    /// Extensions for work with GPIO devices.
    /// </summary>
    [CLSCompliant(false)]
    public static class GpioExtensions
    {
        /// <summary>
        /// Connects to a GPIO pin if it exists.
        /// </summary>
        /// <param name="busNumber">Bus controller number, zero based.</param>
        /// <param name="pinNumber">Pin number.</param>
        /// <param name="driveMode">Drive mode.</param>
        /// <param name="sharingMode">Sharing mode.</param>
        /// <returns>Pin when controller and device exist, otherwise null.</returns>
        public async static Task<GpioPin> Connect(int busNumber, int pinNumber,
            GpioPinDriveMode driveMode = GpioPinDriveMode.Input,
            GpioSharingMode sharingMode = GpioSharingMode.Exclusive)
        {
            // Validate
            if (busNumber < 0) throw new ArgumentOutOfRangeException(nameof(busNumber));
            if (pinNumber < 0) throw new ArgumentOutOfRangeException(nameof(pinNumber));

            // Get controller (return null when doesn't exist)
            var controllers = new List<GpioController> { await GpioController.GetDefaultAsync() };
            // TODO: support multiple controllers (after lightning)
            if (busNumber >= controllers.Count)
                throw new ArgumentOutOfRangeException(nameof(busNumber));
            var controller = controllers[busNumber];

            // Connect to device (return null when doesn't exist)
            var pin = controller.OpenPin(pinNumber, sharingMode);
            if (pin == null)
                return null;
            try
            {
                // Configure and return pin
                if (pin.GetDriveMode() != driveMode)
                    pin.SetDriveMode(driveMode);
                return pin;
            }
            catch
            {
                // Free pin on error during initialization
                pin?.Dispose();

                // Continue error
                throw;
            }
        }
    }
}
