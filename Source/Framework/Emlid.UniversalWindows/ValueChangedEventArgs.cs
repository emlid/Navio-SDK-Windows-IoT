using System;

namespace Emlid.UniversalWindows
{
    /// <summary>
    /// Event arguments for a changed value.
    /// </summary>
    public class ValueChangedEventArgs<T> : EventArgs
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance with the specified values.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        public ValueChangedEventArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        #endregion Lifetime

        #region Properties

        /// <summary>
        /// Old value.
        /// </summary>
        public T OldValue { get; set; }

        /// <summary>
        /// New value.
        /// </summary>
        public T NewValue { get; set; }

        #endregion Properties
    }
}