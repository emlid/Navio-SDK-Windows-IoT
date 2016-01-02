using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace Emlid.WindowsIot.Common
{
    /// <summary>
    /// Hexadecimal value converter enables hexadecimal values to be used in data binding.
    /// </summary>
    public class HexByteValueConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Validate
            if (ReferenceEquals(value, null))
                throw new ArgumentNullException(nameof(value));
            if (value.GetType() != typeof(byte))
                throw new NotSupportedException();
            var byteValue = (byte)value;

            // Convert to hexadecimal byte string
            return String.Format(CultureInfo.InvariantCulture, "{0:X2}", byteValue);
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object. This method is called only in TwoWay bindings.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // Validate
            var stringValue = value as string;
            if (ReferenceEquals(stringValue, null))
                throw new ArgumentNullException(nameof(value));

            // Convert hexadecimal string to byte
            return byte.Parse(stringValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }
    }
}
