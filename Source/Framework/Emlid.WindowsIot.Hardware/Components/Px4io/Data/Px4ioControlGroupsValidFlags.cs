using System;
using System.Diagnostics.CodeAnalysis;

namespace Emlid.WindowsIot.Hardware.Components.Px4io.Data
{
    /// <summary>
    /// Defines the flags in the <see cref="Px4ioControlRegisters.GroupsValid"/> register.
    /// </summary>
    /// <see href="https://github.com/emlid/navio-rcio-linux-driver/blob/master/protocol.h"/>
    [Flags]
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Design", "CA1028", Justification = "Non-integer enumeration better suited to match the hardware specification in a hardware library.")]
    public enum Px4ioControlGroupsValidFlags : ushort
    {
        /// <summary>
        /// Group 0 is valid / received.
        /// </summary>
        Group0 = (1 << 0),

        /// <summary>
        /// Group 1 is valid / received.
        /// </summary>
        Group1 = (1 << 1),

        /// <summary>
        /// Group 2 is valid / received.
        /// </summary>
        Group2 = (1 << 2),

        /// <summary>
        /// Group 3 is valid / received.
        /// </summary>
        Group3 = (1 << 3)
    }
}