using Emlid.WindowsIot.Common;
using Emlid.WindowsIot.Hardware.System;
using System;
using Windows.Devices.I2c;

namespace Emlid.WindowsIot.Hardware.Components.Mb85rcv
{
    /// <summary>
    /// Base class for MB85RC#V FRAM (Ferroelectric Random Access Memory) family of chips (hardware devices), connected via I2C.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The MB85RC#V family are FRAM (Ferroelectric Random Access Memory) chips in different configurations,
    /// providing more or less words of memory, using the ferroelectric process and silicon gate CMOS process technologies
    /// for forming the nonvolatile memory cells.
    /// </para>
    /// <para>
    /// Unlike SRAM, the MB85RC#V chips are able to retain data without using a data backup battery.
    /// </para>
    /// <para>
    /// The read/write endurance of the nonvolatile memory cells used for the MB85RC#V has improved to be at
    /// least 10^12 cycles, significantly outperforming other nonvolatile memory products in the number.
    /// </para>
    /// <para>
    /// The MB85RC#V chips do not need a polling sequence after writing to the memory such as the case of Flash
    /// memory or E2PROM.
    /// </para>
    /// <para>
    /// MB85RC04V data sheet: https://www.fujitsu.com/us/Images/MB85RC04V-DS501-00016-2v0-E.pdf
    /// MB85RC256V data sheet: https://www.fujitsu.com/us/Images/MB85RC256V-DS501-00017-3v0-E.pdf
    /// </para>
    /// <para>
    /// The "Device Address Code" has been renamed to "chip number" in code to help clarify documentation and
    /// avoid confusion of I2C and RAM address parameters.
    /// </para>
    /// </remarks>
    public abstract class Mb85rcvDevice : DisposableObject
    {
        #region Constants

        /// <summary>
        /// Code which identifies the device type, and are fixed at “1010” for the MB85RC#V.
        /// </summary>
        public const byte TypeCode = 0xa;

        /// <summary>
        /// 7-bit I2C address of the first chip on the bus.
        /// </summary>
        /// <remarks>
        /// When using multiple chips the chip address must be added.
        /// </remarks>
        public const byte DataI2cAddress = TypeCode << 3;

        /// <summary>
        /// 7-bit I2C address used for chip identification.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The device ID sequence is as follows (Master = host, Slave = chip):
        /// 1) Master: Start of transaction.
        /// 2) Master: Byte 1 = 0XF8.
        /// 3) Master: Byte 2 = Address of chip to query, e.g. 0XA0, 0XA4, 0XA8, 0XAA.
        /// 4) Slave: Byte 3...5 device ID data bytes.
        /// 5) Master: End of transaction.
        /// </para>
        /// <para>
        /// With read and write operations (bit 1) both 0XF8 and 0XF9 are used. Hence the device ID sequence for obtaining
        /// the device ID bytes is to call <see cref="I2cDevice.WriteRead(byte[], byte[])"/> writing one byte as
        /// the <see cref="DataI2cAddress"/> set to the chip number to identify (full byte as data, not shifted down to 7-bit address)
        /// then reading the resulting 3 device ID bytes, all in one operation.
        /// </para>
        /// </remarks>
        public const byte DeviceIdI2cAddress = 0xf8 >> 1;

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates an instance with the specified parameters.
        /// </summary>
        /// <param name="chipNumber">Chip number (device address code).</param>
        /// <param name="size">Memory size in bytes.</param>
        /// <remarks>
        /// Inheritors must connect to the I2C device and set it in the <see cref="Hardware"/> property,
        /// which is disposed by this base class.
        /// </remarks>
        [CLSCompliant(false)]
        protected Mb85rcvDevice(byte chipNumber, int size)
        {
            // Validate
            if (chipNumber < 0) throw new ArgumentOutOfRangeException(nameof(chipNumber));
            if (size < 0) throw new ArgumentOutOfRangeException(nameof(size));

            // Initialize members
            ChipNumber = chipNumber;
            Size = size;
        }

        #region IDisposable

        /// <summary>
        /// <see cref="DisposableObject.Dispose(bool)"/>.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            // Only managed resources to dispose
            if (!disposing)
                return;

            // Close device
            Hardware?.Dispose();
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        /// Device address code (chip number).
        /// </summary>
        public byte ChipNumber { get; protected set; }

        /// <summary>
        /// Size of memory in bytes.
        /// </summary>
        public int Size { get; private set; }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Main I2C device of the chip (the lower memory area).
        /// </summary>
        /// <remarks>
        /// Additional I2C devices may exist to facilitate access to higher memory areas.
        /// </remarks>
        [CLSCompliant(false)]
        protected I2cDevice Hardware { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the device identifier of the first device (address code zero).
        /// </summary>
        /// <param name="controller">I2C controller on which the device is connected.</param>
        /// <returns>Device ID.</returns>
        /// <remarks>
        /// It is not possible to get the identifier of other devices until the device
        /// density is known, because one bit of the device address code is used
        /// for the higher address commands with lower densities.
        /// </remarks>
        [CLSCompliant(false)]
        public static Mb85rcvDeviceId GetDeviceId(I2cController controller)
        {
            // Call overloaded method
            return GetDeviceId(controller, DeviceIdI2cAddress, DataI2cAddress);
        }

        /// <summary>
        /// Gets the device identifier by sending the Device ID command to the
        /// specified I2C (device ID) address.
        /// </summary>
        /// <param name="controller">I2C controller on which the device is connected.</param>
        /// <param name="idAddress">
        /// 7-bit I2C device ID command address for the desired chip number
        /// (offset by device address code differently depending on model).
        /// </param>
        /// <param name="dataAddress">
        /// 7-bit I2C memory address for the desired chip number
        /// (offset by device address code differently depending on model).
        /// Required to complete the ID command sequence.
        /// </param>
        /// <returns>Device ID.</returns>
        [CLSCompliant(false)]
        public static Mb85rcvDeviceId GetDeviceId(I2cController controller, byte idAddress, byte dataAddress)
        {
            // Validate
            if (controller == null) throw new ArgumentNullException(nameof(controller));

            // Connect to ID device
            using (var framId = controller.Connect(idAddress))
            {
                // Send ID command sequence, returning device ID bytes
                var dataAddress8bit = (byte)(dataAddress << 1);
                var data = framId.WriteReadBytes(dataAddress8bit, Mb85rcvDeviceId.Size);

                // Return data structure
                return new Mb85rcvDeviceId(data);
            }
        }

        /// <summary>
        /// Reads a single byte at the "current address" (next byte after the last operation).
        /// </summary>
        public byte ReadByte()
        {
            // Get correct I2C device for memory address and flags
            var device = GetI2cDeviceForMemoryAddress(0);

            // Read and return data
            return device.ReadByte();
        }

        /// <summary>
        /// Reads a single byte at the "current address" (next byte after the last operation).
        /// </summary>
        public byte[] ReadPage(int length)
        {
            // Validate
            if (length < 0 || length > Size) throw new ArgumentOutOfRangeException(nameof(length));

            // Get correct I2C device for memory address and flags
            var device = GetI2cDeviceForMemoryAddress(0);

            // Read data with chunking when transfer size exceeds limit
            var resultBuffer = new byte[length];
            var remaining = length; var offset = 0;
            do
            {
                // Check transfer size and reduce when necessary
                var transferSize = remaining;
                if (transferSize > I2cExtensions.MaximumTransferSize)
                    transferSize = I2cExtensions.MaximumTransferSize;

                // Read data
                var buffer = device.ReadBytes(transferSize);
                Array.ConstrainedCopy(buffer, 0, resultBuffer, offset, transferSize);

                // Next transfer when necessary...
                remaining -= transferSize;
                offset += transferSize;
            }
            while (remaining > 0);
            return resultBuffer;
        }

        /// <summary>
        /// Reads a single byte "randomly" at the specified address.
        /// </summary>
        public byte ReadByte(int address)
        {
            // Validate
            if (address < 0 || address > Size - 1) throw new ArgumentOutOfRangeException(nameof(address));

            // Get correct I2C device and data for memory address and flags
            var device = GetI2cDeviceForMemoryAddress(address);
            var addressBytes = GetMemoryAddressBytes(address);

            // Read and return data
            return device.WriteReadByte(addressBytes);
        }

        /// <summary>
        /// Reads a "page" of bytes "sequentially" starting at the specified address.
        /// </summary>
        public byte[] ReadPage(int address, int length)
        {
            // Validate
            if (address < 0 || address > Size - 1) throw new ArgumentOutOfRangeException(nameof(address));
            if (length < 0 || length > Size - address) throw new ArgumentOutOfRangeException(nameof(length));

            // Get correct I2C device and data for memory address and flags
            var device = GetI2cDeviceForMemoryAddress(address);

            // Read data with chunking when transfer size exceeds limit
            var resultBuffer = new byte[length];
            var remaining = length; var offset = 0;
            do
            {
                var addressBytes = GetMemoryAddressBytes(address);
                var addressBytesLength = addressBytes.Length;

                // Check transfer size and reduce when necessary
                var transferSize = remaining;
                if (transferSize > I2cExtensions.MaximumTransferSize)
                    transferSize = I2cExtensions.MaximumTransferSize;

                // Read data
                var buffer = device.WriteReadBytes(addressBytes, transferSize);
                Array.ConstrainedCopy(buffer, 0, resultBuffer, offset, transferSize);

                // Next transfer when necessary...
                remaining -= transferSize;
                offset += transferSize;
                address += transferSize;
            }
            while (remaining > 0);
            return resultBuffer;
        }

        /// <summary>
        /// Writes a single byte at the specified address.
        /// </summary>
        public void WriteByte(int address, byte data)
        {
            // Validate
            if (address < 0 || address > Size - 1) throw new ArgumentOutOfRangeException(nameof(address));

            // Get correct I2C device and data for memory address and flags
            var device = GetI2cDeviceForMemoryAddress(address);
            var addressBytes = GetMemoryAddressBytes(address);

            // Write data
            device.WriteJoinByte(addressBytes, data);
        }

        /// <summary>
        /// Writes a "page" of multiple bytes starting at the specified address.
        /// </summary>
        public void WritePage(int address, byte[] data)
        {
            // Call overloaded method
            WritePage(address, data, 0, data.Length);
        }

        /// <summary>
        /// Writes a "page" of multiple bytes starting at the specified address.
        /// </summary>
        public void WritePage(int address, byte[] data, int offset, int length)
        {
            // Validate
            if (address < 0 || address > Size - 1) throw new ArgumentOutOfRangeException(nameof(address));
            if (data == null || data.Length < 1) throw new ArgumentNullException(nameof(data));
            if (offset < 0 || offset >= data.Length) throw new ArgumentOutOfRangeException(nameof(offset));
            if (length < 1 || offset + length > data.Length || offset + length > Size - address)
                throw new ArgumentOutOfRangeException(nameof(data));

            // Get correct I2C device and data for memory address and flags
            var device = GetI2cDeviceForMemoryAddress(address);

            // Write data with chunking when transfer size exceeds limit
            var remaining = length;
            do
            {
                var addressBytes = GetMemoryAddressBytes(address);
                var addressBytesLength = addressBytes.Length;

                // Check transfer size and reduce when necessary
                var transferSize = addressBytesLength + remaining;
                if (transferSize > I2cExtensions.MaximumTransferSize)
                    transferSize = I2cExtensions.MaximumTransferSize;
                var transferDataSize = transferSize - addressBytesLength;

                // Write data
                var buffer = new byte[transferSize];
                Array.Copy(addressBytes, buffer, addressBytesLength);
                Array.ConstrainedCopy(data, offset, buffer, addressBytesLength, transferDataSize);
                device.Write(buffer);

                // Next transfer when necessary...
                remaining -= transferDataSize;
                offset += transferDataSize;
                address += transferDataSize;
            }
            while (remaining > 0);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the <see cref="I2cDevice"/> required to communicate with a specific memory address.
        /// </summary>
        /// <param name="address">Memory address. Leave zero when performing commands which operate on the current address.</param>
        /// <remarks>
        /// Because the chip mixes sub-address bits and flags with the slave address and the Windows
        /// <see cref="I2cDevice.ConnectionSettings"/> cannot be modified after creation, it is
        /// necessary to use multiple <see cref="I2cDevice"/> instances for each.
        /// <para>
        /// This base class implementation is the least complex like newer/larger chips, only
        /// requiring different I2C addresses for the read/write flag (no memory address bits).
        /// Older chips should override this implementation to index into the additional
        /// I2C devices they need to access all memory addresses.
        /// </para>
        /// </remarks>
        [CLSCompliant(false)]
        protected virtual I2cDevice GetI2cDeviceForMemoryAddress(int address)
        {
            return Hardware;
        }

        /// <summary>
        /// Gets the I2C command memory address bytes for the specified logical address.
        /// </summary>
        /// <param name="address"></param>
        /// <returns>
        /// Byte array which can be written to request the specified memory address,
        /// assuming the correct I2C device is being used as provided by <see cref="GetI2cDeviceForMemoryAddress(int)"/>
        /// which may include the MSB in it's I2C address.
        /// </returns>
        /// <remarks>
        /// Besides being split into bytes, some older/smaller chips separate the MSB
        /// into the I2C device address.
        /// </remarks>
        public abstract byte[] GetMemoryAddressBytes(int address);

        #endregion
    }
}
