using Emlid.WindowsIot.Hardware.Protocols.Barometer;
using System;

namespace Emlid.WindowsIot.Hardware.Boards.Navio
{
    /// <summary>
    /// Navio barometer device interface.
    /// </summary>
    public interface INavioBarometerDevice
    {
        #region Properties

        /// <summary>
        /// Last measurement taken from the device.
        /// </summary>
        BarometerMeasurement Measurement { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Resets the device and clears current measurements.
        /// </summary>
        void Reset();

        /// <summary>
        /// Performs calculation on the device then fires the <see cref="MeasurementUpdated"/> event.
        /// </summary>
        BarometerMeasurement Update();

        #endregion

        #region Events

        /// <summary>
        /// Fired after a new measurement is calculated.
        /// </summary>
        event EventHandler<BarometerMeasurement> MeasurementUpdated;

        #endregion
    }
}
