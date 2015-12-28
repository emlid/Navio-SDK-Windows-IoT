using System;
using System.Globalization;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace Emlid.WindowsIot.Hardware
{
    /// <summary>
    /// Provides common functions and lifetime management for I2C connected hardware devices.
    /// </summary>
    public abstract class I2cConnectedDevice : IDisposable
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance at the specified I2C <paramref name="address"/> with custom settings.
        /// </summary>
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
        protected I2cConnectedDevice(int address, bool fast, bool exclusive)
        {
            // Validate
            if (address < 0 || address > Int16.MaxValue) throw new ArgumentOutOfRangeException(nameof(address));

            // Initialize hardware
            ControllerId = DiscoverI2cMasterId();
            Hardware = Connect(ControllerId, address, fast, exclusive);
            Id = Hardware.DeviceId;
        }

        #region IDisposable

        /// <summary>
        /// Indicates this instance has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Frees resources owned by this instance.
        /// </summary>
        /// <param name="disposing">
        /// True when called via <see cref="Dispose()"/>, false when called from the finalizer.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            // Do nothing when already disposed
            if (IsDisposed) return;

            // Dispose
            try
            {
                // Dispose managed resource during dispose
                if (disposing)
                {
                    Hardware.Dispose();
                }
            }
            finally
            {
                // Flag disposed
                IsDisposed = true;
            }
        }

        /// <summary>
        /// Finalizer which calls <see cref="Dispose(bool)"/> with false when it has not been disabled
        /// by a proactive call to <see cref="Dispose()"/>.
        /// </summary>
        ~I2cConnectedDevice()
        {
            // Partial dispose
            Dispose(false);
        }

        /// <summary>
        /// Proactively frees resources owned by this instance.
        /// </summary>
        public void Dispose()
        {
            try
            {
                // Full managed dispose
                Dispose(true);
            }
            finally
            {
                // Suppress finalizer (we already cleaned-up)
                GC.SuppressFinalize(this);
            }
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Caches the I2C device ID which must be auto-detected via plug-and-play.
        /// </summary>
        /// <remarks>
        /// Besides making start-up faster it also avoids initialization hangs.
        /// </remarks>
        private static string _i2cMasterId;

        #endregion

        #region Protected Properties

        /// <summary>
        /// I2C device and connection indicator. Set during the first call to <see cref="Connect"/>.
        /// </summary>
        [CLSCompliant(false)]
        protected I2cDevice Hardware { get; private set; }

        #endregion

        #region Public Properties

        /// <summary>
        /// Controller ID (I2C master ID).
        /// </summary>
        public string ControllerId { get; private set; }

        /// <summary>
        /// Device ID (I2C slave ID).
        /// </summary>
        public string Id { get; private set; }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates an I2C conection to the chip.
        /// </summary>
        /// <param name="deviceId">I2C master device ID.</param>
        /// <param name="address">I2C slave address of the chip.</param>
        /// <param name="fast">
        /// Set true for I2C <see cref="I2cBusSpeed.FastMode"/> or false for <see cref="I2cBusSpeed.StandardMode"/>.
        /// </param>
        /// <param name="exclusive">
        /// Set true for I2C <see cref="I2cSharingMode.Exclusive"/> or false for <see cref="I2cSharingMode.Shared"/>.
        /// </param>
        /// <returns>True when initialized, false when already initialized (but no error).</returns>
        /// <exception cref="Exception">Thrown when initialization failed.</exception>
        [CLSCompliant(false)]
        protected static I2cDevice Connect(string deviceId, int address, bool fast, bool exclusive)
        {
            // Connect to device
            var settings = new I2cConnectionSettings(address)
            {
                BusSpeed = fast ? I2cBusSpeed.FastMode : I2cBusSpeed.StandardMode,
                SharingMode = exclusive ? I2cSharingMode.Exclusive : I2cSharingMode.Shared
            };
            var device = I2cDevice.FromIdAsync(deviceId, settings).AsTask().GetAwaiter().GetResult();
            if (device == null)
            {
                // Initialization error
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                    new Resources.Strings().I2cErrorDeviceCannotOpen, settings.SlaveAddress, deviceId));
            }

            // Return result
            return device;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Performs Plug-and-Play detection of the I2C master device.
        /// </summary>
        /// <remarks>
        /// The ID is cached to speed-up any initialization and avoid potential Plug &amp; Play hangs.
        /// </remarks>
        /// <returns>I2C ID.</returns>
        public static string DiscoverI2cMasterId()
        {
            // Check cache
            if (_i2cMasterId != null)
                return _i2cMasterId;

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
            return _i2cMasterId = device.Id;
        }

        #endregion

        #endregion
    }
}
