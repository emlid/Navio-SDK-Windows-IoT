using CodeForDotNet.Collections;
using System;

namespace Emlid.WindowsIot.Hardware.Components.Px4io
{
    /// <summary>
    /// PX4IO serial interface protocol packet.
    /// </summary>
    /// <see href="https://github.com/emlid/navio-rcio-linux-driver/blob/master/protocol.h"/>
    /// <remarks>
    /// <para>
    /// Communication is performed via writes to and reads from 16-bit virtual
    /// registers organized into pages of 255 registers each.
    /// </para>
    /// <para>
    /// The first two bytes of each write select a page and offset address
    /// respectively. Subsequent reads and writes increment the offset within
    /// the page.
    /// </para>
    /// <para>
    /// Some pages are read or write-only.
    /// </para>
    /// <para>
    /// Note that some pages may permit offset values greater than 255, which
    /// can only be achieved by long writes. The offset does not wrap.
    /// </para>
    /// <para>
    /// Writes to unimplemented registers are ignored. Reads from unimplemented
    /// registers return undefined values.
    /// </para>
    /// <para>
    /// As convention, values that would be floating point in other parts of
    /// the PX4 system are expressed as signed integer values scaled by 10000,
    /// e.g.control values range from -10000..10000.  Use the REG_TO_SIGNED and
    /// SIGNED_TO_REG macros to convert between register representation and
    /// the signed version, and REG_TO_FLOAT/FLOAT_TO_REG to convert to float.
    /// </para>
    /// <para>
    /// Note that the implementation of readable pages prefers registers within
    /// readable pages to be densely packed. Page numbers do not need to be
    /// packed.
    /// </para>
    /// </remarks>
    [CLSCompliant(false)]
    public class Px4ioPacket
    {
        #region Constants

        /// <summary>
        /// Highest compatible protocol version.
        /// </summary>
        public const int Version = 4;

        /// <summary>
        /// Header size in bytes.
        /// </summary>
        public const int HeaderSize = 4;

        /// <summary>
        /// Data <see cref="Registers"/> size in bytes.
        /// </summary>
        public const int DataSize = sizeof(ushort) * RegistersMaximum;

        /// <summary>
        /// Total size of a packet in bytes.
        /// </summary>
        /// <remarks>
        /// The full package size is always transmitted or received regardless of how many
        /// registers actually contain data.
        /// </remarks>
        public const int Size = HeaderSize + DataSize;

        /// <summary>
        /// Maximum number of <see cref="Registers"/>.
        /// </summary>
        /// <remarks>
        /// The full package size is always transmitted or received regardless of how many
        /// registers actually contain data.
        /// </remarks>
        public const int RegistersMaximum = 32;

        /// <summary>
        /// Masks the code bits.
        /// </summary>
        public const byte CodeMask = 0xc0;

        /// <summary>
        /// Masks the count bits.
        /// </summary>
        public const byte CountMask = 0x3f;

        #endregion Constants

        #region Lifetime

        /// <summary>
        /// Creates an instance with the specified values.
        /// </summary>
        public Px4ioPacket(byte code, byte page, byte offset, byte count)
        {
            // Validate
            if ((code & CountMask) != 0) throw new ArgumentOutOfRangeException(nameof(code));
            if ((count & CodeMask) != 0) throw new ArgumentOutOfRangeException(nameof(count));

            // Initialize members
            CountCode = (byte)(code | count);
            Page = page;
            Offset = offset;
            Crc = 0;
            Registers = new ushort[count];
        }

        /// <summary>
        /// Creates an instance with the specified values.
        /// </summary>
        public Px4ioPacket(byte code, byte page, byte offset, ushort[] values)
            : this(code, page, offset, (byte)(values?.Length ?? 0))
        {
            // Validate
            if (values == null) throw new ArgumentNullException(nameof(values));

            // Copy register values
            Array.Copy(values, Registers, values.Length);
        }

        /// <summary>
        /// Creates an instance from raw data bytes.
        /// </summary>
        public Px4ioPacket(byte[] buffer)
        {
            // Validate
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (buffer.Length != Size) throw new ArgumentOutOfRangeException(nameof(buffer));

            // Copy header values
            var offset = 0;
            var countCode = buffer[offset++];
            CountCode = countCode;
            var count = countCode & CountMask;
            Crc = buffer[offset++];
            Page = buffer[offset++];
            Offset = buffer[offset++];
            Registers = new ushort[count];

            // Copy register values
            for (int index = 0; index < count; index++, offset += 2)
                Registers[index] = BitConverter.ToUInt16(buffer, offset);
        }

        #endregion Lifetime

        #region Operators

        /// <summary>
        /// Tests two objects of this type for equality by value.
        /// </summary>
        public static bool operator ==(Px4ioPacket left, Px4ioPacket right)
        {
            return !ReferenceEquals(left, null)
                ? left.Equals(right)
                : ReferenceEquals(right, null);
        }

        /// <summary>
        /// Tests two objects of this type for inequality by value.
        /// </summary>
        public static bool operator !=(Px4ioPacket left, Px4ioPacket right)
        {
            return !ReferenceEquals(left, null)
                ? !left.Equals(right)
                : !ReferenceEquals(right, null);
        }

        /// <summary>
        /// Compares this object with another by value.
        /// </summary>
        /// <param name="value">Object with which to compare by value.</param>
        public override bool Equals(object value)
        {
            // Call overloaded method
            return Equals(value as Px4ioPacket);
        }

        /// <summary>
        /// Compares this object with another of the same type by value.
        /// </summary>
        /// <param name="value">Object with which to compare by value.</param>
        public bool Equals(Px4ioPacket value)
        {
            // Check null
            if (ReferenceEquals(value, null))
                return false;

            // Compare values
            return
                value.CountCode == CountCode &&
                value.Crc == Crc &&
                value.Offset == Offset &&
                value.Page == Page &&
                ArrayExtensions.AreEqual(value.Registers, Registers);
        }

        /// <summary>
        /// Returns a hash-code based on the current value of this object.
        /// </summary>
        public override int GetHashCode()
        {
            return
                CountCode.GetHashCode() ^
                Crc.GetHashCode() ^
                Offset.GetHashCode() ^
                Page.GetHashCode() ^
                ArrayExtensions.GetHashCode(Registers);
        }

        #endregion Operators

        #region Public Properties

        /// <summary>
        /// Count and code.
        /// </summary>
        public byte CountCode;

        /// <summary>
        /// Code from the <see cref="CountCode"/> (masked with <see cref="CodeMask"/>).
        /// </summary>
        public byte Code => (byte)(CountCode & CodeMask);

        /// <summary>
        /// Count from the <see cref="CountCode"/> (masked with <see cref="CountMask"/>).
        /// </summary>
        public byte Count => (byte)(CountCode & CountMask);

        /// <summary>
        /// CRC checksum.
        /// </summary>
        public byte Crc;

        /// <summary>
        /// Page number.
        /// </summary>
        public byte Page;

        /// <summary>
        /// Register offset.
        /// </summary>
        public byte Offset;

        /// <summary>
        /// Register values.
        /// </summary>
        [CLSCompliant(false)]
        public readonly ushort[] Registers;

        #endregion Public Properties

        #region Private Fields

        /// <summary>
        /// Table used to calculate <see cref="Crc"/>.
        /// </summary>
        private static readonly byte[] _crcTable = new byte[256]
        {
            0x00, 0x07, 0x0E, 0x09, 0x1C, 0x1B, 0x12, 0x15,
            0x38, 0x3F, 0x36, 0x31, 0x24, 0x23, 0x2A, 0x2D,
            0x70, 0x77, 0x7E, 0x79, 0x6C, 0x6B, 0x62, 0x65,
            0x48, 0x4F, 0x46, 0x41, 0x54, 0x53, 0x5A, 0x5D,
            0xE0, 0xE7, 0xEE, 0xE9, 0xFC, 0xFB, 0xF2, 0xF5,
            0xD8, 0xDF, 0xD6, 0xD1, 0xC4, 0xC3, 0xCA, 0xCD,
            0x90, 0x97, 0x9E, 0x99, 0x8C, 0x8B, 0x82, 0x85,
            0xA8, 0xAF, 0xA6, 0xA1, 0xB4, 0xB3, 0xBA, 0xBD,
            0xC7, 0xC0, 0xC9, 0xCE, 0xDB, 0xDC, 0xD5, 0xD2,
            0xFF, 0xF8, 0xF1, 0xF6, 0xE3, 0xE4, 0xED, 0xEA,
            0xB7, 0xB0, 0xB9, 0xBE, 0xAB, 0xAC, 0xA5, 0xA2,
            0x8F, 0x88, 0x81, 0x86, 0x93, 0x94, 0x9D, 0x9A,
            0x27, 0x20, 0x29, 0x2E, 0x3B, 0x3C, 0x35, 0x32,
            0x1F, 0x18, 0x11, 0x16, 0x03, 0x04, 0x0D, 0x0A,
            0x57, 0x50, 0x59, 0x5E, 0x4B, 0x4C, 0x45, 0x42,
            0x6F, 0x68, 0x61, 0x66, 0x73, 0x74, 0x7D, 0x7A,
            0x89, 0x8E, 0x87, 0x80, 0x95, 0x92, 0x9B, 0x9C,
            0xB1, 0xB6, 0xBF, 0xB8, 0xAD, 0xAA, 0xA3, 0xA4,
            0xF9, 0xFE, 0xF7, 0xF0, 0xE5, 0xE2, 0xEB, 0xEC,
            0xC1, 0xC6, 0xCF, 0xC8, 0xDD, 0xDA, 0xD3, 0xD4,
            0x69, 0x6E, 0x67, 0x60, 0x75, 0x72, 0x7B, 0x7C,
            0x51, 0x56, 0x5F, 0x58, 0x4D, 0x4A, 0x43, 0x44,
            0x19, 0x1E, 0x17, 0x10, 0x05, 0x02, 0x0B, 0x0C,
            0x21, 0x26, 0x2F, 0x28, 0x3D, 0x3A, 0x33, 0x34,
            0x4E, 0x49, 0x40, 0x47, 0x52, 0x55, 0x5C, 0x5B,
            0x76, 0x71, 0x78, 0x7F, 0x6A, 0x6D, 0x64, 0x63,
            0x3E, 0x39, 0x30, 0x37, 0x22, 0x25, 0x2C, 0x2B,
            0x06, 0x01, 0x08, 0x0F, 0x1A, 0x1D, 0x14, 0x13,
            0xAE, 0xA9, 0xA0, 0xA7, 0xB2, 0xB5, 0xBC, 0xBB,
            0x96, 0x91, 0x98, 0x9F, 0x8A, 0x8D, 0x84, 0x83,
            0xDE, 0xD9, 0xD0, 0xD7, 0xC2, 0xC5, 0xCC, 0xCB,
            0xE6, 0xE1, 0xE8, 0xEF, 0xFA, 0xFD, 0xF4, 0xF3
        };

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Calculates the CRC of a packet.
        /// </summary>
        public static byte CalculateCrc(Px4ioPacket packet)
        {
            // Validate
            if (packet == null) throw new ArgumentNullException(nameof(packet));

            // Calculate header CRC
            byte crc = 0;
            crc = _crcTable[crc ^ packet.CountCode];
            crc = _crcTable[crc ^ 0];                   // Cannot CRC self
            crc = _crcTable[crc ^ packet.Page];
            crc = _crcTable[crc ^ packet.Offset];

            // Add register CRCs
            for (var index = 0; index < packet.Count; index++)
            {
                var value = packet.Registers[index];
                var bytes = BitConverter.GetBytes(value);
                crc = _crcTable[crc ^ bytes[0]];
                crc = _crcTable[crc ^ bytes[1]];
            }

            // Return result
            return crc;
        }

        /// <summary>
        /// Calculates and updates the <see cref="Crc"/> of the current packet.
        /// </summary>
        public void CalculateCrc()
        {
            Crc = CalculateCrc(this);
        }

        /// <summary>
        /// Recalculates then validates the current <see cref="Crc"/>.
        /// </summary>
        public bool ValidateCrc()
        {
            return Crc == CalculateCrc(this);
        }

        /// <summary>
        /// Creates an instance from raw data bytes.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static Px4ioPacket FromByteArray(byte[] buffer)
        {
            return new Px4ioPacket(buffer);
        }

        /// <summary>
        /// Converts this value to raw data bytes.
        /// </summary>
        public byte[] ToByteArray()
        {
            // Allocate buffer
            var buffer = new byte[Size];
            var offset = 0;

            // Copy fixed values
            buffer[offset++] = CountCode;
            buffer[offset++] = Crc;
            buffer[offset++] = Page;
            buffer[offset++] = Offset;

            // Copy registers
            for (var index = 0; index < Count; index++)
            {
                var valueBytes = BitConverter.GetBytes(Registers[index]);
                buffer[offset++] = valueBytes[0];
                buffer[offset++] = valueBytes[1];
            }

            // Return result
            return buffer;
        }

        #endregion Public Methods
    }
}