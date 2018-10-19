using System;
using System.Diagnostics.CodeAnalysis;

namespace Emlid.WindowsIot.Hardware.Components.Px4io.Data
{
    /// <summary>
    /// Defines the flags in the <see cref="Px4ioRCConfigRegisters.Options"/> register.
    /// </summary>
    /// <see href="https://github.com/emlid/navio-rcio-linux-driver/blob/master/protocol.h"/>
    [Flags]
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Design", "CA1028", Justification = "Non-integer enumeration better suited to match the hardware specification in a hardware library.")]
    public enum Px4ioRCConfigOptions : ushort
    {
        /// <summary>
        /// Enabled.
        /// </summary>
        Enabled = (1 << 0),

        /// <summary>
        /// Reverse.
        /// </summary>
        Reverse = (1 << 1)
    }
}