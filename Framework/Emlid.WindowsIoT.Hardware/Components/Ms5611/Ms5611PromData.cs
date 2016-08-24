using System;

namespace Emlid.WindowsIot.Hardware.Components.Ms5611
{
    /// <summary>
    /// Defines the PROM of the <see cref="Ms5611Device"/>.
    /// </summary>
    public class Ms5611PromData
    {
        #region Constants

        /// <summary>
        /// Size of a PROM coefficient in bytes.
        /// </summary>
        public const int CoefficientSize = 2;

        /// <summary>
        /// Number of coefficients in the PROM.
        /// </summary>
        public const int CoefficientCount = 8;

        /// <summary>
        /// PROM memory size in bytes (must be read as individual variable requests).
        /// </summary>
        public const int MemorySize = CoefficientSize * CoefficientCount;

        /// <summary>
        /// PROM read coefficient 0 (manufacturer data) offset.
        /// </summary>
        /// <remarks>
        /// 2 bytes (16 bit) unsigned data.
        /// </remarks>
        public const int C0ManufacturerOffset = 0x00;

        /// <summary>
        /// PROM read coefficient 1 (pressure sensitivity / SENS) offset.
        /// </summary>
        /// <remarks>
        /// 2 bytes (16 bit) unsigned data.
        /// </remarks>
        public const int C1SensOffset = 0x02;

        /// <summary>
        /// PROM read coefficient 2 (pressure offset / OFF) offset.
        /// </summary>
        /// <remarks>
        /// 2 bytes (16 bit) unsigned data.
        /// </remarks>
        public const int C2OffOffset = 0x04;

        /// <summary>
        /// PROM read coefficient 3 (temperature coefficient of pressure sensitivity / TCS) offset.
        /// </summary>
        /// <remarks>
        /// 2 bytes (16 bit) unsigned data.
        /// </remarks>
        public const int C3TcsOffset = 0x06;

        /// <summary>
        /// PROM read coefficient 4 (temperature coefficient of pressure offset / TCO) offset.
        /// </summary>
        /// <remarks>
        /// 2 bytes (16 bit) unsigned data.
        /// </remarks>
        public const int C4TcoOffset = 0x08;

        /// <summary>
        /// PROM read coefficient 5 (reference temperature / TREF) offset.
        /// </summary>
        /// <remarks>
        /// 2 bytes (16 bit) unsigned data.
        /// </remarks>
        public const int C5TRefOffset = 0x0a;

        /// <summary>
        /// PROM read coefficient 6 (temperature coefficient of the temperature / TEMPSENS) offset.
        /// </summary>
        /// <remarks>
        /// 2 bytes (16 bit) unsigned data.
        /// </remarks>
        public const int C6TempSensOffset = 0x0c;

        /// <summary>
        /// PROM coefficient 7 (serial and CRC) offset.
        /// </summary>
        /// <remarks>
        /// 2 bytes (16 bit) unsigned data.
        /// Lowest 4 bits are CRC.
        /// Highest 12 bits are serial number.
        /// </remarks>
        public const int C7SerialCrcOffset = 0x0e;

        #endregion

        #region Properties

        /// <summary>
        /// Manufacturer reserved data, e.g. company or device ID.
        /// </summary>
        /// <remarks>
        /// Stored in memory at the <see cref="C0ManufacturerOffset"/>.
        /// </remarks>
        public int C0Manufacturer { get; private set; }

        /// <summary>
        /// Pressure sensitivity (SENS).
        /// </summary>
        /// <remarks>
        /// Stored in memory at the <see cref="C1SensOffset"/>.
        /// </remarks>
        public int C1PressureSensitivity { get; private set; }

        /// <summary>
        /// Pressure offset (OFF).
        /// </summary>
        /// <remarks>
        /// Stored in memory at the <see cref="C2OffOffset"/>.
        /// </remarks>
        public int C2PressureOffset { get; private set; }

        /// <summary>
        /// Temperature coefficient of pressure sensitivity (TCS).
        /// </summary>
        /// <remarks>
        /// Stored in memory at the <see cref="C3TcsOffset"/>.
        /// </remarks>
        public int C3TemperatureFromPressureSensitivity { get; private set; }

        /// <summary>
        /// Temperature coefficient of pressure offset (TCO).
        /// </summary>
        /// <remarks>
        /// Stored in memory at the <see cref="C4TcoOffset"/>.
        /// </remarks>
        public int C4TemperatureFromPressureOffset { get; private set; }

        /// <summary>
        /// Reference temperature (TREF).
        /// </summary>
        /// <remarks>
        /// Stored in memory at the <see cref="C5TRefOffset"/>.
        /// </remarks>
        public int C5TemperatureReference { get; private set; }

        /// <summary>
        /// Temperature sensitivity (TEMPSENS).
        /// </summary>
        /// <remarks>
        /// Stored in memory at the <see cref="C6TempSensOffset"/>.
        /// </remarks>
        public int C6TemperatureSensitivity { get; private set; }

        /// <summary>
        /// Serial number.
        /// </summary>
        /// <remarks>
        /// Stored in memory at <see cref="C7SerialCrcOffset"/> in the upper 12 bits.
        /// </remarks>
        public int C7SerialNumber { get; private set; }

        /// <summary>
        /// CRC check code which validated the data.
        /// </summary>
        /// <remarks>
        /// Stored in memory at <see cref="C7SerialCrcOffset"/> in the lower 4 bits.
        /// </remarks>
        public int C7Crc { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Validates a PROM data set (bytes read from all coefficients/memory).
        /// </summary>
        /// <param name="buffer">PROM data buffer to validate.</param>
        /// <returns>
        /// True when the CRC check passed, false when failed.
        /// </returns>
        public static bool Validate(byte[] buffer)
        {
            // Get hardware calculated 4 bit CRC from buffer
            var crc = buffer[C7SerialCrcOffset + 1] & 0x0f;

            // Re-calculate CRC based on data in buffer
            var remainder = 0;
            for (var bufferIndex = 0; bufferIndex < MemorySize; bufferIndex++)
            {
                // Get data byte in 16 bit form
                var data = (int)buffer[bufferIndex];

                // Mask byte containing CRC (from the CRC check itself)
                // Note the whole byte is masked not just the 4 CRC bits,
                // i.e. 4 bits of serial are excluded too
                if (bufferIndex == C7SerialCrcOffset + 1)
                    data &= 0xff00;

                // XOR data
                remainder ^= data;

                // XOR bits
                for (var bits = 8; bits > 0; bits--)
                {
                    if ((remainder & 0x8000) != 0)
                        remainder = (remainder << 1) ^ 0x3000;
                    else
                        remainder = remainder << 1;
                }
            }

            // Final shift down to 4 bits produces CRC result
            var crcCheck = 0x000f & (remainder >> 12);      

            // Return successful when CRC matches hardware
            return crc == crcCheck;
        }

        /// <summary>
        /// Extracts a single coefficient value from a PROM data set (bytes read from all coefficients/memory).
        /// </summary>
        /// <param name="buffer">PROM data buffer to read.</param>
        /// <param name="offset">Coefficient offset at which to read.</param>
        [CLSCompliant(false)]
        public static ushort ReadCoefficient(byte[] buffer, int offset)
        {
            return (ushort)(buffer[offset] << 8 | buffer[offset + 1]);
        }

        /// <summary>
        /// Validates then reads properties from a PROM data set (bytes read from all coefficients/memory). 
        /// </summary>
        /// <param name="buffer">PROM data buffer to read.</param>
        /// <returns>
        /// True when the CRC check passed, false when failed.
        /// </returns>
        public bool Read(byte[] buffer)
        {
            // Validate
            if (!Validate(buffer)) return false;

            // Extract properties from data
            C0Manufacturer = ReadCoefficient(buffer, C0ManufacturerOffset);
            C1PressureSensitivity = ReadCoefficient(buffer, C1SensOffset);
            C2PressureOffset = ReadCoefficient(buffer, C2OffOffset);
            C3TemperatureFromPressureSensitivity = ReadCoefficient(buffer, C3TcsOffset);
            C4TemperatureFromPressureOffset = ReadCoefficient(buffer, C4TcoOffset);
            C5TemperatureReference = ReadCoefficient(buffer, C5TRefOffset);
            C6TemperatureSensitivity = ReadCoefficient(buffer, C6TempSensOffset);
            var serialCrc = ReadCoefficient(buffer, C7SerialCrcOffset);
            C7SerialNumber = serialCrc >> 4;
            C7Crc = serialCrc & 0x000f;

            // Return successful
            return true;
        }

        #endregion
    }
}
