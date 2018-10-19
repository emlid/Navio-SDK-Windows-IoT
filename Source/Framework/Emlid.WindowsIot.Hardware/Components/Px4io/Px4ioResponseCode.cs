using System.Diagnostics.CodeAnalysis;

namespace Emlid.WindowsIot.Hardware.Components.Px4io
{
    /// <summary>
    /// PX4IO response packet code.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1028", Justification = "Non-integer enumeration better suited to match the hardware specification in a hardware library.")]
    public enum Px4ioResponseCode : byte
    {
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
        Error = 0x80
    }
}