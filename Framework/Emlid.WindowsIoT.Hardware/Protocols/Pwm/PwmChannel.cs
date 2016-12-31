using System;

namespace Emlid.WindowsIot.Hardware.Protocols.Pwm
{
    /// <summary>
    /// References an <seealso cref="PwmValue"/> by index and provides change notification.
    /// </summary>
    public class PwmChannel
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance at the specified index and an empty value.
        /// </summary>
        public PwmChannel(int index) : this(index, new PwmValue())
        {
        }

        /// <summary>
        /// Creates an instance with the specified values.
        /// </summary>
        public PwmChannel(int index, PwmValue value)
        {
            Index = index;
            Value = value;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Tests two objects of this type for equality by value.
        /// </summary>
        public static bool operator ==(PwmChannel left, PwmChannel right)
        {
            return !ReferenceEquals(left, null)
                ? left.Equals(right)
                : ReferenceEquals(right, null);
        }

        /// <summary>
        /// Tests two objects of this type for inequality by value.
        /// </summary>
        public static bool operator !=(PwmChannel left, PwmChannel right)
        {
            return !ReferenceEquals(left, null)
                ? !left.Equals(right)
                : !ReferenceEquals(right, null);
        }

        /// <summary>
        /// Compares this object with another by value.
        /// </summary>
        /// <param name="value">Object with which to compare by value.</param>
        public override bool Equals(object value)
        {
            // Compare nullability and type
            var other = value as PwmChannel;
            if (ReferenceEquals(other, null))
                return false;

            // Compare values
            return
                other.Index == Index &&
                other.Value == Value;
        }

        /// <summary>
        /// Returns a hash-code based on the current value of this object.
        /// </summary>
        public override int GetHashCode()
        {
            return
                Index.GetHashCode() ^
                Value.GetHashCode();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Zero based channel index.
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Value.
        /// </summary>
        public PwmValue Value { get; private set; }

        #endregion

        #region Events

        /// <summary>
        /// Fired when the value changes.
        /// </summary>
        public event EventHandler ValueChanged;

        /// <summary>
        /// Fires the <see cref="ValueChanged"/> event.
        /// </summary>
        private void DoValueChanged()
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Fired when the <see cref="Value"/> changes.
        /// </summary>
        /// <param name="sender">Sender, this channel.</param>
        /// <param name="arguments">Standard event arguments, no specific data.</param>
        private void OnValueChanged(object sender, EventArgs arguments)
        {
            DoValueChanged();
        }

        #endregion
    }
}
