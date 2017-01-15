namespace Emlid.WindowsIot.Hardware.Components.Px4io
{
    /// <summary>
    /// PX4IO configuration page.
    /// </summary>
    /// <see href="https://github.com/emlid/navio-rcio-linux-driver/blob/master/protocol.h"/>
    public struct Px4ioConfigurationPage
    {
        #region Public Fields

        /// <summary>
        /// Protocol version.
        /// </summary>
        /// <see cref="Px4ioProtocol.Version"/>
        public byte ProtocolVersion;

        /// <summary>
        /// Hardware version.
        /// </summary>
        public byte HardwareVersion;

        /// <summary>
        /// Boot loader version.
        /// </summary>
        public byte BootLoaderVersion;

        /// <summary>
        /// Maximum I2C transfer size.
        /// </summary>
        public byte TransferMaximum;

        /// <summary>
        /// Maximum control count supported.
        /// </summary>
        public byte ControlCountMaximum;

        /// <summary>
        /// Maximum actuator count supported.
        /// </summary>
        public byte ActuatorCountMaximum;

        /// <summary>
        /// Maximum RC input count supported.
        /// </summary>
        public byte RCInputCountMaximum;

        /// <summary>
        /// Maximum ADC input count supported.
        /// </summary>
        public byte AdcInputCountMaximum;

        /// <summary>
        /// number of relay outputs.
        /// </summary>
        public byte RelayCount;

        /// <summary>
        /// Number of control groups.
        /// </summary>
        public byte ControlGroupCount;

        #endregion
    }
}
