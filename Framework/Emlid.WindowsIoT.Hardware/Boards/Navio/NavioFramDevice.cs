using Emlid.WindowsIot.Common;
using Emlid.WindowsIot.Hardware.Components.Mb85rcv;
using System;
using System.Globalization;

namespace Emlid.WindowsIot.Hardware.Boards.Navio
{
    /// <summary>
    /// Navio FRAM (MB85RC256V or MB85RC04V hardware device), connected via I2C.
    /// </summary>
    public class NavioFramDevice : DisposableObject
    {
        #region Constants

        /// <summary>
        /// I2C controller index of the chip on the Navio board.
        /// </summary>
        public const int I2cControllerIndex = 0;

        /// <summary>
        /// I2C address of the MB85RC#V on the Navio board.
        /// </summary>
        public const int I2cAddress = 0x50;

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates an instance of the correct type depending on the Navio model.
        /// </summary>
        public NavioFramDevice(NavioHardwareModel model)
        {
            // Connect to I2C device
            var device = NavioHardwareProvider.ConnectI2c(I2cControllerIndex, I2cAddress);
            if (device == null)
            {
                // Initialization error
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    Resources.Strings.I2cErrorDeviceNotFound, I2cAddress, I2cControllerIndex));
            }

            // Create model specific device
            switch (model)
            {
                case NavioHardwareModel.Navio:
                    
                    // Connect to upper I2C device
                    var upperAddress = Mb85rc04vDevice.GetUpperI2cAddress(I2cAddress);
                    var upperDevice = NavioHardwareProvider.ConnectI2c(I2cControllerIndex, upperAddress);
                    if (device == null)
                    {
                        // Initialization error
                        throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                            Resources.Strings.I2cErrorDeviceNotFound, upperAddress, I2cControllerIndex));
                    }

                    // Create 512 byte device for Navio
                    Hardware = new Mb85rc04vDevice(device, upperDevice);
                    break;

                case NavioHardwareModel.NavioPlus:

                    // Create 32KiB device for Navio+
                    Hardware = new Mb85rc256vDevice(device);
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
            Hardware?.Dispose();
        }

        #endregion

        #endregion

        #region Protected Properties

        /// <summary>
        /// FRAM device specific to the requested Navio model.
        /// </summary>
        [CLSCompliant(false)]
        protected Mb85rcvDevice Hardware { get; private set; }

        #endregion

        #region Public Properties

        /// <summary>
        /// Size of memory in bytes.
        /// </summary>
        public int Size { get { return Hardware.Size; } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reads a single byte at the "current address" (next byte after the last operation).
        /// </summary>
        public byte ReadByte()
        {
            // Call method on contained instance
            return Hardware.ReadByte();
        }

        /// <summary>
        /// Reads a single byte at the "current address" (next byte after the last operation).
        /// </summary>
        public byte[] ReadPage(int length)
        {
            // Call method on contained instance
            return Hardware.ReadPage(length);
        }

        /// <summary>
        /// Reads a single byte "randomly" at the specified address.
        /// </summary>
        public byte ReadByte(int address)
        {
            // Call method on contained instance
            return Hardware.ReadByte(address);
        }

        /// <summary>
        /// Reads a "page" of bytes "sequentially" starting at the specified address.
        /// </summary>
        public byte[] ReadPage(int address, int length)
        {
            // Call method on contained instance
            return Hardware.ReadPage(address, length);
        }

        /// <summary>
        /// Writes a single byte at the specified address.
        /// </summary>
        public void WriteByte(int address, byte data)
        {
            // Call method on contained instance
            Hardware.WriteByte(address, data);
        }

        /// <summary>
        /// Writes a "page" of multiple bytes starting at the specified address.
        /// </summary>
        public void WritePage(int address, byte[] data)
        {
            // Call method on contained instance
            Hardware.WritePage(address, data);
        }

        /// <summary>
        /// Writes a "page" of multiple bytes starting at the specified address.
        /// </summary>
        public void WritePage(int address, byte[] data, int offset, int length)
        {
            // Call method on contained instance
            Hardware.WritePage(address, data, offset, length);
        }

        #endregion
    }
}
