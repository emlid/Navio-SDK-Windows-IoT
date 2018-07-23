#pragma once

#include "pch.h"

namespace Emlid
{
	namespace WindowsIot
	{
		namespace HardwarePlus
		{
			namespace Buses
			{
				/// <summary>
				/// Extensions for work with SPI devices.
				/// </summary>
				public ref class SpiExtensions sealed
				{

				private:

					//SpiExtensions(); // Abstract class

				public:

					/// <summary>
					/// Connects to an SPI device if it exists.
					/// </summary>
					/// <param name="busNumber">Bus controller number, zero based.</param>
					/// <param name="chipSelectLine">Slave Chip Select Line.</param>
					/// <param name="mode">Communication mode, i.e. clock polarity.</param>
					/// <param name="dataBitLength">Data length in bits.</param>
					/// <param name="clockFrequency">Frequency in Hz.</param>
					/// <returns>Device when the bus controller and device exist, otherwise null.</returns>
					static cx::SpiDevice^ Connect(int busNumber, int chipSelectLine, cx::SpiMode mode, int dataBitLength, int clockFrequency, cx::SpiSharingMode sharingMode);
				};
			}
		}
	}
}
