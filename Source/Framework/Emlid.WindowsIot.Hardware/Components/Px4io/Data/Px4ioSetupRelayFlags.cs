using System;
using System.Diagnostics.CodeAnalysis;

namespace Emlid.WindowsIot.Hardware.Components.Px4io.Data
{
    /// <summary>
    /// Defines the flags in the <see cref="Px4ioSetupRegisters.Relays"/> register.
    /// </summary>
    /// <see href="https://github.com/emlid/navio-rcio-linux-driver/blob/master/protocol.h"/>
    [Flags]
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Design", "CA1028", Justification = "Non-integer enumeration better suited to match the hardware specification in a hardware library.")]
    public enum Px4ioSetupRelayFlags : ushort
    {
        /// <summary>
        /// Power relay 1.
        /// </summary>
        /// <remarks>
        /// Hardware version 1 only.
        /// </remarks>
        Power1 = (1 << 0),

        /// <summary>
        /// Power relay 2.
        /// </summary>
        /// <remarks>
        /// Hardware version 1 only.
        /// </remarks>
        Power2 = (1 << 1),

        /// <summary>
        /// Accessory relay 1.
        /// </summary>
        /// <remarks>
        /// Hardware version 1 only.
        /// </remarks>
        AccessoryPower1 = (1 << 2),

        /// <summary>
        /// Accessory relay 2.
        /// </summary>
        /// <remarks>
        /// Hardware version 1 only.
        /// </remarks>
        AccessoryPower2 = (1 << 3)
    }
}