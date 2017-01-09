using Emlid.WindowsIot.Hardware.Protocols.Pwm;
using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace Emlid.WindowsIot.Tests.NavioHardwareTestApp.Models.Shared
{
    /// <summary>
    /// <see cref="PwmPulse"/> converter allowing PWM values to be converted to any compatible numeric type.
    /// </summary>
    /// <remarks>
    /// The <see cref="PwmPulse.Width"/> is used as the value.
    /// The converter parameter must be set to the <see cref="PwmPulse.Frequency"/> to support conversion back.
    /// </remarks>
    public class PwmPulseValueConverter : IValueConverter
    {
        /// <summary>
        /// Converts <see cref="PwmPulse.Width"/> values to any compatible numeric type.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Ignore nulls and non-PWM types
            if (ReferenceEquals(value, null) || !(value is PwmPulse))
                return null;

            // Get PWM cycle (binding source)
            var pulse = (PwmPulse)value;

            // Convert width to target type then return
            return System.Convert.ChangeType(pulse.Width, targetType, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts any compatible numeric type to a <see cref="PwmPulse.Width"/>.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // Validate
            if (targetType != typeof(PwmPulse)) throw new InvalidOperationException();
            if (ReferenceEquals(parameter, null)) throw new ArgumentNullException(nameof(parameter));

            // Ignore null values
            if (ReferenceEquals(value, null))
                return null;

            // Get width of PWM pulse from binding source
            var width = (decimal)System.Convert.ChangeType(value, typeof(decimal), CultureInfo.InvariantCulture);

            // Get frequency of PWM pulse from binding parameter (required to calculate back to a whole PWM cycle)
            var frequency = (int)System.Convert.ChangeType(parameter, typeof(int), CultureInfo.InvariantCulture);

            // Convert to pulse then return
            return PwmPulse.FromWidth(frequency, width);
        }
    }
}
