namespace Emlid.WindowsIot.Hardware.Components.Px4io
{
    /// <summary>
    /// PX4IO request packet code.
    /// </summary>
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
