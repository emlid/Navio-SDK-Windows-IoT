namespace Emlid.WindowsIot.Hardware.Components.Px4io
{
    /// <summary>
    /// PX4IO response packet code.
    /// </summary>
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
