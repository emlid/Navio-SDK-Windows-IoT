using Emlid.WindowsIot.Hardware.Components.Ms5611;
using System;

namespace Emlid.WindowsIot.Hardware.Boards.Navio
{
    /// <summary>
    /// Navio barometric pressure and temperature sensor (MS5611 hardware device), connected via I2C.
    /// </summary>
    public sealed class NavioBarometerDevice : Ms5611Device
    {
        #region Constants

        /// <summary>
        /// I2C controller index of the chip on the Navio board.
        /// </summary>
        public const int I2cControllerIndex = 0;

        /// <summary>
        /// I2C address of the MS5611 on the Navio board.
        /// </summary>
        public const int I2cAddress = 0x77;

        /// <summary>
        /// Over-Sampling Rate to use by default (maximum of 4096).
        /// </summary>
        public const Ms5611Osr DefaultOsr = Ms5611Osr.Osr4096;

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates an instance without any initialization.
        /// </summary>
        /// <remarks>
        /// It is necessary to call the <see cref="Ms5611Device.Reset"/> and <see cref="Ms5611Device.Update"/>
        /// before any pressure or temperature data can be read.
        /// </remarks>
        [CLSCompliant(false)]
        public NavioBarometerDevice()
            : base(NavioHardwareProvider.ConnectI2c(I2cControllerIndex, I2cAddress), DefaultOsr)
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Performs calculation then fires the <see cref="MeasurementUpdated"/> event.
        /// </summary>
        protected override void Calculate(int rawPressure, int rawTemperature)
        {
            // Perform calculation
            base.Calculate(rawPressure, rawTemperature);

            // Fire event
            MeasurementUpdated?.Invoke(this, Measurement);
        }

        #endregion

        #region Events

        /// <summary>
        /// Fired when a new measurement is calculated.
        /// </summary>
        public EventHandler<Ms5611Measurement> MeasurementUpdated;

        #endregion
    }
}
