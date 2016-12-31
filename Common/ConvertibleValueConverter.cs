using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace Emlid.WindowsIot.Common
{
    /// <summary>
    /// <see cref="IConvertible"/> two-way value converter.
    /// </summary>
    /// <remarks>
    /// Enables values to be used in data binding which are already convertible
    /// via <see cref="System.Convert"/>.
    /// </remarks>
    public class ConvertibleValueConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ChangeType(value, targetType, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object. This method is called only in TwoWay bindings.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ChangeType(value, targetType, CultureInfo.CurrentCulture);
        }
    }
}
