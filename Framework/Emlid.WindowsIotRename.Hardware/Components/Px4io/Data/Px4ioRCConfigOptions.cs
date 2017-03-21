using System;

namespace Emlid.WindowsIot.Hardware.Components.Px4io.Data
{
    /// <summary>
    /// Defines the flags in the <see cref="Px4ioRCConfigRegisters.Options"/> register.
    /// </summary>
    /// <see href="https://github.com/emlid/navio-rcio-linux-driver/blob/master/protocol.h"/>
    [Flags]
    [CLSCompliant(false)]
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
