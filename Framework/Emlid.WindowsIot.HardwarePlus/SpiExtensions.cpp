#include "SpiExtensions.h"

using namespace Emlid::WindowsIot::HardwarePlus::System;

IAsyncOperation<SpiDevice^>^ SpiExtensions::ConnectAsync(int busNumber, int chipSelectLine, SpiMode mode, int dataBitLength, int clockFrequency)
{
	// Validate
	if (busNumber < 0) throw ref new InvalidArgumentException(L"busNumber");
	if (chipSelectLine < 0) throw ref new InvalidArgumentException(L"chipSelectLine");

	// Asynchronous operation
	return create_async([=]() -> task<SpiDevice^>
	{
		// Query bus information
		String^ query = SpiDevice::GetDeviceSelector();
		auto busInformation = co_await DeviceInformation::FindAllAsync(query);
		if (busInformation->Size < 1)
			throw ref new InvalidArgumentException(L"busNumber");

		// Configure connection
		String^ id = busInformation->GetAt(0)->Id;
		auto settings = ref new SpiConnectionSettings(chipSelectLine);
		if (int(mode) != -1) settings->Mode = mode;
		if (dataBitLength != 0) settings->DataBitLength = dataBitLength;
		if (clockFrequency != 0) settings->ClockFrequency = clockFrequency;

		// Connect to device and return (null when failed)
		return co_await SpiDevice::FromIdAsync(id, settings);
	});
}

SpiDevice^ SpiExtensions::Connect(int busNumber, int chipSelectLine, SpiMode mode, int dataBitLength, int clockFrequency)
{
	return SpiExtensions::ConnectAsync(busNumber, chipSelectLine, mode, dataBitLength, clockFrequency)->GetResults();
}