using Emlid.UniversalWindows;
using Emlid.WindowsIot.HardwarePlus.Buses;
using System;
using System.Globalization;
using Windows.Devices.Spi;
using Emlid.WindowsIot.Hardware.Protocols.Ppm;
using System.Collections.ObjectModel;
using Emlid.WindowsIot.Hardware.Components.Px4io;
using Emlid.WindowsIot.Hardware.Components.Px4io.Data;
using System.Collections.Generic;

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
        /// SPI bus controller number.
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

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates and initializes an instance.
        /// </summary>
        public Navio2RcioDevice()
        {
            // Create device
            _device = new Px4ioDevice(SpiBusNumber, SpiChipSelectLine, SpiOperationMode, SpiBitsPerWord, SpiFrequency, SpiSharingMode.Exclusive);

            // Initialize members
            _channels = new int[_device.Configuration.RCInputCount];
            _channelsReadOnly = new ReadOnlyCollection<int>(_channels);
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
            // Only managed resources to dispose
            if (!disposing)
                return;

            //  Close device
            _device?.Dispose();
        }

        #endregion

        #endregion

        #region Private Fields

        /// <summary>
        /// SPI connection to the RCIO co-processor.
        /// </summary>
        private readonly Px4ioDevice _device;

        #endregion

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

        #endregion

        #region Events

        /// <summary>
        /// Fired after a new frame of data has been received and decoded into <see cref="INavioRCInputDevice.Channels"/>.
        /// </summary>
        event EventHandler<PpmFrame> INavioRCInputDevice.ChannelsChanged
        {
            add { _channelsChanged += value; }
            remove { _channelsChanged -= value; }
        }
        private EventHandler<PpmFrame> _channelsChanged;

        #endregion

        #endregion
    }
}
