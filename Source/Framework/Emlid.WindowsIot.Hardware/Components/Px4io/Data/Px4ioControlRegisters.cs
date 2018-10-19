using System;
using System.Collections.ObjectModel;

namespace Emlid.WindowsIot.Hardware.Components.Px4io.Data
{
    /// <summary>
    /// <see cref="Px4ioPage.Controls"/> page register data.
    /// </summary>
    /// <see href="https://github.com/emlid/navio-rcio-linux-driver/blob/master/protocol.h"/>
    [CLSCompliant(false)]
    public sealed class Px4ioControlRegisters
    {
        #region Constants

        /// <summary>
        /// Number of registers on this page.
        /// </summary>
        public const byte RegisterCount = 32; // TODO: Should be 65 including groups valid

        /// <summary>
        /// Maximum number of controls.
        /// </summary>
        public const int ControlsMaximum = 8;

        #endregion Constants

        #region Lifetime

        /// <summary>
        /// Creates an instance from register values.
        /// </summary>
        /// <param name="data">Register values read from the device.</param>
        public Px4ioControlRegisters(ushort[] data)
        {
            // Validate
            if (data == null || data.Length < RegisterCount)
                throw new ArgumentOutOfRangeException(nameof(data));

            // Set properties from data
            var offset = 0;
            Group0 = new Collection<byte>();
            for (var index = 0; index < ControlsMaximum; index++, offset++)
                Group0.Add((byte)data[offset]);
            Group1 = new Collection<byte>();
            for (var index = 0; index < ControlsMaximum; index++, offset++)
                Group1.Add((byte)data[offset]);
            Group2 = new Collection<byte>();
            for (var index = 0; index < ControlsMaximum; index++, offset++)
                Group2.Add((byte)data[offset]);
            Group3 = new Collection<byte>();
            for (var index = 0; index < ControlsMaximum; index++, offset++)
                Group3.Add((byte)data[offset]);
            GroupsValid = (Px4ioControlGroupsValidFlags)data[(int)Px4ioControlRegisterOffset.GroupsValid];
        }

        #endregion Lifetime

        #region Public Fields

        /// <summary>
        /// Control group 0.
        /// </summary>
        public Collection<byte> Group0 { get; private set; }

        /// <summary>
        /// Control group 1.
        /// </summary>
        public Collection<byte> Group1 { get; private set; }

        /// <summary>
        /// Control group 2.
        /// </summary>
        public Collection<byte> Group2 { get; private set; }

        /// <summary>
        /// Control group 3.
        /// </summary>
        public Collection<byte> Group3 { get; private set; }

        /// <summary>
        /// Group validation flags.
        /// </summary>
        public Px4ioControlGroupsValidFlags GroupsValid { get; set; }

        #endregion Public Fields
    }
}