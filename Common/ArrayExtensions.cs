using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Emlid.WindowsIot.Hardware
{
    /// <summary>
    /// Provides helper methods and extensions for working with arrays and collections.
    /// </summary>
    public static class ArrayExtensions
    {
        #region Public Methods

        /// <summary>
        /// Compares two list based arrays by value.
        /// </summary>
        public static bool AreEqual(IList array1, IList array2)
        {
            // Compare null
            if (array1 == null)
                return array2 == null;
            if (array2 == null)
                return false;

            // Compare length
            if (array1.Count != array2.Count)
                return false;

            // Compare values
            for (var index = 0; index < array1.Count; index++)
            {
                var value1 = array1[index];
                var value2 = array2[index];
                if (!ReferenceEquals(value1, null))
                {
                    // Compare nested array by value too
                    if (value1.GetType().IsArray)
                        return AreEqual((Array)value1, (Array)value2);

                    // Compare other objects using any defined comparer or operator overloads
                    // This will still compare reference types by reference when none are defined
                    if (!value1.Equals(value2))
                        return false;
                }
                else if (!ReferenceEquals(value2, null))
                    return false;
            }

            // Return same
            return true;
        }

        /// <summary>
        /// Compares two collections by value.
        /// </summary>
        public static bool AreEqual(IEnumerable enumeration1, IEnumerable enumeration2)
        {
            // Compare null
            if (enumeration1 == null)
                return enumeration2 == null;
            if (enumeration2 == null)
                return false;

            // Compare values
            var enumerator1 = enumeration1.GetEnumerator();
            var enumerator2 = enumeration2.GetEnumerator();
            bool enumeration1HasMore;
            var enumeration2HasMore = false;
            while ((enumeration1HasMore = enumerator1.MoveNext()) &&
                (enumeration2HasMore = enumerator2.MoveNext()))
            {
                var value1 = enumerator1.Current;
                var value2 = enumerator2.Current;
                if (!ReferenceEquals(value1, null))
                {
                    // Compare nested array by value too
                    if (value1.GetType().IsArray)
                        return AreEqual((Array)value1, (Array)value2);

                    // Compare other objects using any defined comparer or operator overloads
                    // This will still compare reference types by reference when none are defined
                    if (!value1.Equals(value2))
                        return false;
                }
                else if (!ReferenceEquals(value2, null))
                    return false;
            }

            // Compare length
            return !enumeration1HasMore && !enumeration2HasMore;
        }

        /// <summary>
        /// Compares part of two arrays for equality.
        /// </summary>
        public static bool AreEqual(byte[] array1, int offset1, byte[] array2, int offset2, int length)
        {
            // Check length does not exceed boundaries
            if (offset1 + length > array1.Length || offset2 + length > array2.Length)
                return false;

            // Compare array contents
            for (var i = 0; i < length; i++)
            {
                if (array1[offset1 + i] != array2[offset2 + i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Searches any array for a value, i.e. without having to create a list or collection.
        /// </summary>
        /// <typeparam name="T">Array type.</typeparam>
        /// <param name="array">Array to search.</param>
        /// <param name="value">Value to find.</param>
        /// <returns>True when present.</returns>
        public static bool Contains<T>(this IEnumerable<T> array, T value)
        {
            return array.Any(item => item.Equals(value));
        }

        /// <summary>
        /// Checks if the string array contains the specified value optionally ignoring case.
        /// </summary>
        /// <param name="array">Array to search.</param>
        /// <param name="value">Value to search for.</param>
        /// <param name="comparisonType">Comparison options, e.g. set to <see cref="StringComparison.OrdinalIgnoreCase"/> for a case insensitive comparison.</param>
        /// <returns>True when found.</returns>
        public static bool Contains(this IEnumerable<string> array, string value, StringComparison comparisonType = StringComparison.Ordinal)
        {
            return array.FirstOrDefault(item => String.Compare(item, value, comparisonType) == 0) != null;
        }

        /// <summary>
        /// Gets the hash code of all items in the array.
        /// </summary>
        public static int GetHashCodeOfItems(this IList array)
        {
            return array.Cast<object>().Aggregate(0, (current, item) => current ^ (!ReferenceEquals(item, null) ? item.GetHashCode() : 0));
        }

        /// <summary>
        /// Gets the hash code of all items in the array, or zero when null.
        /// </summary>
        public static int GetHashCodeOfItemsIfExists(IList array)
        {
            if (ReferenceEquals(array, null))
                return 0;
            return array.GetHashCodeOfItems();
        }

        /// <summary>
        /// Disposes all members implementing <see cref="IDisposable"/>.
        /// </summary>
        /// <param name="list">List of items to dispose.</param>
        public static void Dispose(this IList list)
        {
            foreach (var disposable in list.Cast<IDisposable>().ToArray())
            {
                list.Remove(disposable);
                disposable.Dispose();
            }
        }

        #endregion
    }
}
