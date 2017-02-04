#include "pch.h"
#include "GpioExtensions.h"

using namespace Emlid::WindowsIot::HardwarePlus::Buses;

cx::GpioPin^ GpioExtensions::Connect(int busNumber, int pinNumber, cx::GpioPinDriveMode driveModeCX, cx::GpioSharingMode sharingModeCX)
{
	// TODO: Remove argument conversion when C++/WinRT supports components
	winrt::GpioPinDriveMode driveMode = (winrt::GpioPinDriveMode)driveModeCX;
	winrt::GpioSharingMode sharingMode = (winrt::GpioSharingMode)sharingModeCX;

	// C++/WinRT Start -----------------------------------------------------------

	// Validate
	if (busNumber < 0) throw winrt::hresult_invalid_argument(L"busNumber");
	if (pinNumber < 0) throw winrt::hresult_invalid_argument(L"pinNumber");

	// Get controller (return null when doesn't exist)
	// TODO: support multiple controllers (after lightning)
	if (busNumber >= 1)
		throw winrt::hresult_invalid_argument(L"busNumber");
	auto controller = winrt::GpioController::GetDefaultAsync().get();

	// Connect to device (return null when doesn't exist)
	auto pin = controller.OpenPin(pinNumber, sharingMode);
	if (!pin)
		return nullptr;

	// Configure and return pin
	if (pin.GetDriveMode() != driveMode)
		pin.SetDriveMode(driveMode);

	// C++/WinRT End -------------------------------------------------------------

	// TODO: Remove result conversion when C++/WinRT supports components
	return reinterpret_cast<cx::GpioPin^>(winrt::get(pin));
}
