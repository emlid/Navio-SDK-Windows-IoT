#include "pch.h"
#include "SpiExtensions.h"

using namespace Emlid::WindowsIot::HardwarePlus::Buses;

cx::SpiDevice^ SpiExtensions::Connect(int busNumber, int chipSelectLine, cx::SpiMode modeCX,
	int dataBitLength, int clockFrequency, cx::SpiSharingMode sharingModeCX)
{
	// TODO: Remove argument conversion when C++/WinRT supports components
	winrt::SpiMode mode = (winrt::SpiMode)modeCX;
	winrt::SpiSharingMode sharingMode = (winrt::SpiSharingMode)sharingModeCX;

	// C++/WinRT Start -----------------------------------------------------------

	// Validate
	if (busNumber < 0) throw winrt::hresult_invalid_argument(L"busNumber");
	if (chipSelectLine < 0) throw winrt::hresult_invalid_argument(L"chipSelectLine");

	// Query bus information
	auto query = winrt::SpiDevice::GetDeviceSelector();
	auto busInformation = winrt::DeviceInformation::FindAllAsync(query).get();
	if (busInformation.Size() < 1)
		throw winrt::hresult_invalid_argument(L"busNumber");

	// Configure connection
	auto id = busInformation.GetAt(busNumber).Id();
	auto settings = winrt::SpiConnectionSettings(chipSelectLine);
	settings.Mode(mode);
	settings.DataBitLength(dataBitLength);
	settings.ClockFrequency(clockFrequency);
	settings.SharingMode(sharingMode);

	// Connect to device and return (null when failed)
	auto device = winrt::SpiDevice::FromIdAsync(id, settings).get();

	// C++/WinRT End -------------------------------------------------------------

	// TODO: Remove result conversion when C++/WinRT supports components
	return reinterpret_cast<cx::SpiDevice^>(winrt::get_abi(device));
}
