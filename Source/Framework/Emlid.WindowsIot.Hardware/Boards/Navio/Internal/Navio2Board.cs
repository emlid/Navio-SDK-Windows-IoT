using CodeForDotNet;
using System;

namespace Emlid.WindowsIot.Hardware.Boards.Navio.Internal
{
    /// <summary>
    /// Navio 2 hardware board.
    /// </summary>
    /// <remarks>
    /// Encapsulates common initialization and access to all components on this hardware model.
    /// </remarks>
    public sealed class Navio2Board : DisposableObject, INavioBoard
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        public Navio2Board()
        {
            try
            {
                // Initialize components
                _barometerDevice = new NavioBarometerDevice();
                _ledDevice = new Navio2LedDevice();
                _rcioDevice = new Navio2RcioDevice();
            }
            catch
            {
                // Close devices in case partially initialized
                _barometerDevice?.Dispose();
                _ledDevice?.Dispose();
                _rcioDevice?.Dispose();

                // Continue error
                throw;
            }
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

            // Dispose owned objects
            _barometerDevice?.Dispose();
            _ledDevice?.Dispose();
            _rcioDevice?.Dispose();
        }

        #endregion IDisposable

        #endregion Lifetime

        #region Private Fields

        /// <summary>
        /// MS5611 chip which provides <see cref="Barometer"/> functionality.
        /// </summary>
        private NavioBarometerDevice _barometerDevice;

        /// <summary>
        /// LED chip with RGB components connected to GPIO pins.
        /// </summary>
        private Navio2LedDevice _ledDevice;

        /// <summary>
        /// RCIO co-processor which acts as a proxy to the two IMUs, RC input and output.
        /// </summary>
        private Navio2RcioDevice _rcioDevice;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Hardware model.
        /// </summary>
        public NavioHardwareModel Model => NavioHardwareModel.Navio2;

        /// <summary>
        /// ADC device.
        /// </summary>
        public INavioAdcDevice Adc => null; // TODO: Implement ADC support

        /// <summary>
        /// Barometric pressure and temperature sensor.
        /// </summary>
        public INavioBarometerDevice Barometer => _barometerDevice;

        /// <summary>
        /// Ferroelectric RAM device.
        /// </summary>
        /// <remarks>
        /// No FRAM is available on Navio 2, returns null.
        /// </remarks>
        public INavioFramDevice Fram => null;

        /// <summary>
        /// GPS device.
        /// </summary>
        public INavioGpsDevice Gps => null; // TODO: Implement GPS support

        /// <summary>
        /// Primary IMU device.
        /// </summary>
        public INavioImuDevice Imu1 => null;   // TODO: Implement IMU support

        /// <summary>
        /// Secondary IMU device.
        /// </summary>
        public INavioImuDevice Imu2 => null;   // TODO: Implement IMU support

        /// <summary>
        /// LED device.
        /// </summary>
        public INavioLedDevice Led => _ledDevice;

        /// <summary>
        /// PWM device.
        /// </summary>
        public INavioPwmDevice Pwm { get; private set; }

        /// <summary>
        /// RC input device.
        /// </summary>
        public INavioRCInputDevice RCInput => _rcioDevice;

        #endregion Public Properties
    }
}