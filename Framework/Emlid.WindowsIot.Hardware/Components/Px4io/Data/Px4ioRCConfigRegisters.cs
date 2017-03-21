using System;

namespace Emlid.WindowsIot.Hardware.Components.Px4io.Data
{
    /// <summary>
    /// <see cref="Px4ioPage.RCConfig"/> page register data.
    /// </summary>
    /// <see href="https://github.com/emlid/navio-rcio-linux-driver/blob/master/protocol.h"/>
    [CLSCompliant(false)]
    public sealed class Px4ioRCConfigRegisters
    {
        #region Constants

        /// <summary>
        /// Number of registers on this page.
        /// </summary>
        public const byte RegisterCount = 7;

        /// <summary>
        /// Value for the <see cref="Assignment"/> register to cause a mode switch.
        /// </summary>
        public const ushort AssignmentModeSwitchValue = 100;

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates an instance from register values.
        /// </summary>
        /// <param name="data">Register values read from the device.</param>
        public Px4ioRCConfigRegisters(ushort[] data)
        {
            // Validate
            if (data == null || data.Length < RegisterCount)
                throw new ArgumentOutOfRangeException(nameof(data));

            // Set properties from data
            var offset = 0;
            Minimum = data[offset++];
            Center = data[offset++];
            Maximum = data[offset++];
            DeadZone = data[offset++];
            Assignment = data[offset++];
            Options = (Px4ioRCConfigOptions)data[offset++];
            Stride = data[offset++];
        }

        #endregion

        #region Public Fields

        /// <summary>
        /// Lowest input value.
        /// </summary>
        public ushort Minimum;

        /// <summary>
        /// Center input value.
        /// </summary>
        public ushort Center;

        /// <summary>
        /// Highest input value.
        /// </summary>
        public ushort Maximum;

        /// <summary>
        /// Band around center that is ignored.
        /// </summary>
        public ushort DeadZone;

        /// <summary>
        /// Mapped input value.
        /// </summary>
        public ushort Assignment;

        /// <summary>
        /// Channel options bitmask.
        /// </summary>
        public Px4ioRCConfigOptions Options;

        /// <summary>
        /// Spacing between channel configuration data.
        /// </summary>
        public ushort Stride;

        #endregion
    }
}
