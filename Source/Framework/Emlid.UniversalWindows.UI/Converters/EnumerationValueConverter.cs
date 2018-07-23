using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace Emlid.UniversalWindows.UI.Converters
{
    /// <summary>
    /// Enumeration two-way value converter.
    /// </summary>
    /// <remarks>
    /// Converts enumeration values to their underlying type's value, e.g. enumeration value to integer.
    /// Converts to enumeration values by doing nothing but pass the value through,
    /// allowing the runtime to implicitly cast back to the enumeration value.
    /// </remarks>
    public class EnumerationValueConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Validate
            if (ReferenceEquals(value, null))
                throw new ArgumentNullException(nameof(value));

            // Convert to underlying enumeration value type
            var valueType = Enum.GetUnderlyingType(value.GetType());
            return System.Convert.ChangeType(value, valueType, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object. This method is called only in TwoWay bindings.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // Validate
            if (ReferenceEquals(value, null))
                throw new ArgumentNullException(nameof(value));
            if (!Enum.IsDefined(targetType, value))
                throw new ArgumentOutOfRangeException(value.ToString());

            // No conversion necessary as should cast directly to enumeration type
            return value;
        }
    }
}
