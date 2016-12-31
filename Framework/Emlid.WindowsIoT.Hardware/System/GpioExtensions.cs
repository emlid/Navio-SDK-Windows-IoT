using System;
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
        /// Opens a GPIO pin and checks the drive mode.
        /// </summary>
        /// <param name="controller">GPIO controller.</param>
        /// <param name="pinNumber">GPIO pin number.</param>
        /// <param name="driveMode">Desired drive mode.</param>
        /// <param name="shareMode">Optional share mode, default is <see cref="GpioSharingMode.Exclusive"/>.</param>
        /// <returns>GPIO pin.</returns>
        public static GpioPin OpenPin(this GpioController controller, int pinNumber,
            GpioPinDriveMode driveMode, GpioSharingMode shareMode = GpioSharingMode.Exclusive)
        {
            // Open pin
            var pin = controller.OpenPin(pinNumber, shareMode);
            try
            {
                // Check drive mode when specified
                if (pin.GetDriveMode() != driveMode)
                    pin.SetDriveMode(driveMode);
            }
            catch
            {
                // Free pin on error during initialization
                pin?.Dispose();

                // Continue error
                throw;
            }

            // Return initialized pin
            return pin;
        }
    }
}
