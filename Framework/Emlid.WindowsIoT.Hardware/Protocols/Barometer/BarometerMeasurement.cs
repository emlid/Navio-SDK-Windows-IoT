using System.Globalization;

namespace Emlid.WindowsIot.Hardware.Protocols.Barometer
{
    /// <summary>
    /// Contains a (pressure and temperature) measurement of a barometer.
    /// </summary>
    public struct BarometerMeasurement
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance with the specified values.
        /// </summary>
        public BarometerMeasurement(double pressure, double temperature)
        {
            Pressure = pressure;
            Temperature = temperature;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Pressure in millibars.
        /// </summary>
        public double Pressure { get; set; }

        /// <summary>
        /// Temperature in celsius.
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
        /// e.g. "Pressure: 1013.43155085426mbar Temperature:36.3892484283447°c".
        /// </summary>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture,
                Resources.Strings.BarometerMeasurementStringFormat, Pressure, Temperature);
        }

        #endregion
    }
}
