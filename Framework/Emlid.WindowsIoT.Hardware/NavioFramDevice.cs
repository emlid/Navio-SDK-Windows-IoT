using Emlid.WindowsIot.Common;
using System;
using System.Diagnostics;

namespace Emlid.WindowsIot.Hardware
{
    /// <summary>
    /// Navio FRAM (MB85RC256V or MB85RC04V hardware device), connected via I2C.
    /// </summary>
    public class NavioFramDevice : DisposableObject
    {
        #region Constants

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
            // Create model specific device
            switch (model)
            {
                case NavioHardwareModel.Navio:
                    Hardware = new Mb85rc04vDevice(I2cAddress, true, true);
                    break;

                case NavioHardwareModel.NavioPlus:
                    Hardware = new Mb85rc256vDevice(I2cAddress, true, true);
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
            // Do nothing when already disposed
            if (IsDisposed) return;

            // Dispose
            try
            {
                // Dispose managed resource during dispose
                if (disposing)
                {
                    if (Hardware != null)
                        Hardware.Dispose();
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
        /// FRAM device specific to the requested Navio model.
        /// </summary>
        [CLSCompliant(false)]
        protected Mb85rcvDevice Hardware { get; private set; }

        #endregion

        #region Public Properties

        /// <summary>
        /// Controller ID (I2C master ID).
        /// </summary>
        public string ControllerId { get { return Hardware.ControllerId; } }

        /// <summary>
        /// Device ID (I2C slave ID).
        /// </summary>
        public string Id { get { return Hardware.Id; } }

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
