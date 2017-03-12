using System;

namespace Emlid.WindowsIot.Hardware.Components.Px4io.Data
{
    /// <summary>
    /// Defines the flags in the <see cref="Px4ioRCInputRawRegisters.Status"/> register.
    /// </summary>
    /// <see href="https://github.com/emlid/navio-rcio-linux-driver/blob/master/protocol.h"/>
    [Flags]
    [CLSCompliant(false)]
    public enum Px4ioRCInputRawStatusFlags : ushort
    {
        /// <summary>
        /// Single frame drop.
        /// </summary>
        FrameDropped = (1 << 0),

        /// <summary>
        /// Receiver is in failsafe mode.
        /// </summary>
        Failsafe = (1 << 1),

        /// <summary>
        /// DSM decoding is 11 bit mode.
        /// </summary>
        Dsm11 = (1 << 2),

        /// <summary>
        /// Channel mapping is okay.
        /// </summary>
        MappingOk = (1 << 3),

        /// <summary>
        /// RC reception okay.
        /// </summary>
        ReceptionOk = (1 << 4)
    }
}
