using System;
using Windows.Devices.I2c;

namespace Emlid.WindowsIot.Hardware
{
    /// <summary>
    /// MB85RC04V 512 byte FRAM (Ferroelectric Random Access Memory) chip (hardware device), connected via I2C.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The MB85RC04V is an FRAM (Ferroelectric Random Access Memory) chip in a configuration of 512
    /// words × 8 bits, using the ferroelectric process and silicon gate CMOS process technologies for forming the
    /// nonvolatile memory cells.
    /// </para>
    /// <para>
    /// Unlike SRAM, the MB85RC04V is able to retain data without using a data backup battery.
    /// </para>
    /// <para>
    /// The read/write endurance of the nonvolatile memory cells used for the MB85RC04V has improved to be at
    /// least 10^12 cycles, significantly outperforming other nonvolatile memory products in the number.
    /// </para>
    /// <para>
    /// The MB85RC04V does not need a polling sequence after writing to the memory such as the case of Flash
    /// memory or E2PROM.
    /// </para>
    /// <para>
    /// Data sheet: https://www.fujitsu.com/us/Images/MB85RC04V-DS501-00016-2v0-E.pdf
    /// </para>
    /// </remarks>
    public class Mb85rc04vDevice : Mb85rcvDevice
    {
        #region Constants

        /// <summary>
        /// Memory size in bytes.
        /// </summary>
        public const int MemorySize = 512;

        /// <summary>
        /// Bit mask for the A1 and A2 device address code.
        /// </summary>
        /// <remarks>
        /// The device address code identifies one device from up to 4 devices connected to the bus.
        /// Each MB85RC04V is given a unique 2 bits code on the device address pin (external hardware pin A2 and A1).
        /// The slave only responds if the received device address code is equal to this unique 2 bits code.
        /// </remarks>
        /// <remarks>
        /// Shifted down 1 bit because the <see cref="I2cDevice"/> handles the read/write flag automatically (a.k.a. 7-bit addressing).
        /// </remarks>
        public const int DeviceAddressBitmask = 0x0c >> 1;

        /// <summary>
        /// Bit mask for the memory upper address code.
        /// </summary>
        /// <remarks>
        /// This bit is not the setting bit for the slave address, but the upper 1-bit setting bit for the memory address.
        /// Shifted down 1 bit because the <see cref="I2cDevice"/> handles the read/write flag automatically (a.k.a. 7-bit addressing).
        /// </remarks>
        public const int MemoryUpperAddressBitmask = 0x02 >> 1;

        /// <summary>
        /// Number bytes which make-up the memory address in commands.
        /// </summary>
        /// <remarks>
        /// The MS85RC04V chip has a complex memory addressing scheme with only the lower byte
        /// specified as part of the command request. The 9th bit is separated into the device address
        /// so it is necessary to interact with multiple I2C addresses to access all memory individually.
        /// </remarks>
        public const int MemoryLowerAddressCommandBytes = 1;

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
        public Mb85rc04vDevice(int address, bool fast, bool exclusive)
            : base(address, fast, exclusive, MemorySize)
        {
            // Validate
            if ((address & MemoryUpperAddressBitmask) != 0)
                throw new ArgumentOutOfRangeException(nameof(address),
                "The MB85RC04V I2C device connection must not have the memory upper address bit set.");

            // Initialize additional device addresses
            var upperAddress = address | MemoryUpperAddressBitmask;
            HardwareUpper = Connect(Id, upperAddress, fast, exclusive);
        }

        #region IDisposable

        /// <summary>
        /// Frees resources owned by this instance.
        /// </summary>
        /// <param name="disposing">
        /// True when called via <see cref="IDisposable.Dispose()"/>, false when called from the finalizer.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            // Do nothing when already disposed
            if (IsDisposed) return;

            // Dispose
            try
            {
                // Dispose managed resource during dispose
                if (disposing)
                {
                    if (HardwareUpper != null)
                        HardwareUpper.Dispose();
                }
            }
            finally
            {
                // Dispose base class
                base.Dispose(disposing);
            }
        }

        #endregion

        #endregion

        #region Protected Properties

        /// <summary>
        /// I2C device for the slave address with the MSB/upper address bit set and
        /// the read/write flag cleared for write (0 = input).
        /// </summary>
        /// <remarks>
        /// The base class <see cref="Hardware"/> device is opened with all non-I2C
        /// address bits clear, which means the MSB/upper address bit is 0 and the
        /// read/write flag is cleared for write (0 = input).
        /// </remarks>
        [CLSCompliant(false)]
        protected I2cDevice HardwareUpper { get; private set; }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the <see cref="I2cDevice"/> required to communicate with a specific 
        /// memory address or access method.
        /// </summary>
        /// <param name="address">Memory address. Leave zero when performing commands which operate on the current address.</param>
        /// <remarks>
        /// Because the chip mixes sub-address bits and flags with the slave address and the Windows
        /// <see cref="I2cDevice.ConnectionSettings"/> cannot be modified after creation, it is
        /// necessary to use multiple <see cref="I2cDevice"/> instances for each.
        /// </remarks>
        [CLSCompliant(false)]
        public override I2cDevice GetDevice(int address)
        {
            var upper = (address & MemoryUpperAddressBitmask) != 0;
            return upper ? HardwareUpper : Hardware;
        }

        /// <summary>
        /// Gets the I2C command memory address bytes for the specified logical address.
        /// </summary>
        /// <param name="address"></param>
        /// <returns>
        /// Byte array which can be written to request the specified memory address,
        /// assuming the correct I2C device is being used as provided by <see cref="GetDevice(int)"/>
        /// which may include the MSB in it's I2C address.
        /// </returns>
        /// <remarks>
        /// Besides being split into bytes, some older/smaller chips separate the MSB
        /// into the I2C device address.
        /// </remarks>
        public override byte[] GetMemoryAddressBytes(int address)
        {
            // Lower 8 bits only (9th MSB is encoded in I2C "memory upper" address)
            return new[] { (byte)(address) };
        }

        #endregion
    }
}
