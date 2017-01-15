#include "GpioExtensions.h"

using namespace Emlid::WindowsIot::HardwarePlus::System;

IAsyncOperation<GpioPin^>^ GpioExtensions::ConnectAsync(int busNumber, int pinNumber, GpioPinDriveMode driveMode, GpioSharingMode sharingMode)
{
	// Validate
	if (busNumber < 0) throw ref new InvalidArgumentException(L"busNumber");
	if (pinNumber < 0) throw ref new InvalidArgumentException(L"pinNumber");

	// Asynchronous operation
	return create_async([=]() -> task<GpioPin^>
	{
		// Get controller (return null when doesn't exist)
		// TODO: support multiple controllers (after lightning)
		if (busNumber >= 1)
			throw ref new InvalidArgumentException(L"busNumber");
		auto controller = co_await GpioController::GetDefaultAsync();

		// Connect to device (return null when doesn't exist)
		auto pin = controller->OpenPin(pinNumber, sharingMode);
		if (!pin)
			return nullptr;
		try
		{
			// Configure and return pin
			if (pin->GetDriveMode() != driveMode)
				pin->SetDriveMode(driveMode);

			// Return connected device
			return pin;
		}
		catch (Exception^ error)
		{
			// Free pin on error during initialization
			if (pin != nullptr) delete pin;

			// Continue error
			throw error;
		}
	});
}

GpioPin^ GpioExtensions::Connect(int busNumber, int pinNumber, GpioPinDriveMode driveMode, GpioSharingMode sharingMode)
{
	return GpioExtensions::ConnectAsync(busNumber, pinNumber, driveMode, sharingMode)->GetResults();
}