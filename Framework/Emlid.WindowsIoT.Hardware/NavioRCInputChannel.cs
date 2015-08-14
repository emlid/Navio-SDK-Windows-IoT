using System;

namespace Emlid.WindowsIoT.Hardware
{
    /// <summary>
    /// Identifies and contains the value of an RC input channel with change notification.
    /// </summary>
    public class NavioRCInputChannel
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance at the specified index and an empty value.
        /// </summary>
        public NavioRCInputChannel(int index) : this(index, 0)
        {
        }

        /// <summary>
        /// Creates an instance with the specified values.
        /// </summary>
        public NavioRCInputChannel(int index, double value)
        {
            Index = index;
            Value = value;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Tests two objects of this type for equality by value.
        /// </summary>
        public static bool operator ==(NavioRCInputChannel item1, NavioRCInputChannel item2)
        {
            if (!ReferenceEquals(item1, null))
                return item1.Equals(item2);

            return ReferenceEquals(item2, null);
        }

        /// <summary>
        /// Tests two objects of this type for inequality by value.
        /// </summary>
        public static bool operator !=(NavioRCInputChannel item1, NavioRCInputChannel item2)
        {
            if (!ReferenceEquals(item1, null))
                return !item1.Equals(item2);

            return !ReferenceEquals(item2, null);
        }

        /// <summary>
        /// Compares this object with another by value.
        /// </summary>
        public override bool Equals(object obj)
        {
            // Compare nullability and type
            var other = obj as NavioRCInputChannel;
            if (ReferenceEquals(other, null))
                return false;

            // Compare values
            return
                other.Index == Index &&
                other.Value == Value;
        }

        /// <summary>
        /// Returns a hashcode based on the current value of this object.
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
        public double Value
        {
            get { return _value; }
            set
            {
                // Do nothing when same
                if (_value == value)
                    return;

                // Set new value
                _value = value;

                // Fire changed event
                DoValueChanged();
            }
        }
        double _value;

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
            if (ValueChanged != null)
                ValueChanged(this, EventArgs.Empty);
        }

        #endregion
    }
}
