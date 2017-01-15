using Emlid.UniversalWindows;
using Emlid.WindowsIot.Hardware.System;
using System;
using System.Globalization;
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
    public sealed class Navio2RcioDevice : DisposableObject
    {
        #region Constants

        /// <summary>
        /// SPI bus controller number.
        /// </summary>
        public const int SpiBusNumber = 1;

        /// <summary>
        /// SPI Chip Select Line (CSL) number.
        /// </summary>
        public const int SpiChipSelectLine = 1;

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
        [CLSCompliant(false)]
        public const SpiMode SpiOperationMode = SpiMode.Mode0;

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates and initializes an instance.
        /// </summary>
        public Navio2RcioDevice()
        {
            // Configure GPIO
            _device = SpiExtensions.Connect(SpiBusNumber, SpiChipSelectLine, SpiFrequency, SpiBitsPerWord, SpiOperationMode)
                .GetAwaiter().GetResult();
            if (_device == null)
            {
                // Initialization error
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    Resources.Strings.SpiErrorDeviceNotFound, SpiBusNumber));
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
        private readonly SpiDevice _device;

        #endregion
    }
}
