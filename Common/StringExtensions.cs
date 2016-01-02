using System;
using System.Text;

namespace Emlid.WindowsIot.Common
{
    public static class StringExtensions
    {
        /// <summary>
        /// Minimum ASCII character value which is printable (not a special character).
        /// </summary>
        public const char AsciiMinimumPrintable = '\u0020';

        /// <summary>
        /// Minimum ASCII character value which is printable (not a special character).
        /// </summary>
        public const char AsciiMaximumPrintable = '\u007f';

        /// <summary>
        /// Filters a string down to printable characters.
        /// </summary>
        /// <param name="value">Raw string to filter.</param>
        /// <param name="placeholder">Optional place-holder to replace filtered characters with.</param>
        /// <returns>Filtered string.</returns>
        public static string FilterSpecial(this string value, char? placeholder = '?')
        {
            // Validate
            if (value == null) throw new ArgumentNullException(nameof(value));

            // Filter...
            var result = new StringBuilder();
            foreach (var rawChar in value)
            {
                var valid = rawChar >= AsciiMinimumPrintable && rawChar <= AsciiMaximumPrintable;
                if (valid)
                    result.Append(rawChar);
                else if (placeholder.HasValue)
                    result.Append(placeholder.Value);
            }
            return result.ToString();
        }
    }
}