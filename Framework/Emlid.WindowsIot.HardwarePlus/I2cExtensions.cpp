#include "I2cExtensions.h"

using namespace Emlid::WindowsIot::HardwarePlus::System;

IAsyncOperation<I2cDevice^>^ I2cExtensions::ConnectAsync(int busNumber, int address, I2cBusSpeed speed, I2cSharingMode sharingMode)
{
	// Validate
	if (busNumber < 0) throw ref new InvalidArgumentException(L"busNumber");
	if (address < 0 || address > 0x7f) throw ref new InvalidArgumentException(L"address");

	// Asynchronous operation
	return create_async([=]() -> task<I2cDevice^>
	{
		// Query bus information
		String^ query = I2cDevice::GetDeviceSelector();
		auto busInformation = co_await DeviceInformation::FindAllAsync(query);
		if (busInformation->Size < 1)
			throw ref new InvalidArgumentException(L"busNumber");

		// Configure connection
		String^ id = busInformation->GetAt(0)->Id;
		auto settings = ref new I2cConnectionSettings(address);
		if (int(speed) != -1) settings->BusSpeed = speed;
		if (int(sharingMode) != -1) settings->SharingMode = sharingMode;

		// Connect to device and return (null when failed)
		return co_await I2cDevice::FromIdAsync(id, settings);
	});
}

I2cDevice^ I2cExtensions::Connect(int busNumber, int address, I2cBusSpeed speed, I2cSharingMode sharingMode)
{
	return I2cExtensions::ConnectAsync(busNumber, address, speed, sharingMode)->GetResults();
}