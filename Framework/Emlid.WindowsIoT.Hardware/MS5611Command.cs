namespace Emlid.WindowsIot.Hardware
{
    /// <summary>
    /// Defines the I2C commands of the <see cref="Ms5611Device"/>.
    /// </summary>
    /// <remarks>
    /// Instead of registers data is exchanged by means of writing a command byte
    /// then reading a variable number of data bytes returned by that command.
    /// Some commands must be executed in sequence.
    /// </remarks>
    public enum Ms5611Command : byte
    {
        #region Commands

        /// <summary>
        /// Resets the device to a known state and prepares calibration
        /// values for the PROM read commands.
        /// </summary>
        /// <remarks>
        /// Follow with the <see cref="PromRead"/> command.
        /// </remarks>
        Reset = 0x1e,

        /// <summary>
        /// Analog to Digital Converter (ADC) read command address.
        /// </summary>
        /// <remarks>
        /// Returns a 3 byte (24 bit) result.
        /// </remarks>
        AdcRead = 0x00,

        /// <summary>
        /// PROM read coefficient 0 (manufacturer data) command address.
        /// </summary>
        /// <remarks>
        /// Only called once after the <see cref="Reset"/> command.
        /// A sequence of 8 reads must be made to read all coefficient values and checksum.
        /// Each read returns a 2 byte (16 bit) result. See <see cref="Ms5611PromData.Read(byte[])"/>.
        /// </remarks>
        PromRead = 0xa0,

        /// <summary>
        /// Digital pressure conversion command address.
        /// </summary>
        /// <remarks>
        /// Returns a 4 byte (32 bit) unsigned result.
        /// Over-Sampling Rate (OSR) option is specified by adding the <see cref="Ms5611Osr"/> to this address.
        /// </remarks>
        ConvertD1Pressure = 0x40,

        /// <summary>
        /// Digital temperature conversion command address.
        /// </summary>
        /// <remarks>
        /// Returns a 4 byte (32 bit) unsigned result.
        /// Over-Sampling Rate (OSR) option is specified by adding the <see cref="Ms5611Osr"/> to this address.
        /// </remarks>
        ConvertD2Temperature = 0x50,

        #endregion

        #region Offsets

        /// <summary>
        /// Offset to the convert commands for OSR 256.
        /// </summary>
        ConvertOsr256Offset = 0,

        /// <summary>
        /// Offset to the convert commands for OSR 512.
        /// </summary>
        ConvertOsr512Offset = 2,

        /// <summary>
        /// Offset to the convert commands for OSR 1024.
        /// </summary>
        ConvertOsr1024Offset = 4,

        /// <summary>
        /// Offset to the convert commands for OSR 2048.
        /// </summary>
        ConvertOsr2048Offset = 6,

        /// <summary>
        /// Offset to the convert commands for OSR 4096.
        /// </summary>
        ConvertOsr4096Offset = 8

        #endregion
    }
}
