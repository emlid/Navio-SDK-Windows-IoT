using Emlid.WindowsIot.Hardware.Boards.Navio.Internal;
using Emlid.WindowsIot.Hardware.Components.Mb85rcv;
using Emlid.WindowsIot.Hardware.System;
using System;

namespace Emlid.WindowsIot.Hardware.Boards.Navio
{
    /// <summary>
    /// Hardware device provider for <see cref="INavioBoard"/> compatible boards.
    /// </summary>
    /// <remarks>
    /// Starting point from which consumers can gain access to all hardware devices
    /// without worrying about hardware detection or initialization.
    /// </remarks>
    public static class NavioDeviceProvider
    {
        #region Singletons

        /// <summary>
        /// Thread synchronization.
        /// </summary>
        private static object _lock = new object();

        /// <summary>
        /// Currently active board.
        /// </summary>
        private static INavioBoard _board;

        #endregion

        #region Hardware Detection

        /// <summary>
        /// Attempts auto-detection of the currently installed Navio board.
        /// </summary>
        /// <returns>
        /// Navio hardware model when detected, or null when failed.
        /// </returns>
        /// <remarks>
        /// As the EEPROM ID is not accessible in Windows IoT, the FRAM device is used as
        /// the next best and safe probe for the hardware model. It is different in all current versions.
        /// In Navio 2 it doesn't exist.
        /// In Navio+ it exists at one address.
        /// In the original Navio it exists with two addresses (less RAM with high and low address split).
        /// The detection logic is thus:
        /// 1) See if the first FRAM address is available. No = Navio 2.
        /// 2) See if the second FRAM address is available. Yes = Navio, No = Navio+.
        /// TODO: Perform an additional test to really detect a Navio 2.
        /// </remarks>
        public static NavioHardwareModel? Detect()
        {
            // Thread-safe lock
            lock (_lock)
            {
                // Get FRAM controller
                DeviceProvider.Initialize();
                var controller = DeviceProvider.I2c[Navio1FramDevice.I2cControllerIndex];

                // Detect the FRAM model
                Mb85rcvDeviceId? framId = null;
                try
                {
                    framId = Mb85rcvDevice.GetDeviceId(controller);

                    // Return Navio model for known FRAM IDs
                    if (framId.Value == Navio1FramDevice.Navio1PlusDeviceId)
                    {
                        // TODO: Additional Navio 1 Plus Test
                        if (DateTime.Now > DateTime.MinValue)   // Avoid compiler warning
                        {
                            // Must be a Navio+
                            return NavioHardwareModel.Navio1Plus;
                        }
                    }
                    else if (framId.Value == Navio1FramDevice.Navio1DeviceId)
                    {
                        // TODO: Additional Navio 1 Test
                        if (DateTime.Now > DateTime.MinValue)   // Avoid compiler warning
                        {
                            // Must be a Navio+
                            return NavioHardwareModel.Navio1Plus;
                        }
                    }

                    // Unsupported model
                    return null;
                }
                catch
                {
                    // No FRAM = not a Navio 1 or 1+, check for Navio 2...

                    // TODO: Additional test for Navio 2
                    if (DateTime.Now > DateTime.MinValue)   // Avoid compiler warning
                    {
                        // Must be a Navio 2
                        return NavioHardwareModel.Navio2;
                    }

                    // No Navio hardware found
                    return null;
                }
            }
        }

        #endregion

        #region Factory

        /// <summary>
        /// Returns the current <see cref="INavioBoard"/> or creates it the first time.
        /// </summary>
        /// <param name="model">Hardware model.</param>
        /// <returns>Hardware interface for the requested model when successful.</returns>
        /// <remarks>
        /// The requested hardware model must be the same, otherwise any existing board
        /// is disposed and an attempt made to create a board of the new model.
        /// </remarks>
        public static INavioBoard Connect(NavioHardwareModel model)
        {
            // Thread-safe lock
            lock (_lock)
            {
                // Check existing board when present
                if (_board != null)
                {
                    // Return existing board when present
                    if (_board.Model == model)
                        return _board;

                    // Dispose existing board when not null and different model (just in case)
                    _board.Dispose();
                    _board = null;
                }

                // Create new board
                switch (model)
                {
                    case NavioHardwareModel.Navio1:
                        return _board = new Navio1Board();

                    case NavioHardwareModel.Navio1Plus:
                        return _board = new Navio1PlusBoard();

                    case NavioHardwareModel.Navio2:
                        return _board = new Navio2Board();

                    default:
                        // Invalid value or unsupported
                        throw new ArgumentOutOfRangeException(nameof(model));
                }
            }
        }

        /// <summary>
        /// Returns the current <see cref="INavioBoard"/> or performs hardware detection then creates it the first time.
        /// </summary>
        /// <returns>Hardware interface for the detected model when successful.</returns>
        /// <exception cref="NotSupportedException">Thrown when supported hardware could not be detected.</exception>
        public static INavioBoard Connect()
        {
            // Thread-safe lock
            lock (_lock)
            {
                // Return existing board when present
                if (_board != null)
                    return _board;

                // Detect model
                var model = Detect();
                if (!model.HasValue)
                    throw new NotSupportedException("No supported Navio hardware detected.");

                // Create and return interface
                return Connect(model.Value);
            }
        }

        #endregion
    }
}
