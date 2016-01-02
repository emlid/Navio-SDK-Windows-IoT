using System;
using System.Globalization;

namespace Emlid.WindowsIot.Hardware
{
    /// <summary>
    /// Contains a (pressure and temperature) measurement of the <see cref="Ms5611Device"/> barometer.
    /// </summary>
    public struct Ms5611Measurement
    {
        #region Constants

        /// <summary>
        /// Minimum pressure measurement in millibars.
        /// </summary>
        public const double PressureMin = 10;

        /// <summary>
        /// Maximum pressure measurement in millibars.
        /// </summary>
        public const double PressureMax = 1200;

        /// <summary>
        /// Minimum pressure measurement in millibars.
        /// </summary>
        public const double TemperatureMin = -40;

        /// <summary>
        /// Maximum pressure measurement in millibars.
        /// </summary>
        public const double TemperatureMax = 85;

        /// <summary>
        /// Accuracy of the temperature and pressure measurements.
        /// </summary>
        public const double Accuracy = 0.01;

        /// <summary>
        /// Empty value.
        /// </summary>
        public static readonly Ms5611Measurement Zero = new Ms5611Measurement(0, 0);

        #endregion

        #region Liftime

        /// <summary>
        /// Creates an instance with the specified values.
        /// </summary>
        public Ms5611Measurement(double pressure, double temperature)
        {
            Pressure = pressure;
            Temperature = temperature;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Pressure in millibars.
        /// </summary>
        public double Pressure { get; set; }

        /// <summary>
        /// Temperature in celcius.
        /// </summary>
        /// <remarks>
        /// The temperature will be higher than outside because
        /// it is heated by other components.
        /// </remarks>
        public double Temperature { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string representation of the current contents,
        /// e.g. "Pressure: 1013.43155085426 Temperature:36.3892484283447".
        /// </summary>
        public override string ToString()
        {
            return String.Format(CultureInfo.CurrentCulture, 
                new Resources.Strings().MS5611MeasurementStringFormat, Pressure, Temperature);
        }

        #endregion
    }
}
