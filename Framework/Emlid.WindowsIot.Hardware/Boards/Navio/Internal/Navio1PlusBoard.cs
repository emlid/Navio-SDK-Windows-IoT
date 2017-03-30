using Emlid.UniversalWindows;
using System;

namespace Emlid.WindowsIot.Hardware.Boards.Navio.Internal
{
    /// <summary>
    /// Navio+ hardware board.
    /// </summary>
    /// <remarks>
    /// Encapsulates common initialization and access to all components on this hardware model.
    /// </remarks>
    public sealed class Navio1PlusBoard : DisposableObject, INavioBoard
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        public Navio1PlusBoard()
        {
            try
            {
                // Initialize components
                _barometerDevice = new NavioBarometerDevice();
                _framDevice = new Navio1FramDevice(NavioHardwareModel.Navio1Plus);
                _ledPwmDevice = new Navio1LedPwmDevice();
                _rcInputDevice = new Navio1RCInputDevice();
            }
            catch
            {
                // Close devices in case partially initialized
                _barometerDevice?.Dispose();
                _framDevice?.Dispose();
                _ledPwmDevice?.Dispose();
                _rcInputDevice?.Dispose();

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
            _framDevice?.Dispose();
            _ledPwmDevice?.Dispose();
            _rcInputDevice?.Dispose();
        }

        #endregion

        #endregion

        #region Private Fields

        /// <summary>
        /// Model specific MS5611 chip which provides <see cref="Barometer"/> functionality.
        /// </summary>
        NavioBarometerDevice _barometerDevice;

        /// <summary>
        /// Model specific MB85RC256V chip which provides <see cref="Fram"/> functionality.
        /// </summary>
        Navio1FramDevice _framDevice;

        /// <summary>
        /// Model specific PCA9685 chip which provides <see cref="Led"/> and <see cref="Pwm"/> functionality.
        /// </summary>
        Navio1LedPwmDevice _ledPwmDevice;

        /// <summary>
        /// Model specific GPIO software PWM decoding, providing the <see cref="RCInput"/> functionality.
        /// </summary>
        Navio1RCInputDevice _rcInputDevice;

        #endregion

        #region Public Properties

        /// <summary>
        /// Hardware model.
        /// </summary>
        public NavioHardwareModel Model { get { return NavioHardwareModel.Navio1Plus; } }

        /// <summary>
        /// ADC device.
        /// </summary>
        public INavioAdcDevice Adc => null; // TODO: Implement ADC support

        /// <summary>
        /// Barometric pressure and temperature sensor.
        /// </summary>
        public INavioBarometerDevice Barometer { get { return _barometerDevice; } }

        /// <summary>
        /// Ferroelectric RAM device.
        /// </summary>
        public INavioFramDevice Fram { get { return _framDevice; } }

        /// <summary>
        /// GPS device.
        /// </summary>
        public INavioGpsDevice Gps => null; // TODO: Implement GPS support

        /// <summary>
        /// Primary IMU device.
        /// </summary>
        public INavioImuDevice Imu1 => null;   // TODO: Implement IMU support

        /// <summary>
        /// Secondary IMU device is always null because the Navio+ only has one IMU.
        /// </summary>
        public INavioImuDevice Imu2 => null;

        /// <summary>
        /// LED device.
        /// </summary>
        public INavioLedDevice Led { get { return _ledPwmDevice; } }

        /// <summary>
        /// PWM device.
        /// </summary>
        public INavioPwmDevice Pwm { get { return _ledPwmDevice; } }

        /// <summary>
        /// RC input device.
        /// </summary>
        /// <remarks>
        /// Not really hardware; only software PWM decoding over a GPIO pin on Navio and Navio boards.
        /// Latencies and inaccuracies possible due to software overhead.
        /// </remarks>
        public INavioRCInputDevice RCInput { get { return _rcInputDevice; } }

        #endregion
    }
}
