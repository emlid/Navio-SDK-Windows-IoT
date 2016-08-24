using System;
using Windows.Devices.I2c;

namespace Emlid.WindowsIot.Hardware.Components
{
    /// <summary>
    /// Extensions for work with I2C devices.
    /// </summary>
    [CLSCompliant(false)]
    public static class I2cExtensions
    {
        #region Read

        /// <summary>
        /// Writes data then reads a single byte result.
        /// </summary>
        /// <param name="device">Device to use.</param>
        /// <param name="writeData">Data to write.</param>
        /// <returns>Read data byte.</returns>
        public static byte WriteReadByte(this I2cDevice device, byte writeData)
        {
            // Call overloaded method
            return WriteReadBytes(device, new[] { writeData }, 1)[0];
        }

        /// <summary>
        /// Writes data then reads a single byte result.
        /// </summary>
        /// <param name="device">Device to use.</param>
        /// <param name="writeData">Data to write.</param>
        /// <returns>Read data byte.</returns>
        public static byte WriteReadByte(this I2cDevice device, byte[] writeData)
        {
            // Call overloaded method
            return WriteReadBytes(device, writeData, 1)[0];
        }

        /// <summary>
        /// Writes data then reads a single byte result.
        /// </summary>
        /// <param name="device">Device to use.</param>
        /// <param name="writeData">Data to write.</param>
        /// <param name="size">Amount of data to read.</param>
        /// <returns>Read data bytes.</returns>
        public static byte[] WriteReadBytes(this I2cDevice device, byte writeData, int size)
        {
            // Call overloaded method
            return WriteReadBytes(device, new[] { writeData }, size);
        }

        /// <summary>
        /// Writes data then reads one or more bytes.
        /// </summary>
        /// <param name="device">Device to use.</param>
        /// <param name="writeData">Data to write.</param>
        /// <param name="size">Amount of data to read.</param>
        /// <returns>Read data bytes.</returns>
        public static byte[] WriteReadBytes(this I2cDevice device, byte[] writeData, int size)
        {
            var buffer = new byte[size];
            device.WriteRead(writeData, buffer);
            return buffer;
        }

        /// <summary>
        /// Writes data then reads one or more bytes into an existing buffer.
        /// </summary>
        /// <param name="device">Device to use.</param>
        /// <param name="writeData">Data to write.</param>
        /// <param name="size">Amount of data to read.</param>
        /// <param name="buffer">Target buffer.</param>
        /// <param name="offset">Target buffer offset.</param>
        public static void WriteReadBytes(this I2cDevice device, byte writeData, int size, byte[] buffer, int offset)
        {
            // Call overloaded method
            WriteReadBytes(device, new[] { writeData }, size, buffer, offset);
        }

        /// <summary>
        /// Writes data then reads one or more bytes into an existing buffer.
        /// </summary>
        /// <param name="device">Device to use.</param>
        /// <param name="writeData">Data to write.</param>
        /// <param name="size">Amount of data to read.</param>
        /// <param name="buffer">Target buffer.</param>
        /// <param name="offset">Target buffer offset.</param>
        public static void WriteReadBytes(this I2cDevice device, byte[] writeData, int size, byte[] buffer, int offset)
        {
            // Call overloaded method
            var data = WriteReadBytes(device, writeData, size);

            // Copy data to target buffer
            Array.ConstrainedCopy(data, 0, buffer, offset, size);
        }

        /// <summary>
        /// Writes data, reads a byte result then tests on or more bits.
        /// </summary>
        /// <remarks>
        /// Commonly used to test register flags.
        /// </remarks>
        /// <param name="device">Device to use.</param>
        /// <param name="writeData">Data to write.</param>
        /// <param name="mask">Index of the bit to read, zero based.</param>
        /// <returns>True when the result was positive (any bits in the mask were set).</returns>
        public static bool WriteReadBit(this I2cDevice device, byte writeData, byte mask)
        {
            // Call overloaded method
            return WriteReadBit(device, new[] { writeData }, mask);
        }

        /// <summary>
        /// Writes data, reads a byte result then tests on or more bits.
        /// </summary>
        /// <remarks>
        /// Commonly used to test register flags.
        /// </remarks>
        /// <param name="device">Device to use.</param>
        /// <param name="writeData">Data to write.</param>
        /// <param name="mask">Index of the bit to read, zero based.</param>
        /// <returns>True when the result was positive (any bits in the mask were set).</returns>
        public static bool WriteReadBit(this I2cDevice device, byte[] writeData, byte mask)
        {
            // Read byte
            var value = WriteReadByte(device, writeData);

            // Apply mask and return true when set
            return (value & mask) != 0;
        }

        #endregion

        #region Write

        /// <summary>
        /// Joins two byte values then writes them.
        /// </summary>
        /// <param name="device">Device to use.</param>
        /// <param name="data1">First part of data to write.</param>
        /// <param name="data2">Second part of data to write.</param>
        public static void WriteJoinByte(this I2cDevice device, byte data1, byte data2)
        {
            device.Write(new[] { data1, data2 });
        }

        /// <summary>
        /// Joins two byte values then writes them.
        /// </summary>
        /// <param name="device">Device to use.</param>
        /// <param name="data1">First part of data to write.</param>
        /// <param name="data2">Second part of data to write.</param>
        public static void WriteJoinByte(this I2cDevice device, byte[] data1, byte data2)
        {
            // Call overloaded method
            WriteJoinBytes(device, data1, new[] { data2 });
        }

        /// <summary>
        /// Joins two byte values then writes them.
        /// </summary>
        /// <param name="device">Device to use.</param>
        /// <param name="data1">First part of data to write.</param>
        /// <param name="data2">Second part of data to write.</param>
        public static void WriteJoinBytes(this I2cDevice device, byte data1, byte[] data2)
        {
            // Call overloaded method
            WriteJoinBytes(device, new[] { data1 }, data2);
        }

        /// <summary>
        /// Joins two byte values then writes them.
        /// </summary>
        /// <param name="device">Device to use.</param>
        /// <param name="data1">First part of data to write.</param>
        /// <param name="data2">Second part of data to write.</param>
        public static void WriteJoinBytes(this I2cDevice device, byte[] data1, byte[] data2)
        {
            var addressLength = data1.Length;
            var dataLength = data2.Length;
            var buffer = new byte[addressLength + dataLength];
            Array.Copy(data1, buffer, addressLength);
            Array.ConstrainedCopy(data2, 0, buffer, addressLength, dataLength);
            device.Write(buffer);
        }

        /// <summary>
        /// Sets or clears one or more bits.
        /// </summary>
        /// <param name="device">Device to use.</param>
        /// <param name="writeData">Data to write.</param>
        /// <param name="mask">
        /// Mask of the bit to set or clear according to value.
        /// Supports setting or clearing multiple bits.
        /// </param>
        /// <param name="value">Value of the bits, i.e. set or clear.</param>
        /// <returns>Value written.</returns>
        /// <remarks>
        /// Commonly used to set register flags.
        /// Reads the current byte value, merges the positive or negative bit mask according to value,
        /// then writes the modified byte back.
        /// </remarks>
        public static byte WriteReadWriteBit(this I2cDevice device, byte writeData, byte mask, bool value)
        {
            // Read existing byte
            var oldByte = WriteReadByte(device, writeData);

            // Merge bit (set or clear bit accordingly)
            var newByte = value ? (byte)(oldByte | mask) : (byte)(oldByte & ~mask);

            // Write new byte
            WriteJoinByte(device, writeData, newByte);

            // Return the value written.
            return newByte;
        }

        #endregion
    }
}
