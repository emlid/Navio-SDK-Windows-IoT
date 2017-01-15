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
using namespace Windows::Devices::Gpio;

namespace Emlid
{
	namespace WindowsIot
	{
		namespace HardwarePlus
		{
			namespace System
			{
				/// <summary>
				/// Extensions for work with GPIO devices.
				/// </summary>
				public ref class GpioExtensions sealed
				{

				public:

					/// <summary>
					/// Connects to a GPIO pin if it exists.
					/// </summary>
					/// <param name="busNumber">Bus controller number, zero based.</param>
					/// <param name="pinNumber">Pin number.</param>
					/// <param name="driveMode">Drive mode.</param>
					/// <param name="sharingMode">Sharing mode.</param>
					/// <returns>Pin when controller and device exist, otherwise null.</returns>
					static GpioPin^ Connect(int busNumber, int pinNumber, GpioPinDriveMode driveMode, GpioSharingMode sharingMode);

					/// <summary>
					/// Asynchronous overload of <see cref="Connect" />.
					/// </summary>
					static IAsyncOperation<GpioPin^>^ ConnectAsync(int busNumber, int pinNumber, GpioPinDriveMode driveMode, GpioSharingMode sharingMode);
				};
			}
		}
	}
}
