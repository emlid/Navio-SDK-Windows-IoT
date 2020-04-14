using CodeForDevices.WindowsUniversal.Hardware.Buses;
using CodeForDotNet;
using System;
using System.Threading.Tasks;
using Windows.Devices.I2c;

namespace Emlid.WindowsIot.Hardware.Components.Ms5611
{
    /// <summary>
    /// MS5611 barometric pressure and temperature sensor (hardware device), connected via I2C.
    /// </summary>
    /// <remarks>
    /// <see href="http://www.te.com/commerce/DocumentDelivery/DDEController?Action=showdoc&amp;DocId=Data+Sheet%7FMS5611-01BA03%7FB%7Fpdf%7FEnglish%7FENG_DS_MS5611-01BA03_B.pdf%7FCAT-BLPS0036">Data sheet.</see>
    /// </remarks>
    public sealed class Ms5611Device : DisposableObject
    {
        // TODO: Support same chip connected via SPI (it supports both)

        #region Constants

        /// <summary>
        /// 7-bit I2C address of the first chip on the I2C bus.
        /// </summary>
        /// <remarks>
        /// When using multiple chips the Chip Select Bit (CSB) must be added for the second chip.
        /// </remarks>
        public const byte I2cAddress = 0xec >> 1;

        /// <summary>
        /// Time to wait for the <see cref="Reset"/> command to complete in microseconds.
        /// </summary>
        public const int ResetTime = 2800;

        /// <summary>
        /// Time to wait for conversion with OSR 256 in microseconds.
        /// </summary>
        public const int ConvertOsr256Time = 600;

        /// <summary>
        /// Time to wait for conversion with OSR 512 in microseconds.
        /// </summary>
        public const int ConvertOsr512Time = 1170;

        /// <summary>
        /// Time to wait for conversion with OSR 1024 in microseconds.
        /// </summary>
        public const int ConvertOsr1024Time = 2280;

        /// <summary>
        /// Time to wait for conversion with OSR 2048 in microseconds.
        /// </summary>
        public const int ConvertOsr2048Time = 4540;

        /// <summary>
        /// Time to wait for conversion with OSR 4096 in microseconds.
        /// </summary>
        public const int ConvertOsr4096Time = 9040;

        /// <summary>
        /// Minimum pressure measurement in millibars.
        /// </summary>
        public const double PressureMin = 10;

        /// <summary>
        /// Maximum pressure measurement in millibars.
        /// </summary>
        public const double PressureMax = 1200;

        /// <summary>
        /// Minimum pressure measurement in millibars.
        /// </summary>
        public const double TemperatureMin = -40;

        /// <summary>
        /// Maximum pressure measurement in millibars.
        /// </summary>
        public const double TemperatureMax = 85;

        /// <summary>
        /// Accuracy of the temperature and pressure measurements.
        /// </summary>
        public const double Accuracy = 0.01;

        #endregion Constants

        #region Lifetime

        /// <summary>
        /// Creates an instance using the specified device and sampling rate.
        /// </summary>
        /// <param name="busNumber">I2C bus controller number (zero based).</param>
        /// <param name="csb">Chip Select Bit (CSB).</param>
        /// <param name="rate">Sampling rate.</param>
        /// <param name="speed">Bus speed.</param>
        /// <param name="sharingMode">Sharing mode.</param>
        [CLSCompliant(false)]
        public Ms5611Device(int busNumber, bool csb, Ms5611Osr rate,
            I2cBusSpeed speed = I2cBusSpeed.FastMode, I2cSharingMode sharingMode = I2cSharingMode.Exclusive)
        {
            // Get address
            ChipSelectBit = csb;
            Address = GetI2cAddress(csb);

            // Connect to hardware
            _hardware = I2cExtensions.Connect(busNumber, Address, speed, sharingMode);

            // Initialize members
            Prom = new Ms5611PromData();
            Osr = rate;

            // Read current calibration data (potentially not stable until next reset)
            // We don't reset automatically so that it is possible for any ongoing tasks to complete
            ReadProm();
        }

        /// <summary>
        /// <see cref="DisposableObject.Dispose(bool)"/>.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            // Only managed resources to dispose
            if (!disposing)
                return;

            // Close device
            _hardware?.Dispose();
        }

        #endregion Lifetime

        #region Private Fields

        /// <summary>
        /// I2C device.
        /// </summary>
        private I2cDevice _hardware;

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// I2C address of the current chip.
        /// </summary>
        public int Address { get; private set; }

        /// <summary>
        /// Chip Select Bit (CSB).
        /// </summary>
        /// <remarks>
        /// False when connected to the first/only chip, true when connected to an optional second chip.
        /// </remarks>
        public bool ChipSelectBit { get; private set; }

        /// <summary>
        /// PROM data, containing the pressure and temperature calculation coefficients and other information.
        /// </summary>
        /// <remarks>
        /// Populated during <see cref="Reset"/>.
        /// </remarks>
        public Ms5611PromData Prom { get; private set; }

        /// <summary>
        /// Over-Sampling Rate to use for measurement.
        /// </summary>
        /// <remarks>
        /// Default is set during construction, but can be overridden.
        /// Then call <see cref="Update"/> to re-calculate.
        /// </remarks>
        public Ms5611Osr Osr { get; set; }

        /// <summary>
        /// Last measured pressure in millibars.
        /// </summary>
        public double Pressure { get; private set; }

        /// <summary>
        /// Last measured temperature in celsius.
        /// </summary>
        /// <remarks>
        /// The temperature will be higher than outside because
        /// it is heated by other components.
        /// </remarks>
        public double Temperature { get; private set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Gets the I2C address for the
        /// </summary>
        /// <param name="csb">Chip Select Bit (CSB).</param>
        /// <returns>7-bit I2C address.</returns>
        public static byte GetI2cAddress(bool csb)
        {
            return csb ? (byte)(I2cAddress + 1) : I2cAddress;
        }

        /// <summary>
        /// Resets the device, updates PROM data and clears current measurements.
        /// </summary>
        public void Reset()
        {
            // Send reset command
            I2cExtensions.WriteJoinByte(_hardware, (byte)Ms5611Command.Reset, 0);

            // Wait for completion
            Task.Delay(TimeSpanExtensions.FromMicroseconds(ResetTime)).Wait();

            // Update PROM values
            ReadProm();

            // Clear measurements
            Pressure = 0;
            Temperature = 0;
        }

        /// <summary>
        /// Converts then calculates the <see cref="Pressure"/> and <see cref="Temperature"/>.
        /// </summary>
        public void Update()
        {
            // Measure with current OSR
            var rawPressure = ConvertPressure(Osr);
            var rawTemperature = ConvertTemperature(Osr);

            // Calculate and update properties
            Calculate(rawPressure, rawTemperature);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Reads calibration data from PROM, validates and updates the PROM data.
        /// </summary>
        /// <returns>
        /// True when data was read and the CRC check passed.
        /// </returns>
        /// <remarks>
        /// Should only be executed after <see cref="Reset"/> to get accurate values.
        /// </remarks>
        public bool ReadProm()
        {
            // Read from hardware
            var buffer = new byte[Ms5611PromData.MemorySize];
            for (var coefficient = 0; coefficient < Ms5611PromData.CoefficientCount; coefficient++)
                ReadPromCoefficient(coefficient, buffer, Ms5611PromData.CoefficientSize * coefficient);

            // Validate and update properties
            return Prom.Update(buffer);
        }

        /// <summary>
        /// Reads a coefficient value from the PROM.
        /// </summary>
        /// <param name="index">Coefficient index (0-7).</param>
        /// <param name="buffer">Target buffer.</param>
        /// <param name="offset">Target offset.</param>
        /// <remarks>
        /// Reads <see cref="Ms5611PromData.CoefficientSize"/> bytes into the target buffer at the specified offset.
        /// </remarks>
        private void ReadPromCoefficient(int index, byte[] buffer, int offset)
        {
            // Calculate address
            var coefficientOffset = (byte)(index * Ms5611PromData.CoefficientSize);
            var address = (byte)(Ms5611Command.PromRead + coefficientOffset);

            // Read from hardware return value
            var coefficient = I2cExtensions.WriteReadBytes(_hardware, address, Ms5611PromData.CoefficientSize);
            Array.ConstrainedCopy(coefficient, 0, buffer, offset, Ms5611PromData.CoefficientSize);
        }

        /// <summary>
        /// Executes the <see cref="Ms5611Command.ConvertD1Pressure"/> command to measure
        /// pressure at the specified OSR, waits then returns the result.
        /// </summary>
        public int ConvertPressure(Ms5611Osr rate)
        {
            // Send command to hardware
            I2cExtensions.WriteJoinByte(_hardware, (byte)(Ms5611Command.ConvertD1Pressure + (byte)rate), 0);

            // Wait for completion
            WaitForConversion(rate);

            // Return result
            var result = I2cExtensions.WriteReadBytes(_hardware, (byte)Ms5611Command.AdcRead, 3);
            return result[0] << 16 | result[1] << 8 | result[2];
        }

        /// <summary>
        /// Executes the <see cref="Ms5611Command.ConvertD2Temperature"/> command to measure
        /// pressure at the specified OSR, waits then returns the result.
        /// </summary>
        public int ConvertTemperature(Ms5611Osr rate)
        {
            // Send command to hardware
            I2cExtensions.WriteJoinByte(_hardware, (byte)(Ms5611Command.ConvertD2Temperature + (byte)rate), 0);

            // Wait for completion
            WaitForConversion(rate);

            // Return result
            var result = I2cExtensions.WriteReadBytes(_hardware, (byte)Ms5611Command.AdcRead, 3);
            return result[0] << 16 | result[1] << 8 | result[2];
        }

        /// <summary>
        /// Waits for conversion at the specified OSR.
        /// </summary>
        /// <param name="rate">Over-Sampling Rate to wait for.</param>
        private static void WaitForConversion(Ms5611Osr rate)
        {
            var delay = TimeSpanExtensions.FromMicroseconds(GetConvertDelay(rate));
            Task.Delay(delay).Wait();
        }

        /// <summary>
        /// Gets the time to wait in milliseconds for conversion with the specified OSR.
        /// </summary>
        /// <param name="rate">Over-Sampling Rate for which to return the delay.</param>
        /// <returns>Delay in milliseconds.</returns>
        public static int GetConvertDelay(Ms5611Osr rate)
        {
            switch (rate)
            {
                case Ms5611Osr.Osr256:
                    return ConvertOsr256Time;

                case Ms5611Osr.Osr512:
                    return ConvertOsr512Time;

                case Ms5611Osr.Osr1024:
                    return ConvertOsr1024Time;

                case Ms5611Osr.Osr2048:
                    return ConvertOsr2048Time;

                case Ms5611Osr.Osr4096:
                    return ConvertOsr4096Time;

                default:    // Future proof
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Calculates the <see cref="Pressure"/> and <see cref="Temperature"/> by applying the
        /// <see cref="Prom"/> coefficients and other rules to the raw measurements.
        /// </summary>
        private void Calculate(int rawPressure, int rawTemperature)
        {
            // Constants for calculation
            const double temperatureAt20C = 2000;
            const double temperatureAt15C = 1500;
            const double temperatureAtMinus15C = -1500;
            const double scale2Power7 = 128;
            const double scale2Power8 = 256;
            const double scale2Power15 = 32768;
            const double scale2Power16 = 65536;
            const double scale2Power21 = 2097152;
            const double scale2Power23 = 8388608;
            const double scale2Power31 = 2147483648;

            // First order pressure and temperature calculation...

            // Calculate reference temperature
            var referenceTemperature = Prom.C5TemperatureReference * scale2Power8;

            // Difference between actual and reference temperature
            var deltaTemperature = rawTemperature - referenceTemperature;

            // Actual temperature (-40...85°C with 0.01°C resolution)
            var temperature = temperatureAt20C + deltaTemperature *
                Prom.C6TemperatureSensitivity / scale2Power23;

            // Calculate temperature compensated pressure

            // Offset at actual temperature
            var offset = Prom.C2PressureOffset * scale2Power16 +
                (Prom.C4TemperatureFromPressureOffset * deltaTemperature) / scale2Power7;

            // Sensitivity at actual temperature
            var sensitivity = Prom.C1PressureSensitivity * scale2Power15 +
                (Prom.C3TemperatureFromPressureSensitivity * deltaTemperature) / scale2Power8;

            // Second order temperature compensation...
            if (temperature < temperatureAt20C)
            {
                // Low temperature
                var temperature2 = Math.Pow(deltaTemperature, 2) / scale2Power31;
                var offset2 = 5 * Math.Pow(temperature - temperatureAt20C, 2) / 2;
                var sensitivity2 = offset2 / 2;
                if (temperature < temperatureAtMinus15C)
                {
                    // Very low temperature
                    var temperaturePlus15CSquared = Math.Pow(temperature + temperatureAt15C, 2);
                    offset2 = offset2 + 7 * temperaturePlus15CSquared;
                    sensitivity2 = sensitivity2 + 11 * temperaturePlus15CSquared / 2;
                }
                temperature -= temperature2;
                offset -= offset2;
                sensitivity -= sensitivity2;
            }

            // Temperature compensated pressure (10...1200mbar with 0.01mbar resolution)
            var pressure = (rawPressure * sensitivity / scale2Power21 - offset) / scale2Power15;

            // Scale values and update properties
            Pressure = pressure / 100;
            Temperature = temperature / 100;
        }

        #endregion Private Methods
    }
}
