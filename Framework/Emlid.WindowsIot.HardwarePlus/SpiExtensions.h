#pragma once

#include <collection.h>
#include <experimental\resumable>
#include <ppltasks.h>
#include <pplawait.h>
#include <sstream>

using namespace std;
using namespace concurrency;

using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::Devices::Enumeration;
using namespace Windows::Devices::Spi;

namespace Emlid
{
	namespace WindowsIot
	{
		namespace HardwarePlus
		{
			namespace System
			{
				/// <summary>
				/// Extensions for work with SPI devices.
				/// </summary>
				public ref class SpiExtensions sealed
				{

				public:

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
					static SpiDevice^ Connect(int busNumber, int chipSelectLine, SpiMode mode, int dataBitLength, int clockFrequency);

					/// <summary>
					/// Asynchronous overload of <see cref="Connect" />.
					/// </summary>
					static IAsyncOperation<SpiDevice^>^ ConnectAsync(int busNumber, int chipSelectLine, SpiMode mode, int dataBitLength, int clockFrequency);
				};
			}
		}
	}
}
