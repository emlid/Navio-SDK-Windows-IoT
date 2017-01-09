using System;
using Windows.Devices.Spi;

namespace Emlid.WindowsIot.Hardware.System
{
    /// <summary>
    /// Extensions for work with SPI devices.
    /// </summary>
    [CLSCompliant(false)]
    public static class SpiExtensions
    {
        /// <summary>
        /// Connects to an SPI device if it exists.
        /// </summary>
        /// <param name="controller">Controller.</param>
        /// <param name="chipSelectLine">Slave Chip Select Line.</param>
        /// <param name="bits">Data length in bits.</param>
        /// <param name="frequency">Frequency in Hz.</param>
        /// <param name="mode">Communication mode, i.e. clock polarity.</param>
        /// <param name="sharingMode">Sharing mode.</param>
        /// <returns>Device when controller and device exist, otherwise null.</returns>
        public static SpiDevice Connect(this SpiController controller, int chipSelectLine, int frequency, int bits,
            SpiMode mode, SpiSharingMode sharingMode = SpiSharingMode.Exclusive)
        {
            // Validate
            if (chipSelectLine < 0) throw new ArgumentOutOfRangeException(nameof(chipSelectLine));
            // TODO: Add further validation and constants from SPI specification
            if (frequency < 0) throw new ArgumentOutOfRangeException(nameof(frequency));
            if (bits < 0) throw new ArgumentOutOfRangeException(nameof(bits));

            // Connect to device and return (if exists)
            var settings = new SpiConnectionSettings(chipSelectLine)
            {
                ClockFrequency = frequency,
                DataBitLength = bits,
                Mode = mode,
                SharingMode = sharingMode
            };
            return controller.GetDevice(settings);
        }
    }
}
