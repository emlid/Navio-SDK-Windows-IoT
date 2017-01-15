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
using namespace Windows::Devices::I2c;

namespace Emlid
{
	namespace WindowsIot
	{
		namespace HardwarePlus
		{
			namespace System
			{
				/// <summary>
				/// Extensions for work with I2C devices.
				/// </summary>
				public ref class I2cExtensions sealed
				{

				public:

					/// <summary>
					/// Maximum transfer size for I2C requests on Windows IoT or Raspberry Pi 2.
					/// This is a confirmed soft limitation by Microsoft, it should be 64K.
					/// </summary>
					/// <seealso href="https://social.msdn.microsoft.com/Forums/en-US/e938900f-b732-41dc-95f6-058a39dac31d/i2c-transfer-limit-of-16384-bytes-on-raspberry-pi-2?forum=WindowsIoT"/>
					static property int MaximumTransferSize { int get() { return 16384;	} }

					/// <summary>
					/// Connects to an I2C device if it exists.
					/// </summary>
					/// <param name="busNumber">Bus controller number, zero based.</param>
					/// <param name="address">7-bit I2C slave address (8 bit addresses must be shifted down to exclude the read/write bit).</param>
					/// <param name="speed">Bus speed.</param>
					/// <param name="sharingMode">Sharing mode.</param>
					/// <returns>Device when the bus controller and device exist, otherwise null.</returns>
					static I2cDevice^ Connect(int busNumber, int address, I2cBusSpeed speed, I2cSharingMode sharingMode);

					/// <summary>
					/// Asynchronous overload of <see cref="Connect" />.
					/// </summary>
					static IAsyncOperation<I2cDevice^>^ ConnectAsync(int busNumber, int address, I2cBusSpeed speed, I2cSharingMode sharingMode);
				};
			}
		}
	}
}
