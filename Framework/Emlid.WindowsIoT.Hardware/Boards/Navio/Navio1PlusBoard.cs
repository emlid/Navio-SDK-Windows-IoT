using Emlid.WindowsIot.Common;
using Emlid.WindowsIot.Hardware.Protocols.Pwm;
using System;

namespace Emlid.WindowsIot.Hardware.Boards.Navio
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
        /// <param name="pwmFrequency">
        /// Some PWM devices do not tolerate high values and could be damaged if this is set too high,
        /// e.g. analog servos operate at much lower frequencies than digital servos.
        /// See <see cref="PwmCycle.ServoSafeFrequency"/> for more information.
        /// </param>
        public Navio1PlusBoard(float pwmFrequency = PwmCycle.ServoSafeFrequency)
        {
            // Initialize components
            Ms5611 = new NavioBarometerDevice();
            Mb85rc256v = new NavioFramDevice(NavioHardwareModel.Navio1Plus);
            Pca9685 = NavioLedPwmDevice.Initialize(pwmFrequency);
            GpioRCInput = new NavioRCInputDevice();
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
            Ms5611?.Dispose();
            Mb85rc256v?.Dispose();
            Pca9685?.Dispose();
            GpioRCInput?.Dispose();
        }

        #endregion

        #endregion

        #region Public Properties

        #region Common Interface

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
        public INavioBarometerDevice Barometer { get { return Ms5611; } }

        /// <summary>
        /// Ferroelectric RAM device.
        /// </summary>
        public INavioFramDevice Fram { get { return Mb85rc256v; } }

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
        public INavioLedDevice Led { get { return Pca9685; } }

        /// <summary>
        /// PWM device.
        /// </summary>
        public INavioPwmDevice Pwm { get { return Pca9685; } }

        /// <summary>
        /// RC input device.
        /// </summary>
        /// <remarks>
        /// Not really hardware; only software PWM decoding over a GPIO pin on Navio and Navio boards.
        /// Latencies and inaccuracies possible due to software overhead.
        /// </remarks>
        public INavioRCInputDevice RCInput { get { return GpioRCInput; } }

        #endregion

        #region Model Specific

        /// <summary>
        /// Model specific MS5611 chip which provides <see cref="Barometer"/> functionality.
        /// </summary>
        public NavioBarometerDevice Ms5611 { get; private set; }

        /// <summary>
        /// Model specific MB85RC256V chip which provides <see cref="Fram"/> functionality.
        /// </summary>
        public NavioFramDevice Mb85rc256v { get; private set; }

        /// <summary>
        /// Model specific PCA9685 chip which provides <see cref="Led"/> and <see cref="Pwm"/> functionality.
        /// </summary>
        public NavioLedPwmDevice Pca9685 { get; private set; }

        /// <summary>
        /// Model specific GPIO software PWM decoding, providing the <see cref="RCInput"/> functionality.
        /// </summary>
        public NavioRCInputDevice GpioRCInput { get; private set; }

        #endregion

        #endregion
    }
}
