using System;

namespace Emlid.WindowsIot.Hardware.Components.Px4io
{
    /// <summary>
    /// PX4IO serial protocol packet count/codes bits.
    /// </summary>
    [Flags]
    public enum Px4ioSerialPacketCode : byte
    {
        /// <summary>
        /// FMU->IO read transaction.
        /// </summary>
        Read = 0x00,

        /// <summary>
        /// FMU->IO write transaction.
        /// </summary>
        Write = 0x40,

        /// <summary>
        /// IO->FMU success reply.
        /// </summary>
        Success = 0x00,

        /// <summary>
        /// IO->FMU bad packet reply.
        /// </summary>
        Corrupt = 0x40,

        /// <summary>
        /// IO->FMU register op error reply.
        /// </summary>
        Error = 0x80,
    }
}
