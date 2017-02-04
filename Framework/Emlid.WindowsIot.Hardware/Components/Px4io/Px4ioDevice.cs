using Emlid.UniversalWindows;
using Emlid.WindowsIot.Hardware.Components.Px4io.Data;
using Emlid.WindowsIot.HardwarePlus.Buses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using Windows.Devices.Spi;

namespace Emlid.WindowsIot.Hardware.Components.Px4io
{
    /// <summary>
    /// PX4IO UAV I/O board (hardware device), connected via SPI.
    /// </summary>
    /// <see href="https://github.com/emlid/navio-rcio-linux-driver/blob/master/protocol.h"/>
    /// <see href="https://github.com/ArduPilot/PX4Firmware/tree/master/src/drivers/px4io"/>
    [CLSCompliant(false)]
    public sealed class Px4ioDevice : DisposableObject
    {
        #region Constants

        /// <summary>
        /// Delay used to pace transfers and allow the co-processor time to respond.
        /// </summary>
        public static readonly TimeSpan TransferPacingDelay = TimeSpanExtensions.FromMicroseconds(150);

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates an instance using the specified I2C device.
        /// </summary>
        /// <param name="busNumber">Bus controller number, zero based.</param>
        /// <param name="chipSelectLine">Slave Chip Select Line.</param>
        /// <param name="mode">Communication mode, i.e. clock polarity.</param>
        /// <param name="dataBitLength">Data length in bits.</param>
        /// <param name="clockFrequency">Frequency in Hz.</param>
        /// <param name="sharingMode">Sharing mode.</param>
        public Px4ioDevice(int busNumber, int chipSelectLine, SpiMode mode, int dataBitLength, int clockFrequency, SpiSharingMode sharingMode)
        {
            // Connect to hardware
            _hardware = SpiExtensions.Connect(busNumber, chipSelectLine, mode, dataBitLength, clockFrequency, sharingMode);
            if (_hardware == null)
            {
                // Initialization error
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    Resources.Strings.SpiErrorDeviceNotFound, chipSelectLine, busNumber));
            }

            // Initialize configuration
            Read();
        }

        #region IDisposable

        /// <summary>
        /// Frees resources owned by this instance.
        /// </summary>
        /// <param name="disposing">
        /// True when called via <see cref="IDisposable.Dispose"/>, false when called via finalizer.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            // Only managed resources to dispose
            if (!disposing)
                return;

            // Close device
            _hardware?.Dispose();
        }

        #endregion

        #endregion

        #region Private Fields

        /// <summary>
        /// SPI device.
        /// </summary>
        private SpiDevice _hardware;

        #endregion

        #region Public Properties

        #region General

        /// <summary>
        /// <see cref="Px4ioPage.Config"/> registers.
        /// </summary>
        public Px4ioConfigRegisters Configuration { get; private set; }

        /// <summary>
        /// <see cref="Px4ioPage.Setup"/> registers.
        /// </summary>
        public Px4ioSetupRegisters Setup { get; private set; }

        /// <summary>
        /// <see cref="Px4ioPage.Status"/> registers.
        /// </summary>
        public Px4ioStatusRegisters Status { get; private set; }

        /// <summary>
        /// <see cref="Px4ioPage.Test"/> registers.
        /// </summary>
        public Px4ioTestRegisters Test { get; private set; }

        #endregion

        #region RC Input

        /// <summary>
        /// <see cref="Px4ioPage.RCConfig"/> registers.
        /// </summary>
        public Px4ioRCConfigRegisters RCConfig { get; private set; }

        /// <summary>
        /// <see cref="Px4ioPage.RCInputRaw"/> registers.
        /// </summary>
        public Px4ioRCInputRawRegisters RCInputRaw { get; private set; }

        /// <summary>
        /// <see cref="Px4ioPage.RCInput"/> registers.
        /// </summary>
        public Px4ioRCInputRegisters RCInput { get; private set; }

        #endregion

        #region RC Output

        /// <summary>
        /// <see cref="Px4ioPage.Controls"/> registers.
        /// </summary>
        public Px4ioControlRegisters Controls { get; private set; }

        /// <summary>
        /// <see cref="Px4ioPage.MixerLoad"/> registers.
        /// </summary>
        public ReadOnlyCollection<ushort> MixerLoad { get; private set; }
        //private ushort[] _mixerLoad;

        #endregion

        #region ADC

        /// <summary>
        /// <see cref="Px4ioPage.AdcInputRaw"/> registers.
        /// </summary>
        public ReadOnlyCollection<ushort> AdcInputRaw { get; private set; }
        private ushort[] _adcInputRaw;

        #endregion

        #region PWM

        /// <summary>
        /// <see cref="Px4ioPage.Pwm"/> registers.
        /// </summary>
        public ReadOnlyCollection<ushort> Pwm { get; private set; }
        private ushort[] _pwm;

        /// <summary>
        /// <see cref="Px4ioPage.PwmDirect"/> registers.
        /// </summary>
        public ReadOnlyCollection<ushort> PwmDirect { get; private set; }
        private ushort[] _pwmDirect;

        /// <summary>
        /// <see cref="Px4ioPage.PwmFailsafe"/> registers.
        /// </summary>
        public ReadOnlyCollection<ushort> PwmFailsafe { get; private set; }
        private ushort[] _pwmFailsafe;

        /// <summary>
        /// <see cref="Px4ioPage.PwmMinimum"/> registers.
        /// </summary>
        public ReadOnlyCollection<ushort> PwmMinimum { get; private set; }
        private ushort[] _pwmMinimum;

        /// <summary>
        /// <see cref="Px4ioPage.PwmMaximum"/> registers.
        /// </summary>
        public ReadOnlyCollection<ushort> PwmMaximum { get; private set; }
        private ushort[] _pwmMaximum;

        /// <summary>
        /// <see cref="Px4ioPage.PwmDisarmed"/> registers.
        /// </summary>
        public ReadOnlyCollection<ushort> PwmDisarmed { get; private set; }
        private ushort[] _pwmDisarmed;

        #endregion

        #region Actuators

        /// <summary>
        /// <see cref="Px4ioPage.Actuators"/> registers.
        /// </summary>
        public ReadOnlyCollection<ushort> Actuators { get; private set; }
        private ushort[] _actuators;

        /// <summary>
        /// <see cref="Px4ioPage.Servos"/> registers.
        /// </summary>
        public ReadOnlyCollection<ushort> Servos { get; private set; }
        private ushort[] _servos;

        #endregion

        #region Sensors

        /// <summary>
        /// <see cref="Px4ioPage.Sensors"/> registers.
        /// </summary>
        public Px4ioSensorRegisters Sensors { get; private set; }

        #endregion

        #endregion

        #region Public Methods

        /// <summary>
        /// Reads all registers.
        /// </summary>
        public void Read()
        {
            // General
            Configuration = new Px4ioConfigRegisters(ReadRegisters((byte)Px4ioPage.Config,
                0, Px4ioConfigRegisters.RegisterCount));
            Status = new Px4ioStatusRegisters(ReadRegisters((byte)Px4ioPage.Status,
                0, Px4ioStatusRegisters.RegisterCount));
            Setup = new Px4ioSetupRegisters(ReadRegisters((byte)Px4ioPage.Setup,
                0, Px4ioSetupRegisters.RegisterCount));
            // TODO: Investigate RCIO test registers. Doesn't work on Navio 2 original firmware.
            //Test = new Px4ioTestRegisters(ReadRegisters((byte)Px4ioPage.Test,
            //    0, Px4ioTestRegisters.RegisterCount));

            // RC Input
            RCConfig = new Px4ioRCConfigRegisters(ReadRegisters((byte)Px4ioPage.RCConfig,
                0, Px4ioRCConfigRegisters.RegisterCount));
            RCInputRaw = new Px4ioRCInputRawRegisters(ReadRegisters((byte)Px4ioPage.RCInputRaw,
                0, (byte)(Px4ioRCInputRawRegisters.RegisterCount + Configuration.RCInputCount - 1)));
            RCInput = new Px4ioRCInputRegisters(ReadRegisters((byte)Px4ioPage.RCInput,
                0, (byte)(Px4ioRCInputRegisters.RegisterCount + Configuration.RCInputCount - 1),
                requireCount: false));

            // RC Output
            Controls = new Px4ioControlRegisters(ReadRegisters((byte)Px4ioPage.Controls,
                0, Px4ioControlRegisters.RegisterCount));
            // TODO: Investigate mixer load, is it readable at all? Doesn't work on Navio 2 original firmware.
            //var maximumRegisters = (ushort)((Configuration.TransferMaximum - Px4ioPacket.HeaderSize) / sizeof(ushort));
            //_mixerLoad = ReadRegisters((byte)Px4ioPage.MixerLoad, 0, maximumRegisters);
            //MixerLoad = new ReadOnlyCollection<ushort>(_mixerLoad);

            // ADC
            _adcInputRaw = ReadRegisters((byte)Px4ioPage.AdcInputRaw,
                0, (byte)Configuration.AdcInputCount, requireCount: false);
            AdcInputRaw = new ReadOnlyCollection<ushort>(_adcInputRaw);

            // Actuators
            _actuators = ReadRegisters((byte)Px4ioPage.Actuators, 0, (byte)Configuration.ActuatorCount);
            Actuators = new ReadOnlyCollection<ushort>(_actuators);
            _servos = ReadRegisters((byte)Px4ioPage.Servos, 0, (byte)Configuration.ActuatorCount);
            Servos = new ReadOnlyCollection<ushort>(_servos);

            // PWM
            _pwm = ReadRegisters((byte)Px4ioPage.Pwm, 0, (byte)Configuration.ActuatorCount, requireCount: false);
            Pwm = new ReadOnlyCollection<ushort>(_pwm);
            _pwmDirect = ReadRegisters((byte)Px4ioPage.PwmDirect, 0, (byte)Configuration.ActuatorCount);
            PwmDirect = new ReadOnlyCollection<ushort>(_pwmDirect);
            _pwmFailsafe = ReadRegisters((byte)Px4ioPage.PwmFailsafe, 0, (byte)Configuration.ActuatorCount);
            PwmFailsafe = new ReadOnlyCollection<ushort>(_pwmFailsafe);
            _pwmMinimum = ReadRegisters((byte)Px4ioPage.PwmMinimum, 0, (byte)Configuration.ActuatorCount);
            PwmMinimum = new ReadOnlyCollection<ushort>(_pwmMinimum);
            _pwmMaximum = ReadRegisters((byte)Px4ioPage.PwmMaximum, 0, (byte)Configuration.ActuatorCount);
            PwmMaximum = new ReadOnlyCollection<ushort>(_pwmMaximum);
            _pwmDisarmed = ReadRegisters((byte)Px4ioPage.PwmDisarmed, 0, (byte)Configuration.ActuatorCount);
            PwmDisarmed = new ReadOnlyCollection<ushort>(_pwmDisarmed);

            // Sensors
            // TODO: Investigate RCIO sensors. Doesn't work on Navio 2 original firmware.
            //Sensors = new Px4ioSensorRegisters(ReadRegisters((byte)Px4ioPage.Sensors,
            //    0, Px4ioSensorRegisters.RegisterCount));
        }

        /// <summary>
        /// Sends a command to read a single register.
        /// </summary>
        /// <param name="page"><see cref="Px4ioPage"/> index.</param>
        /// <param name="offset"><see cref="Px4ioPage"/> register offset.</param>
        public ushort ReadRegister(byte page, byte offset)
        {
            // Create read request packet
            var request = new Px4ioPacket((byte)Px4ioRequestCode.Read, page, offset, 1);
            request.CalculateCrc();

            // Transfer and return result
            return Transfer(request).Registers[0];
        }

        /// <summary>
        /// Sends a command to read multiple registers.
        /// </summary>
        /// <param name="page"><see cref="Px4ioPage"/> index.</param>
        /// <param name="offset"><see cref="Px4ioPage"/> register offset.</param>
        /// <param name="count">
        /// Amount of data to read. When greater than <see cref="Px4ioPacket.CountMask"/>
        /// multiple transfers will occur.
        /// </param>
        /// <param name="requireCount">
        /// Indicates the exact <paramref name="count"/> must be returned, otherwise
        /// an error is thrown. Set true by default. Set false for requests which
        /// may return a variable count.
        /// </param>
        /// <remarks>
        /// Automatically splits the request into multiple transfers and joins
        /// resulting data when <paramref name="count"/> exceeds the value possible
        /// in <see cref="Px4ioPacket.CountMask"/>.
        /// </remarks>
        public ushort[] ReadRegisters(byte page, byte offset, ushort count, bool requireCount = true)
        {
            // Send requests
            var result = new List<ushort>();
            var resultOffset = (ushort)0;
            var remaining = count;
            do
            {
                // Create read request packet
                var requestOffset = (byte)(offset + resultOffset > byte.MaxValue ? byte.MaxValue : offset + resultOffset);
                var requestCount = (byte)(remaining > Px4ioPacket.RegistersMaximum ? Px4ioPacket.RegistersMaximum : remaining);
                var request = new Px4ioPacket((byte)Px4ioRequestCode.Read, page, requestOffset, requestCount);
                request.CalculateCrc();

                // Transfer and copy result
                var response = Transfer(request);
                result.AddRange(response.Registers);

                // Stop transfer when less data was received than requested
                if (response.Count != request.Count)
                {
                    if (requireCount)
                    {
                        // Error when same count required
                        throw new ArgumentOutOfRangeException(nameof(request));
                    }

                    // Okay to exit with less data
                    break;
                }

                // Next...
                resultOffset += requestCount;
                remaining -= requestCount;
            }
            while (remaining > 0);

            // Return result
            return result.ToArray();
        }

        /// <summary>
        /// Sends a command to write a register.
        /// </summary>
        /// <param name="page"><see cref="Px4ioPage"/> index.</param>
        /// <param name="offset"><see cref="Px4ioPage"/> register offset.</param>
        /// <param name="values">Data values to write.</param>
        public void WriteRegister(byte page, byte offset, ushort[] values)
        {
            // Create write request packet
            var request = new Px4ioPacket((byte)Px4ioRequestCode.Write, page, offset, values);
            request.CalculateCrc();

            // Transfer and return result
            var response = Transfer(request);
        }

        /// <summary>
        /// Sends a request packet, checks the returns the response.
        /// </summary>
        /// <param name="request">Request packet with <see cref="Px4ioPacket.Crc"/> calculated.</param>
        /// <returns>Response packet with data read and validated after request.</returns>
        /// <exception cref="FormatException">Thrown when corruption occurred during send or receive.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the request was rejected in error.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the returned register count doesn't match the requested count.</exception>
        public Px4ioPacket Transfer(Px4ioPacket request)
        {
            // Wait a bit
            // TODO: Wait for data ready interrupt from STM32 before reading.
            Task.Delay(TimeSpanExtensions.FromMicroseconds(150)).Wait();

            // Write request to SPI bus
            var writeBuffer = request.ToByteArray();
            _hardware.Write(writeBuffer);

            // Wait a bit
            // TODO: Wait for data ready interrupt from STM32 before reading.
            Task.Delay(TimeSpanExtensions.FromMicroseconds(150)).Wait();

            // Read response from SPI bus
            var readBuffer = new byte[Px4ioPacket.Size];
            _hardware.Read(readBuffer);

            // Check and return response when valid
            var response = new Px4ioPacket(readBuffer);
            switch ((Px4ioResponseCode)response.Code)
            {
                case Px4ioResponseCode.Corrupt:
                    {
                        // Rejected by RCIO as corrupt (CRC mismatch)
                        throw new FormatException(Resources.Strings.Px4ioPacketCorruptTransmit);
                    }

                case Px4ioResponseCode.Error:
                    {
                        // Rejected by RCIO as error (request specific)
                        throw new InvalidOperationException(Resources.Strings.Px4ioPacketError);
                    }

                case Px4ioResponseCode.Success:
                    {
                        // Successful response

                        // Check CRC
                        if (!response.ValidateCrc())
                            throw new FormatException(Resources.Strings.Px4ioPacketCorruptReceive);

                        // Return valid data
                        return response;
                    }

                default:
                    {
                        // Corrupt or unsupported response code
                        throw new FormatException(Resources.Strings.Px4ioPacketCorruptReceive);
                    }
            }
        }

        #endregion
    }
}