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
				/// Extensions for work with GPIO devices.
				/// </summary>
				public ref class GpioExtensions sealed
				{

				private:

					GpioExtensions();	// Abstract class

				public:

					/// <summary>
					/// Connects to a GPIO pin if it exists.
					/// </summary>
					/// <param name="busNumber">Bus controller number, zero based.</param>
					/// <param name="pinNumber">Pin number.</param>
					/// <param name="driveMode">Drive mode.</param>
					/// <param name="sharingMode">Sharing mode.</param>
					/// <returns>Pin when controller and device exist, otherwise null.</returns>
					static cx::GpioPin^ Connect(int busNumber, int pinNumber, cx::GpioPinDriveMode driveMode, cx::GpioSharingMode sharingMode);
				};
			}
		}
	}
}
