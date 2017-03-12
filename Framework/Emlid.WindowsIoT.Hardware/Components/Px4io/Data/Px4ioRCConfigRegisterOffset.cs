using System;

namespace Emlid.WindowsIot.Hardware.Components.Px4io.Data
{
    /// <summary>
    /// <see cref="Px4ioPage.RCConfig"/> page register offsets.
    /// </summary>
    /// <see href="https://github.com/emlid/navio-rcio-linux-driver/blob/master/protocol.h"/>
    [CLSCompliant(false)]
    public enum Px4ioRCConfigRegisterOffset : byte
    {
        /// <summary>
        /// Lowest input value.
        /// </summary>
        Minimum = 0,

        /// <summary>
        /// Center input value.
        /// </summary>
        Center = 1,

        /// <summary>
        /// Highest input value.
        /// </summary>
        Maximum = 2,

        /// <summary>
        /// Band around center that is ignored.
        /// </summary>
        DeadZone = 3,

        /// <summary>
        /// Mapped input value.
        /// </summary>
        Assignment = 4,

        /// <summary>
        /// Channel options bitmask.
        /// </summary>
        Options = 5,

        /// <summary>
        /// Spacing between channel configuration data.
        /// </summary>
        Stride = 6
    }
}
