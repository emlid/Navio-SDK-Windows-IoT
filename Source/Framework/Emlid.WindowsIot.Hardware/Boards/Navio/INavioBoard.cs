using System;

namespace Emlid.WindowsIot.Hardware.Boards.Navio
{
    /// <summary>
    /// Common hardware interface to all Navio devices.
    /// </summary>
    public interface INavioBoard : IDisposable
    {
        #region General

        /// <summary>
        /// Hardware model.
        /// </summary>
        NavioHardwareModel Model { get; }

        #endregion

        #region Devices

        /// <summary>
        /// ADC device.
        /// </summary>
        INavioAdcDevice Adc { get; }

        /// <summary>
        /// Barometer device.
        /// </summary>
        INavioBarometerDevice Barometer { get; }

        /// <summary>
        /// FRAM device.
        /// </summary>
        INavioFramDevice Fram { get; }

        /// <summary>
        /// GPS device.
        /// </summary>
        INavioGpsDevice Gps { get; }

        /// <summary>
        /// Primary IMU device.
        /// </summary>
        INavioImuDevice Imu1 { get; }

        /// <summary>
        /// Secondary IMU device.
        /// </summary>
        INavioImuDevice Imu2 { get; }

        /// <summary>
        /// LED device.
        /// </summary>
        INavioLedDevice Led { get; }

        /// <summary>
        /// PWM device.
        /// </summary>
        INavioPwmDevice Pwm { get; }

        /// <summary>
        /// RC input device.
        /// </summary>
        INavioRCInputDevice RCInput { get; }

        #endregion
    }
}
