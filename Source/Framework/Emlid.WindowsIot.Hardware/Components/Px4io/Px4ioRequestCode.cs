using System.Diagnostics.CodeAnalysis;

namespace Emlid.WindowsIot.Hardware.Components.Px4io
{
    /// <summary>
    /// PX4IO request packet code.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1028", Justification = "Non-integer enumeration better suited to match the hardware specification in a hardware library.")]
    public enum Px4ioRequestCode : byte
    {
        /// <summary>
        /// FMU->IO read transaction.
        /// </summary>
        Read = 0x00,

        /// <summary>
        /// FMU->IO write transaction.
        /// </summary>
        Write = 0x40
    }
}