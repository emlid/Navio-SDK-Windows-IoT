using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace Emlid.WindowsIot.Common
{
    /// <summary>
    /// Enumeration value converter enables enumeration valies to be used in data binding.
    /// </summary>
    public class EnumValueConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Validate
            if (ReferenceEquals(value, null))
                throw new ArgumentNullException("value");

            // Convert to underlying enumeraiton value type
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
                throw new ArgumentNullException("value");
            if (!Enum.IsDefined(targetType, value))
                throw new ArgumentOutOfRangeException(value.ToString());

            // No conversion necessary as should cast directly to enumeration type
            return value;
        }
    }
}
