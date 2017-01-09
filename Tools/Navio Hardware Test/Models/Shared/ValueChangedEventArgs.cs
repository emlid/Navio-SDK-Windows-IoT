namespace Emlid.WindowsIot.Tests.NavioHardwareTestApp.Views.Tests
{
    /// <summary>
    /// Event arguments for a changed value.
    /// </summary>
    public class ValueChangedEventArgs<T>
    {
        #region Lifetime

        public ValueChangedEventArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Old value.
        /// </summary>
        public T OldValue { get; set; }

        /// <summary>
        /// New value.
        /// </summary>
        public T NewValue { get; set; }

        #endregion
    }
}