using System;

namespace Emlid.WindowsIot.Hardware.Components.Px4io.Data
{
    /// <summary>
    /// <see cref="Px4ioPage.Config"/> page register data.
    /// </summary>
    /// <see href="https://github.com/emlid/navio-rcio-linux-driver/blob/master/protocol.h"/>
    [CLSCompliant(false)]
    public sealed class Px4ioConfigRegisters
    {
        #region Constants

        /// <summary>
        /// Number of registers on this page.
        /// </summary>
        public const byte RegisterCount = 9;

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates an instance from register values.
        /// </summary>
        /// <param name="data">Register values read from the device.</param>
        public Px4ioConfigRegisters(ushort[] data)
        {
            // Validate
            if (data == null || data.Length < RegisterCount)
                throw new ArgumentOutOfRangeException(nameof(data));

            // Set properties from data
            ProtocolVersion = data[0];
            HardwareVersion = data[1];
            BootLoaderVersion = data[2];
            TransferLimit = data[3];
            ControlCount = data[4];
            ActuatorCount = data[5];
            RCInputCount = data[6];
            AdcInputCount = data[7];
            RelayAndControlGroupCount = data[8];
        }

        #endregion

        #region Public Fields

        /// <summary>
        /// Protocol version.
        /// </summary>
        /// <see cref="Px4ioPacket.Version"/>
        public ushort ProtocolVersion;

        /// <summary>
        /// Hardware version.
        /// </summary>
        public ushort HardwareVersion;

        /// <summary>
        /// Boot loader version.
        /// </summary>
        public ushort BootLoaderVersion;

        /// <summary>
        /// Maximum I2C transfer size.
        /// </summary>
        public ushort TransferLimit;

        /// <summary>
        /// Maximum control count supported.
        /// </summary>
        public ushort ControlCount;

        /// <summary>
        /// Maximum actuator count supported.
        /// </summary>
        public ushort ActuatorCount;

        /// <summary>
        /// Maximum RC input count supported.
        /// </summary>
        public ushort RCInputCount;

        /// <summary>
        /// Maximum ADC input count supported.
        /// </summary>
        public ushort AdcInputCount;

        /// <summary>
        /// number of relay outputs or control groups.
        /// </summary>
        public ushort RelayAndControlGroupCount;

        #endregion
    }
}
