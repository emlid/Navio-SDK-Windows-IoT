using Emlid.WindowsIot.Hardware.System;
using System;
using Windows.Devices.I2c;

namespace Emlid.WindowsIot.Hardware.Components.Mb85rcv
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
        /// Density of this model.
        /// </summary>
        public const byte Density = 0x05;

        /// <summary>
        /// Maximum number of devices (chip number) for this model.
        /// </summary>
        public const int MaximumDevices = 8;

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
        /// Creates an instance connected to the specified I2C bus and chip number.
        /// </summary>
        /// <param name="busNumber">I2C bus controller number (zero based).</param>
        /// <param name="chipNumber">Chip number (device address code).</param>
        /// <param name="speed">Bus speed.</param>
        /// <param name="sharingMode">Sharing mode.</param>
        [CLSCompliant(false)]
        public Mb85rc256vDevice(int busNumber, byte chipNumber,
            I2cBusSpeed speed = I2cBusSpeed.FastMode, I2cSharingMode sharingMode = I2cSharingMode.Exclusive)
            : base(chipNumber, MemorySize)
        {
            // Get address
            var address = GetDataI2cAddress(chipNumber);

            // Connect to hardware
            Hardware = I2cExtensions.Connect(busNumber, address, speed, sharingMode).GetAwaiter().GetResult();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the device identifier by sending the Device ID command to the
        /// specified chip number (device address code).
        /// </summary>
        /// <param name="busNumber">I2C bus controller number (zero based).</param>
        /// <param name="chipNumber">
        /// Device (chip) number, from zero to the <see cref="MaximumDevices"/> supported.
        /// </param>
        /// <param name="speed">Bus speed.</param>
        /// <param name="sharingMode">Sharing mode.</param>
        /// <returns>Device ID.</returns>
        [CLSCompliant(false)]
        public static Mb85rcvDeviceId GetDeviceId(int busNumber, byte chipNumber,
            I2cBusSpeed speed = I2cBusSpeed.FastMode, I2cSharingMode sharingMode = I2cSharingMode.Exclusive)
        {
            // Validate
            if (chipNumber < 0 || chipNumber > MaximumDevices)
                throw new ArgumentOutOfRangeException(nameof(chipNumber));

            // Calculate device addresses
            var idAddress = GetDeviceIdI2cAddress(chipNumber);
            var dataAddress = GetDataI2cAddress(chipNumber);

            // Call overloaded method
            return GetDeviceId(busNumber, idAddress, dataAddress);
        }

        /// <summary>
        /// Gets the I2C address for data commands with the specified chip number (device address code).
        /// </summary>
        /// <param name="chipNumber">
        /// Device (chip) number, from zero to the <see cref="MaximumDevices"/> supported.
        /// </param>
        /// <returns>7-bit I2C address.</returns>
        public static byte GetDataI2cAddress(byte chipNumber)
        {
            // Validate
            if (chipNumber < 0 || chipNumber > MaximumDevices)
                throw new ArgumentOutOfRangeException(nameof(chipNumber));

            // Calculate and return address
            return (byte)(DataI2cAddress + chipNumber);
        }

        /// <summary>
        /// Gets the I2C address for the device ID command with the specified chip number (device address code).
        /// </summary>
        /// <param name="chipNumber">
        /// Device (chip) number, from zero to the <see cref="MaximumDevices"/> supported.
        /// </param>
        /// <returns>7-bit I2C address.</returns>
        public static byte GetDeviceIdI2cAddress(byte chipNumber)
        {
            // Validate
            if (chipNumber < 0 || chipNumber > MaximumDevices)
                throw new ArgumentOutOfRangeException(nameof(chipNumber));

            // Calculate and return address
            return (byte)(DeviceIdI2cAddress + chipNumber);
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
