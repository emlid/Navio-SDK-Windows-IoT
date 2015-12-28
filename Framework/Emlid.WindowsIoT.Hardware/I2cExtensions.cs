using System;
using Windows.Devices.I2c;

namespace Emlid.WindowsIot.Hardware
{
    /// <summary>
    /// Extensions for work with I2C devices.
    /// </summary>
    [CLSCompliant(false)]
    public static class I2cExtensions
    {
        #region Device Extensions

        #region Read

        /// <summary>
        /// Reads a byte value.
        /// </summary>
        /// <param name="device">Device to use.</param>
        /// <param name="address">Address from which to read.</param>
        /// <returns>Data byte.</returns>
        public static byte ReadByte(this I2cDevice device, byte address)
        {
            // Call overloaded method
            return ReadBytes(device, address, 1)[0];
        }

        /// <summary>
        /// Reads one or more bytes.
        /// </summary>
        /// <param name="device">Device to use.</param>
        /// <param name="address">Address from whcih to read.</param>
        /// <param name="size">Amount of data to read.</param>
        /// <returns>Data byte(s).</returns>
        public static byte[] ReadBytes(this I2cDevice device, byte address, int size)
        {
            var buffer = new byte[size];
            device.WriteRead(new[] { address }, buffer);
            return buffer;
        }

        /// <summary>
        /// Reads one or more bytes into an existing buffer.
        /// </summary>
        /// <param name="device">Device to use.</param>
        /// <param name="address">Address from whcih to read.</param>
        /// <param name="size">Amount of data to read.</param>
        /// <param name="buffer">Target buffer.</param>
        /// <param name="offset">Target buffer offset.</param>
        /// <returns>Data byte(s).</returns>
        public static void ReadBytes(this I2cDevice device, byte address, int size, byte[] buffer, int offset)
        {
            // Call overloaded method
            var data = ReadBytes(device, address, size);

            // Copy data to target buffer
            Array.ConstrainedCopy(data, 0, buffer, offset, size);
        }

        /// <summary>
        /// Tests one or more bits by reading the byte value then masking the result.
        /// </summary>
        /// <remarks>
        /// Commonly used to test register flags.
        /// </remarks>
        /// <param name="device">Device to use.</param>
        /// <param name="address">Address from which to read.</param>
        /// <param name="mask">Index of the bit to read, zero based.</param>
        /// <returns>True when the result was positive (any bits in the mask were set).</returns>
        public static bool ReadBit(this I2cDevice device, byte address, byte mask)
        {
            // Read byte
            var value = ReadByte(device, address);

            // Apply mask and return true when set
            return (value & mask) != 0;
        }

        #endregion

        #region Write

        /// <summary>
        /// Writes a byte value.
        /// </summary>
        /// <param name="device">Device to use.</param>
        /// <param name="address">Address at which to write.</param>
        /// <param name="value">Value to write.</param>
        public static void WriteByte(this I2cDevice device, byte address, byte value)
        {
            device.Write(new[] { address, value });
        }

        /// <summary>
        /// Writes one or more bytes.
        /// </summary>
        /// <param name="device">Device to use.</param>
        /// <param name="address">Address at which to write.</param>
        /// <param name="data">Data to write.</param>
        public static void WriteBytes(this I2cDevice device, byte address, byte[] data)
        {
            var buffer = new byte[data.Length + 1];
            buffer[0] = address;
            Array.ConstrainedCopy(data, 0, buffer, 1, data.Length);
            device.Write(buffer);
        }

        /// <summary>
        /// Sets or clears one or more bits.
        /// </summary>
        /// <param name="device">Device to use.</param>
        /// <param name="address">Address at which to write.</param>
        /// <param name="mask">
        /// Mask of the bit to set or clear according to value.
        /// Supports setting or clearing multiple bits.
        /// </param>
        /// <param name="value">Value of the bits, i.e. set or clear.</param>
        /// <returns>Value written.</returns>
        /// <remarks>
        /// Commonly used to set register flags.
        /// Reads the current byte value, merges the positive or negative bit mask according to value,
        /// then writes the modifed byte back.
        /// </remarks>
        public static byte WriteBit(this I2cDevice device, byte address, byte mask, bool value)
        {
            // Read existing byte
            var oldByte = ReadByte(device, address);

            // Merge bit (set or clear bit accordingly)
            var newByte = value ? (byte)(oldByte | mask) : (byte)(oldByte & ~mask);

            // Write new byte
            WriteByte(device, address, newByte);

            // Return the value written.
            return newByte;
        }

        #endregion

        #region Read Types

        /// <summary>
        /// <see cref="ReadBytes(I2cDevice, byte, int)"/> then converts to a <see cref="UInt16"/>.
        /// </summary>
        /// <param name="device">Device to use.</param>
        /// <param name="address">Address from which to read.</param>
        /// <returns>Converted data bytes.</returns>
        public static ushort ReadUInt16(this I2cDevice device, byte address)
        {
            // Call overloaded method
            var data = ReadBytes(device, address, sizeof(UInt16));
            return (ushort)(data[0] << 8 | data[1]);
        }

        #endregion

        #endregion
    }
}
