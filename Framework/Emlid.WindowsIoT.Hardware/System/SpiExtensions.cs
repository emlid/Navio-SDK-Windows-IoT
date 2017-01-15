using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
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
        /// <param name="busNumber">Bus controller number, zero based.</param>
        /// <param name="chipSelectLine">Slave Chip Select Line.</param>
        /// <param name="bits">Data length in bits.</param>
        /// <param name="frequency">Frequency in Hz.</param>
        /// <param name="mode">Communication mode, i.e. clock polarity.</param>
        /// <param name="sharingMode">Sharing mode.</param>
        /// <returns>Device when the bus controller and device exist, otherwise null.</returns>
        public async static Task<SpiDevice> Connect(int busNumber, int chipSelectLine, int frequency, int bits,
            SpiMode mode, SpiSharingMode sharingMode = SpiSharingMode.Exclusive)
        {
            // Validate
            if (busNumber < 0) throw new ArgumentOutOfRangeException(nameof(busNumber));
            if (chipSelectLine < 0) throw new ArgumentOutOfRangeException(nameof(chipSelectLine));
            if (frequency < 0) throw new ArgumentOutOfRangeException(nameof(frequency));
            if (bits < 0) throw new ArgumentOutOfRangeException(nameof(bits));

            // Lookup bus controller
            var controllers = await DeviceInformation.FindAllAsync(SpiDevice.GetDeviceSelector());
            if (busNumber >= controllers.Count)
                throw new ArgumentOutOfRangeException(nameof(busNumber));
            var busId = controllers[busNumber].Id;

            // Create connection settings
            var settings = new SpiConnectionSettings(chipSelectLine)
            {
                ClockFrequency = frequency,
                DataBitLength = bits,
                Mode = mode,
                SharingMode = sharingMode
            };

            // Create and return device
            return await SpiDevice.FromIdAsync(busId, settings);
        }
    }
}
