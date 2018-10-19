using System;
using System.Diagnostics.CodeAnalysis;

namespace Emlid.WindowsIot.Hardware.Components.Px4io.Data
{
    /// <summary>
    /// Defines the flags in the <see cref="Px4ioStatusRegisters.Mixer"/> register.
    /// </summary>
    /// <see href="https://github.com/emlid/navio-rcio-linux-driver/blob/master/protocol.h"/>
    [Flags]
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Design", "CA1028", Justification = "Non-integer enumeration better suited to match the hardware specification in a hardware library.")]
    public enum Px4ioStatusMixerFlags : ushort
    {
        /// <summary>
        /// At least one actuator output has reached lower limit.
        /// </summary>
        LowerLimit = (1 << 0),

        /// <summary>
        /// At least one actuator output has reached upper limit.
        /// </summary>
        UpperLimit = (1 << 1),

        /// <summary>
        /// Yaw control is limited because it causes output clipping.
        /// </summary>
        YawLimit = (1 << 2)
    }
}