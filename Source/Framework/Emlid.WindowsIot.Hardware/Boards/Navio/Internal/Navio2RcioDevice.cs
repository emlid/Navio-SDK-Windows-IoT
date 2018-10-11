using CodeForDotNet;
using Emlid.WindowsIot.Hardware.Components.Px4io;
using Emlid.WindowsIot.Hardware.Protocols.Ppm;
using Emlid.WindowsIot.HardwarePlus.Buses;
using System;
using System.Collections.ObjectModel;
using Windows.Devices.Gpio;
using Windows.Devices.Spi;

namespace Emlid.WindowsIot.Hardware.Boards.Navio.Internal
{
    /// <summary>
    /// Navio 2 Remote Control Input Output (RCIO) hardware device.
    /// </summary>
    /// <remarks>
    /// Navio 2 provides ADC, RC (receiver) input SBUS and CPPM decoding, and RC PWM output via
    /// firmware running on an ARM co-processor connected to via the Raspberry Pi auxiliary SPI bus.
    /// </remarks>
    [CLSCompliant(false)]
    public sealed class Navio2RcioDevice : DisposableObject, INavioRCInputDevice // TODO: INavioPwmDevice, INavioImuDevice x 2
    {
        #region Constants

        /// <summary>
        /// SPI bus controller number, zero based.
        /// </summary>
        public const int SpiBusNumber = 1;

        /// <summary>
        /// SPI Chip Select Line (CSL) number.
        /// </summary>
        public const int SpiChipSelectLine = 0;

        /// <summary>
        /// SPI speed in Hz.
        /// </summary>
        public const int SpiFrequency = 1000000;

        /// <summary>
        /// SPI bits per word.
        /// </summary>
        public const int SpiBitsPerWord = 8;

        /// <summary>
        /// SPI operation mode.
        /// </summary>
        public const SpiMode SpiOperationMode = SpiMode.Mode0;

        /// <summary>
        /// Maximum number of RC input channels.
        /// </summary>
        public const int RcInputChannelsMaximum = 16;

        /// <summary>
        /// GPIO bus controller number, zero based.
        /// </summary>
        public const int GpioBusNumber = 0;

        /// <summary>
        /// GPIO pin for the RCIO "PC11" interrupt pin.
        /// </summary>
        public const int GpioInterruptPinNumber = 5;

        /// <summary>
        /// GPIO pin for the RCIO SWD_CLK pin.
        /// </summary>
        public const int GpioSwdClockPinNumber = 12;

        /// <summary>
        /// GPIO pin for the RCIO SWD_IO pin.
        /// </summary>
        public const int GpioSwdIoPinNumber = 13;

        #endregion Constants

        #region Lifetime

        /// <summary>
        /// Creates and initializes an instance.
        /// </summary>
        public Navio2RcioDevice()
        {
            try
            {
                // Create device
                _chip = new Px4ioDevice(SpiBusNumber, SpiChipSelectLine, SpiOperationMode, SpiBitsPerWord, SpiFrequency, SpiSharingMode.Exclusive);
                _swdPort = new GpioSwdPort(GpioBusNumber, GpioSwdClockPinNumber, GpioSwdIoPinNumber);
                _interruptPin = GpioExtensions.Connect(GpioBusNumber, GpioInterruptPinNumber, GpioPinDriveMode.Input, GpioSharingMode.Exclusive);
                _interruptPin.ValueChanged += OnInterruptPinValueChanged;

                // Initialize members
                _channels = new int[_chip.Configuration.RCInputCount];
                _channelsReadOnly = new ReadOnlyCollection<int>(_channels);
            }
            catch
            {
                // Close devices in case partially initialized
                _interruptPin?.Dispose();
                _swdPort?.Dispose();
                _chip?.Dispose();

                // Continue error
                throw;
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
            // Only dispose managed resources
            if (!disposing)
                return;

            // Close devices
            if (_interruptPin != null)
            {
                _interruptPin.ValueChanged -= OnInterruptPinValueChanged;
                _interruptPin.Dispose();
            }
            _swdPort?.Dispose();
            _chip?.Dispose();
        }

        #endregion IDisposable

        #endregion Lifetime

        #region Private Fields

        /// <summary>
        /// SPI connection to the RCIO co-processor.
        /// </summary>
        private readonly Px4ioDevice _chip;

        /// <summary>
        /// RCIO GPIO interrupt pin.
        /// </summary>
        private GpioPin _interruptPin;

        /// <summary>
        /// RCIO Serial Wire Debug GPIO port.
        /// </summary>
        private GpioSwdPort _swdPort;

        #endregion Private Fields

        #region INavioRCInputDevice

        #region Properties

        /// <summary>
        /// Channel values in microseconds.
        /// </summary>
        ReadOnlyCollection<int> INavioRCInputDevice.Channels => _channelsReadOnly;

        private ReadOnlyCollection<int> _channelsReadOnly;
        private int[] _channels;

        /// <summary>
        /// Returns false because multiple protocols are not supported, only CPPM.
        /// </summary>
        bool INavioRCInputDevice.Multiprotocol { get { return true; } }

        #endregion Properties

        #region Events

        /// <summary>
        /// Updates RCIO register values when it fires the interrupt.
        /// </summary>
        private void OnInterruptPinValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (args.Edge == GpioPinEdge.RisingEdge)
                _chip.Read();
        }

        /// <summary>
        /// Fired after a new frame of data has been received and decoded into <see cref="INavioRCInputDevice.Channels"/>.
        /// </summary>
        event EventHandler<PpmFrame> INavioRCInputDevice.ChannelsChanged
        {
            add { _channelsChanged += value; }
            remove { _channelsChanged -= value; }
        }

        private EventHandler<PpmFrame> _channelsChanged;

        #endregion Events

        #endregion INavioRCInputDevice
    }
}