using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace Emlid.UniversalWindows.UI.Converters
{
    /// <summary>
    /// Byte to hexadecimal string two-way value converter.
    /// </summary>
    /// <remarks>
    /// Converts an unsigned byte to a fixed width format uppercase hexadecimal string
    /// without any prefix, i.e. exactly two characters "00" to "FF".
    /// Converts a string of any supported (<see cref="NumberStyles.HexNumber"/>)
    /// format to an unsigned byte.
    /// </remarks>
    public class ByteToHexStringValueConverter : IValueConverter
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
            return string.Format(CultureInfo.InvariantCulture, "{0:X2}", byteValue);
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
