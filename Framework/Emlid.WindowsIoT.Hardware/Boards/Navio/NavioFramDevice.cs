using Emlid.WindowsIot.Common;
using Emlid.WindowsIot.Hardware.Components.Mb85rcv;
using Emlid.WindowsIot.Hardware.System;
using System;

namespace Emlid.WindowsIot.Hardware.Boards.Navio
{
    /// <summary>
    /// Navio FRAM (MB85RC256V or MB85RC04V hardware device), connected via I2C.
    /// </summary>
    public sealed class NavioFramDevice : DisposableObject, INavioFramDevice
    {
        #region Constants

        /// <summary>
        /// I2C controller index of the chip on the Navio board.
        /// </summary>
        public const int I2cControllerIndex = 0;

        /// <summary>
        /// MB85RC#V chip number (device address code).
        /// </summary>
        public const byte ChipNumber = 0;

        /// <summary>
        /// FRAM device ID on the Navio.
        /// </summary>
        public static readonly Mb85rcvDeviceId Navio1DeviceId = new Mb85rcvDeviceId(Mb85rcvDeviceId.FujitsuManufacturerId, Mb85rc04vDevice.Density, 0x10);

        /// <summary>
        /// FRAM device ID on the Navio+.
        /// </summary>
        public static readonly Mb85rcvDeviceId Navio1PlusDeviceId = new Mb85rcvDeviceId(Mb85rcvDeviceId.FujitsuManufacturerId, Mb85rc256vDevice.Density, 0x10);

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates an instance of the correct type depending on the Navio model.
        /// </summary>
        public NavioFramDevice(NavioHardwareModel model)
        {
            // Get I2C controller for FRAM chip
            DeviceProvider.Initialize();
            var controller = DeviceProvider.I2c[I2cControllerIndex];

            // Create model specific device
            switch (model)
            {
                case NavioHardwareModel.Navio1:

                    // Create 512 byte device for Navio
                    _device = new Mb85rc04vDevice(controller, ChipNumber);
                    break;

                case NavioHardwareModel.Navio1Plus:

                    // Create 32KiB device for Navio+
                    _device = new Mb85rc256vDevice(controller, ChipNumber);
                    break;

                default:
                    throw new NotImplementedException();
            }
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
            // Only managed resources to dispose
            if (!disposing)
                return;

            // Close device
            _device?.Dispose();
        }

        #endregion

        #endregion

        #region Private Fields

        /// <summary>
        /// FRAM device specific to the requested Navio model.
        /// </summary>
        private Mb85rcvDevice _device;

        #endregion

        #region Public Properties

        /// <summary>
        /// Size of memory in bytes.
        /// </summary>
        public int Size { get { return _device.Size; } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reads a single byte at the "current address" (next byte after the last operation).
        /// </summary>
        public byte ReadByte()
        {
            // Call method on contained instance
            return _device.ReadByte();
        }

        /// <summary>
        /// Reads a single byte at the "current address" (next byte after the last operation).
        /// </summary>
        /// <param name="length">Length of page to read in bytes.</param>
        public byte[] ReadPage(int length)
        {
            // Call method on contained instance
            return _device.ReadPage(length);
        }

        /// <summary>
        /// Reads a single byte "randomly" at the specified address.
        /// </summary>
        /// <param name="address">Address at which to read.</param>
        public byte ReadByte(int address)
        {
            // Call method on contained instance
            return _device.ReadByte(address);
        }

        /// <summary>
        /// Reads a "page" of bytes "sequentially" starting at the specified address.
        /// </summary>
        /// <param name="address">Address at which to read.</param>
        /// <param name="length">Length of page to read in bytes.</param>
        public byte[] ReadPage(int address, int length)
        {
            // Call method on contained instance
            return _device.ReadPage(address, length);
        }

        /// <summary>
        /// Writes a single byte at the specified address.
        /// </summary>
        /// <param name="address">Address at which to write.</param>
        /// <param name="data">Source data buffer to write from.</param>
        public void WriteByte(int address, byte data)
        {
            // Call method on contained instance
            _device.WriteByte(address, data);
        }

        /// <summary>
        /// Writes a "page" of multiple bytes starting at the specified address.
        /// </summary>
        /// <param name="address">Address at which to write.</param>
        /// <param name="data">Source data buffer to write from.</param>
        public void WritePage(int address, byte[] data)
        {
            // Call method on contained instance
            _device.WritePage(address, data);
        }

        /// <summary>
        /// Writes a "page" of multiple bytes starting at the specified address.
        /// </summary>
        /// <param name="address">Address at which to write.</param>
        /// <param name="data">Source data buffer to write from.</param>
        /// <param name="offset">Offset in the source buffer at which to start reading data to write.</param>
        /// <param name="length">Length of page to write in bytes.</param>
        public void WritePage(int address, byte[] data, int offset, int length)
        {
            // Call method on contained instance
            _device.WritePage(address, data, offset, length);
        }

        #endregion
    }
}
