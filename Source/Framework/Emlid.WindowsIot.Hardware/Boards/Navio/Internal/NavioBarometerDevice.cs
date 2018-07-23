using Emlid.UniversalWindows;
using Emlid.WindowsIot.Hardware.Components.Ms5611;
using Emlid.WindowsIot.Hardware.Protocols.Barometer;
using System;

namespace Emlid.WindowsIot.Hardware.Boards.Navio.Internal
{
    /// <summary>
    /// Navio barometric pressure and temperature sensor (MS5611 hardware device), connected via I2C.
    /// </summary>
    public sealed class NavioBarometerDevice : DisposableObject, INavioBarometerDevice
    {
        #region Constants

        /// <summary>
        /// I2C controller index of the chip on the Navio board.
        /// </summary>
        public const int I2cControllerIndex = 0;

        /// <summary>
        /// Chip Select Bit (CSB) of the MS5611 on the Navio board.
        /// </summary>
        public const bool ChipSelectBit = true;

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
        {
            // Connect to hardware
            _device = new Ms5611Device(I2cControllerIndex, ChipSelectBit, DefaultOsr);
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

            // Close device
            _device?.Dispose();
        }

        #endregion

        #endregion

        #region Private Fields

        /// <summary>
        /// Thread synchronization.
        /// </summary>
        private static object _lock = new object();

        /// <summary>
        /// Barometer device.
        /// </summary>
        private Ms5611Device _device;

        #endregion

        #region Public Properties

        /// <summary>
        /// Last measurement taken from the device.
        /// </summary>
        public BarometerMeasurement Measurement { get; private set; }

        #endregion

        #region Public Methods
        /// <summary>
        /// Resets the device and clears current measurements.
        /// </summary>
        public void Reset()
        {
            // Thread-safe lock
            lock (_lock)
            {
                // Reset device
                _device.Reset();

                // Clear measurement
                Measurement = new BarometerMeasurement();
            }
        }

        /// <summary>
        /// Performs calculation on the device then fires the <see cref="MeasurementUpdated"/> event.
        /// </summary>
        public BarometerMeasurement Update()
        {
            // Thread-safe lock
            lock (_lock)
            {
                // Perform calculation
                _device.Update();
                var measurement = new BarometerMeasurement(_device.Pressure, _device.Temperature);

                // Update property
                Measurement = measurement;

                // Fire event
                MeasurementUpdated?.Invoke(this, measurement);

                // Return result
                return measurement;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Fired after a new measurement is calculated.
        /// </summary>
        public event EventHandler<BarometerMeasurement> MeasurementUpdated;

        #endregion
    }
}
