using System;

namespace Emlid.WindowsIot.Hardware.Components.NxpPca9685
{
    /// <summary>
    /// References an <seealso cref="NxpPca9685ChannelValue"/> by index and provides change notification.
    /// </summary>
    public class NxpPca9685Channel
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance at the specified index and an empty value.
        /// </summary>
        public NxpPca9685Channel(int index) : this(index, new NxpPca9685ChannelValue())
        {
        }

        /// <summary>
        /// Creates an instance with the specified values.
        /// </summary>
        public NxpPca9685Channel(int index, NxpPca9685ChannelValue value)
        {
            Index = index;
            Value = value;
            Value.Changed += OnValueChanged;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Tests two objects of this type for equality by value.
        /// </summary>
        public static bool operator ==(NxpPca9685Channel item1, NxpPca9685Channel item2)
        {
            if (!ReferenceEquals(item1, null))
                return item1.Equals(item2);

            return ReferenceEquals(item2, null);
        }

        /// <summary>
        /// Tests two objects of this type for inequality by value.
        /// </summary>
        public static bool operator !=(NxpPca9685Channel item1, NxpPca9685Channel item2)
        {
            if (!ReferenceEquals(item1, null))
                return !item1.Equals(item2);

            return !ReferenceEquals(item2, null);
        }

        /// <summary>
        /// Compares this object with another by value.
        /// </summary>
        /// <param name="value">Object with which to compare by value.</param>
        public override bool Equals(object value)
        {
            // Compare nullability and type
            var other = value as NxpPca9685Channel;
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
                (Value?.GetHashCode() ?? 0);
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
        public NxpPca9685ChannelValue Value { get; private set; }

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
        /// Fires the <see cref="ValueChanged"/> event when the <see cref="NxpPca9685ChannelValue.Changed"/> event is received.
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
