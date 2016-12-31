using Emlid.WindowsIot.Common;
using Emlid.WindowsIot.Hardware.Protocols.Pwm;
using System;

namespace Emlid.WindowsIot.Hardware.Boards.Navio
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
        /// <param name="pwmFrequency">
        /// Some PWM devices do not tolerate high values and could be damaged if this is set too high,
        /// e.g. analog servos operate at much lower frequencies than digital servos.
        /// See <see cref="PwmCycle.ServoSafeFrequency"/> for more information.
        /// </param>
        public Navio2Board(float pwmFrequency = PwmCycle.ServoSafeFrequency)
        {
            // Initialize components
            Ms5611 = new NavioBarometerDevice();
            GpioLed = new Navio2LedDevice();

            // TODO: Implement Navio 2 PWM
            if (pwmFrequency > 0) { /* Ignore unused parameter warning. */ ; }
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
            GpioLed?.Dispose();
        }

        #endregion

        #endregion

        #region Public Properties

        #region Common Interface

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
        public INavioBarometerDevice Barometer => Ms5611;

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
        public INavioLedDevice Led => GpioLed;

        /// <summary>
        /// PWM device.
        /// </summary>
        public INavioPwmDevice Pwm { get; private set; }

        /// <summary>
        /// RC input device.
        /// </summary>
        /// <remarks>
        /// Not really hardware; only software PWM decoding over a GPIO pin on Navio and Navio plus boards.
        /// Latencies and inaccuracies possible due to software overhead.
        /// </remarks>
        public INavioRCInputDevice RCInput { get; private set; }

        #endregion

        #region Model Specific

        /// <summary>
        /// Model specific MS5611 chip which provides <see cref="Barometer"/> functionality.
        /// </summary>
        public NavioBarometerDevice Ms5611 { get; private set; }

        /// <summary>
        /// Model specific LED chip with RGB components connected to GPIO pins.
        /// </summary>
        public Navio2LedDevice GpioLed { get; private set; }

        // TODO: Implement Navio 2 specific devices

        #endregion

        #endregion
    }
}
