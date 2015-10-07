using System;
using Windows.Devices.I2c;

namespace Emlid.WindowsIoT.Hardware
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
        /// Reads a byte value from a register.
        /// </summary>
        /// <param name="device">Device to use.</param>
        /// <param name="register">Register to read.</param>
        /// <returns>Register value.</returns>
        public static byte ReadByte(this I2cDevice device, byte register)
        {
            // Call overloaded method
            return ReadBytes(device, register, 1)[0];
        }

        /// <summary>
        /// Reads one or more bytes from a register.
        /// </summary>
        /// <param name="device">Device to use.</param>
        /// <param name="register">Register to read.</param>
        /// <param name="size">Amount of data to read.</param>
        /// <returns>Register value byte(s).</returns>
        public static byte[] ReadBytes(this I2cDevice device, byte register, int size)
        {
            var buffer = new byte[size];
            device.WriteRead(new[] { register }, buffer);
            return buffer;
        }

        /// <summary>
        /// Tests one or more bits of a register by reading the byte value then masking the result.
        /// </summary>
        /// <param name="device">Device to use.</param>
        /// <param name="register">Register to read.</param>
        /// <param name="mask">Index of the bit to read, zero based.</param>
        /// <returns>True when the result was positive (any bits in the mask were set).</returns>
        public static bool ReadBit(this I2cDevice device, byte register, byte mask)
        {
            // Read byte
            var value = ReadByte(device, register);

            // Apply mask and return true when set
            return (value & mask) != 0;
        }

        #endregion

        #region Write

        /// <summary>
        /// Writes a byte value to a register.
        /// </summary>
        /// <param name="device">Device to use.</param>
        /// <param name="register">Register to write.</param>
        /// <param name="value">Value to write.</param>
        public static void WriteByte(this I2cDevice device, byte register, byte value)
        {
            device.Write(new[] { register, value });
        }

        /// <summary>
        /// Writes one or more bytes to a register.
        /// </summary>
        /// <param name="device">Device to use.</param>
        /// <param name="register">Register to write.</param>
        /// <param name="data">Data to write.</param>
        public static void WriteBytes(this I2cDevice device, byte register, byte[] data)
        {
            var buffer = new byte[data.Length + 1];
            buffer[0] = register;
            Array.ConstrainedCopy(data, 0, buffer, 1, data.Length);
            device.Write(buffer);
        }

        /// <summary>
        /// Sets or clears one or more bits in a register.
        /// </summary>
        /// <param name="device">Device to use.</param>
        /// <param name="register">Register to set.</param>
        /// <param name="mask">
        /// Mask of the bit to set or clear according to value.
        /// Supports setting or clearing multiple bits.
        /// </param>
        /// <param name="value">Value of the bits, i.e. set or clear.</param>
        /// <returns>Value written to the register.</returns>
        /// <remarks>
        /// Reads the current byte value, merges the positive or negative bit mask according to value,
        /// then writes the modifed byte back.
        /// </remarks>
        public static byte WriteBit(this I2cDevice device, byte register, byte mask, bool value)
        {
            // Read existing byte
            var oldByte = ReadByte(device, register);

            // Merge bit (set or clear bit accordingly)
            var newByte = value ? (byte)(oldByte | mask) : (byte)(oldByte & ~mask);

            // Write new byte
            WriteByte(device, register, newByte);

            // Return the value written.
            return newByte;
        }

        #endregion

        #endregion
    }
}
