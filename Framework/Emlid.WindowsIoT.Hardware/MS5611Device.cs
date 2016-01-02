using Emlid.WindowsIot.Common;
using System;
using System.Threading.Tasks;
using Windows.Devices.I2c;

namespace Emlid.WindowsIot.Hardware
{
    /// <summary>
    /// MS5611 barometric pressure and temperature sensor (hardware device), connected via I2C.
    /// </summary>
    public class Ms5611Device : I2cConnectedDevice
    {
        #region Constants

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

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates an instance at the specified I2C <paramref name="address"/> with custom settings.
        /// </summary>
        /// <param name="address">
        /// I2C slave address of the chip.
        /// This is a physical property, not a software option.
        /// </param>
        /// <param name="fast">
        /// Set true for I2C <see cref="I2cBusSpeed.FastMode"/> or false for <see cref="I2cBusSpeed.StandardMode"/>.
        /// </param>
        /// <param name="exclusive">
        /// Set true for I2C <see cref="I2cSharingMode.Exclusive"/> or false for <see cref="I2cSharingMode.Shared"/>.
        /// </param>
        /// <param name="rate">Sampling rate.</param>
        public Ms5611Device(int address, bool fast, bool exclusive, Ms5611Osr rate)
            : base(address, fast, exclusive)
        {
            // Initialize members
            Prom = new Ms5611PromData();
            Osr = rate;

            // Read current calibration data (potentially not stable until next reset)
            // We don't reset automatically so that it is possible for any ongoing tasks to complete
            ReadProm();
        }

        #endregion

        #region Properties

        /// <summary>
        /// PROM data, containing the pressure and temperature calculation coefficients and other information.
        /// </summary>
        /// <remarks>
        /// Populated during <see cref="Reset"/>.
        /// </remarks>
        public Ms5611PromData Prom { get; protected set; }

        /// <summary>
        /// Over-Sampling Rate to use for measurement.
        /// </summary>
        /// <remarks>
        /// Default is set during construction, but can be overridden.
        /// Then call <see cref="Update"/> to re-calculate.
        /// </remarks>
        public Ms5611Osr Osr { get; set; }

        /// <summary>
        /// Result of the last <see cref="Update"/>.
        /// </summary>
        public Ms5611Measurement Measurement { get; protected set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Resets the device, updates PROM data and clears current measurements
        /// </summary>
        public virtual void Reset()
        {
            // Send reset command
            Hardware.WriteJoinByte((byte)Ms5611Command.Reset, 0);

            // Wait for completion
            Task.Delay(TimeSpanExtensions.FromMicroseconds(ResetTime)).Wait();

            // Update PROM values
            ReadProm();

            // Clear measurement
            Measurement = Ms5611Measurement.Zero;
        }

        /// <summary>
        /// Converts then calculates the <see cref="Measurement"/>.
        /// </summary>
        public virtual void Update()
        {
            // Measure with current OSR
            var rawPressure = ConvertPressure(Osr);
            var rawTemperature = ConvertTemperature(Osr);

            // Calculate and update properties
            Calculate(rawPressure, rawTemperature);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Reads calibration data from PROM, validates and updates the PROM data.
        /// </summary>
        /// <returns>
        /// True when data was read and the CRC check passed.
        /// </returns>
        /// <remarks>
        /// Should only be executed after <see cref="Reset"/> to get accurate values.
        /// </remarks>
        protected virtual bool ReadProm()
        {
            // Read from hardware
            var buffer = new byte[Ms5611PromData.MemorySize];
            for (var coefficient = 0; coefficient < Ms5611PromData.CoefficientCount; coefficient++)
                ReadPromCoefficient(coefficient, buffer, Ms5611PromData.CoefficientSize * coefficient);

            // Validate and update properties
            return Prom.Read(buffer);
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
        protected virtual void ReadPromCoefficient(int index, byte[] buffer, int offset)
        {
            var coefficientOffset = (byte)(index * Ms5611PromData.CoefficientSize);
            var address = (byte)(Ms5611Command.PromRead + coefficientOffset);
            Hardware.WriteReadBytes(address, Ms5611PromData.CoefficientSize, buffer, offset);
        }

        /// <summary>
        /// Executes the <see cref="Ms5611Command.ConvertD1Pressure"/> command to measure
        /// pressure at the specified OSR, waits then returns the result.
        /// </summary>
        protected virtual int ConvertPressure(Ms5611Osr rate)
        {
            // Send command to hardware
            Hardware.WriteJoinByte((byte)(Ms5611Command.ConvertD1Pressure + (byte)rate), 0);

            // Wait for completion
            WaitForConversion(rate);

            // Return result
            var result = Hardware.WriteReadBytes((byte)Ms5611Command.AdcRead, 3);
            return result[0] << 16 | result[1] << 8 | result[2];
        }

        /// <summary>
        /// Executes the <see cref="Ms5611Command.ConvertD2Temperature"/> command to measure
        /// pressure at the specified OSR, waits then returns the result.
        /// </summary>
        protected virtual int ConvertTemperature(Ms5611Osr rate)
        {
            // Send command to hardware
            Hardware.WriteJoinByte((byte)(Ms5611Command.ConvertD2Temperature + (byte)rate), 0);

            // Wait for completion
            WaitForConversion(rate);

            // Return result
            var result = Hardware.WriteReadBytes((byte)Ms5611Command.AdcRead, 3);
            return result[0] << 16 | result[1] << 8 | result[2];
        }

        /// <summary>
        /// Waits for conversion at the specified OSR.
        /// </summary>
        /// <param name="rate">Over-Sampling Rate to wait for.</param>
        protected virtual void WaitForConversion(Ms5611Osr rate)
        {
            var delay = TimeSpanExtensions.FromMicroseconds(GetConvertDelay(rate));
            Task.Delay(delay).Wait();
        }

        /// <summary>
        /// Gets the time to wait in milliseconds for conversion with the specified OSR.
        /// </summary>
        /// <param name="rate">Over-Sampling Rate for which to return the delay.</param>
        /// <returns>Delay in milliseconds.</returns>
        protected virtual int GetConvertDelay(Ms5611Osr rate)
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
        /// Calculates the <see cref="Measurement"/> by applying the <see cref="Prom"/>
        /// coefficients and other rules to the raw measurements.
        /// </summary>
        protected virtual void Calculate(int rawPressure, int rawTemperature)
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

            // Correct floating point to whole values and save in properties
            Measurement = new Ms5611Measurement(pressure / 100, temperature / 100);
        }

        #endregion
    }
}
