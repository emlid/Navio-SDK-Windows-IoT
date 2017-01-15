namespace Emlid.WindowsIot.Hardware.Components.Px4io
{
    /// <summary>
    /// PX4IO protocol.
    /// </summary>
    /// <see href="https://github.com/emlid/navio-rcio-linux-driver/blob/master/protocol.h"/>
    public static class Px4ioProtocol
    {
        #region Constants

        /// <summary>
        /// Highest compatible protocol version.
        /// </summary>
        public const int Version = 4;

        /// <summary>
        /// Maximum number of controls supported by this protocol.
        /// </summary>
        public const int ControlCountMaximum = 8;

        #endregion
    }
}
