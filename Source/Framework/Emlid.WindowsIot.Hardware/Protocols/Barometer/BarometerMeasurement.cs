﻿using System;
using System.Globalization;

namespace Emlid.WindowsIot.Hardware.Protocols.Barometer
{
    /// <summary>
    /// Contains a (pressure and temperature) measurement of a barometer.
    /// </summary>
    public struct BarometerMeasurement : IEquatable<BarometerMeasurement>
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

        #region Operators

        /// <summary>
        /// Tests two objects of this type for equality by value.
        /// </summary>
        public static bool operator ==(BarometerMeasurement left, BarometerMeasurement right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Tests two objects of this type for inequality by value.
        /// </summary>
        public static bool operator !=(BarometerMeasurement left, BarometerMeasurement right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Compares this object with another by value.
        /// </summary>
        /// <param name="something">Object with which to compare by value.</param>
        public override bool Equals(object something)
        {
            return (something is BarometerMeasurement other) && Equals(other);
        }

        /// <summary>
        /// Compares this object with another of the same type by value.
        /// </summary>
        /// <param name="other">Object with which to compare by value.</param>
        public bool Equals(BarometerMeasurement other)
        {
            return
                other.Pressure == Pressure &&
                other.Temperature == Temperature;
        }

        /// <summary>
        /// Returns a hash-code based on the current value of this object.
        /// </summary>
        public override int GetHashCode()
        {
            return
                Pressure.GetHashCode() ^
                Temperature.GetHashCode();
        }

        #endregion Operators

        #endregion Lifetime

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

        #endregion Public Properties

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

        #endregion Public Methods
    }
}