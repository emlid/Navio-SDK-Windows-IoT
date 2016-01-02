using System;

namespace Emlid.WindowsIot.Hardware
{
    /// <summary>
    /// Navio barometric pressure and temperature sensor (MS5611 hardware device), connected via I2C.
    /// </summary>
    public class NavioBarometerDevice : Ms5611Device
    {
        #region Constants

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
        public NavioBarometerDevice()
            : base(I2cAddress, true, true, DefaultOsr)
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
            if (MeasurementUpdated != null)
                MeasurementUpdated(this, Measurement);
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
