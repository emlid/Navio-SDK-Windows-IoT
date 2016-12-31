namespace Emlid.WindowsIot.Hardware.Boards.Navio
{
    /// <summary>
    /// Navio FRAM device interface.
    /// </summary>
    public interface INavioFramDevice
    {
        #region Properties

        /// <summary>
        /// Size of memory in bytes.
        /// </summary>
        int Size { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Reads a single byte at the "current address" (next byte after the last operation).
        /// </summary>
        byte ReadByte();

        /// <summary>
        /// Reads a single byte at the "current address" (next byte after the last operation).
        /// </summary>
        /// <param name="length">Length of page to read in bytes.</param>
        byte[] ReadPage(int length);

        /// <summary>
        /// Reads a single byte "randomly" at the specified address.
        /// </summary>
        /// <param name="address">Address at which to read.</param>
        byte ReadByte(int address);

        /// <summary>
        /// Reads a "page" of bytes "sequentially" starting at the specified address.
        /// </summary>
        /// <param name="address">Address at which to read.</param>
        /// <param name="length">Length of page to read in bytes.</param>
        byte[] ReadPage(int address, int length);

        /// <summary>
        /// Writes a single byte at the specified address.
        /// </summary>
        /// <param name="address">Address at which to write.</param>
        /// <param name="data">Source data buffer to write from.</param>
        void WriteByte(int address, byte data);

        /// <summary>
        /// Writes a "page" of multiple bytes starting at the specified address.
        /// </summary>
        /// <param name="address">Address at which to write.</param>
        /// <param name="data">Source data buffer to write from.</param>
        void WritePage(int address, byte[] data);

        /// <summary>
        /// Writes a "page" of multiple bytes starting at the specified address.
        /// </summary>
        /// <param name="address">Address at which to write.</param>
        /// <param name="data">Source data buffer to write from.</param>
        /// <param name="offset">Offset in the source buffer at which to start reading data to write.</param>
        /// <param name="length">Length of page to write in bytes.</param>
        void WritePage(int address, byte[] data, int offset, int length);

        #endregion
    }
}
