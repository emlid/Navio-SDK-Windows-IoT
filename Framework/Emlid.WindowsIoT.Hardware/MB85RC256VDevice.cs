using Windows.Devices.I2c;

namespace Emlid.WindowsIot.Hardware
{
    /// <summary>
    /// MB85RC256V 32KiB FRAM (Ferroelectric Random Access Memory) chip (hardware device), connected via I2C.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The MB85RC256V is an FRAM (Ferroelectric Random Access Memory) chip in a configuration of 32,768
    /// words × 8 bits, using the ferroelectric process and silicon gate CMOS process technologies for forming the
    /// nonvolatile memory cells.
    /// </para>
    /// <para>
    /// Unlike SRAM, the MB85RC256V is able to retain data without using a data backup battery.
    /// </para>
    /// <para>
    /// The read/write endurance of the nonvolatile memory cells used for the MB85RC256V has improved to be
    /// at least 10^12 cycles, significantly outperforming other nonvolatile memory products in the number.
    /// </para>
    /// <para>
    /// The MB85RC256V does not need a polling sequence after writing to the memory such as the case of Flash
    /// memory or E2PROM.
    /// </para>
    /// <para>
    /// Data sheet: https://www.fujitsu.com/us/Images/MB85RC256V-DS501-00017-3v0-E.pdf
    /// </para>
    /// </remarks>
    public class Mb85rc256vDevice : Mb85rcvDevice
    {
        #region Constants

        /// <summary>
        /// Memory size in bytes.
        /// </summary>
        public const int MemorySize = 32768;

        /// <summary>
        /// Bit mask for the A1, A2 and A0 device address code.
        /// </summary>
        /// <remarks>
        /// The device address code identifies one device from up to eight devices connected to the bus.
        /// Each MB85RC256V is given a unique 3 bits code on the device address pin(external hardware pin A2, A1,
        /// and A0). The slave only responds if the received device address code is equal to this unique 3 bits code.
        /// Shifted down 1 bit because the <see cref="I2cDevice"/> handles the read/write flag automatically (a.k.a. 7-bit addressing).
        /// </remarks>
        public const int DeviceAddressBitmask = 0x0e >> 1;

        /// <summary>
        /// Number bytes which make-up the memory address in commands.
        /// </summary>
        /// <remarks>
        /// The MS85RC256V chip has a simple memory addressing scheme with all whole bytes
        /// specified as part of the command request. No bits are separated into the device address
        /// as was done on older smaller chips.
        /// </remarks>
        public const int MemoryAddressCommandBytes = 2;

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates an instance at the specified I2C <paramref name="address"/> with custom settings.
        /// </summary>
        /// <param name="address">
        /// I2C slave address of the chip.
        /// This is a physical property, not a software option.
        /// </param>
        /// <param name="fast">
        /// Set true for I2C <see cref="I2cBusSpeed.FastMode"/> or false for <see cref="I2cBusSpeed.StandardMode"/>.
        /// </param>
        /// <param name="exclusive">
        /// Set true for I2C <see cref="I2cSharingMode.Exclusive"/> or false for <see cref="I2cSharingMode.Shared"/>.
        /// </param>
        public Mb85rc256vDevice(int address, bool fast, bool exclusive)
            : base(address, fast, exclusive, MemorySize)
        {
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the I2C command memory address bytes for the specified logical address.
        /// </summary>
        /// <param name="address"></param>
        /// <returns>
        /// Byte array which can be written to request the specified memory address.
        /// </returns>
        public override byte[] GetMemoryAddressBytes(int address)
        {
            // High byte comes before low byte
            return new[] { (byte)(address >> 8), (byte)(address) };
        }

        #endregion
    }
}
