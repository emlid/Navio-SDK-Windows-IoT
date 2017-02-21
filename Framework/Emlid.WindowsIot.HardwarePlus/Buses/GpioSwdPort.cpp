#include "pch.h"
#include "GpioSwdPort.h"

using namespace Emlid::WindowsIot::HardwarePlus::Buses;

#pragma region Lifetime

GpioSwdPort::GpioSwdPort(int busNumber, int clockPinNumber, int ioPinNumber)
{
	// Validate
	if (busNumber < 0) throw winrt::hresult_invalid_argument(L"busNumber");
	if (clockPinNumber < 0) throw winrt::hresult_invalid_argument(L"clockPinNumber");
	if (ioPinNumber < 0) throw winrt::hresult_invalid_argument(L"ioPinNumber");

	// Get controller (return null when doesn't exist)
	// TODO: support multiple controllers (after lightning)
	if (busNumber >= 1)
		throw winrt::hresult_invalid_argument(L"busNumber");
	auto controller = winrt::GpioController::GetDefaultAsync().get();

	// Open clock pin
	_clockPin = controller.OpenPin(clockPinNumber, winrt::GpioSharingMode::Exclusive);
	if (_clockPin == nullptr) throw new winrt::hresult_out_of_bounds(L"clockPinNumber");
	if (_clockPin.GetDriveMode() != winrt::GpioPinDriveMode::Output)
		_clockPin.SetDriveMode(winrt::GpioPinDriveMode::Output);

	// Open IO pin
	_ioPin = controller.OpenPin(ioPinNumber, winrt::GpioSharingMode::Exclusive);
	if (_ioPin == nullptr) throw new winrt::hresult_out_of_bounds(L"ioPinNumber");
	if (_ioPin.GetDriveMode() != winrt::GpioPinDriveMode::Output)
		_ioPin.SetDriveMode(winrt::GpioPinDriveMode::Output);
}

GpioSwdPort::~GpioSwdPort()
{
	// Free resources
	if (_ioPin != nullptr)
		_ioPin.Close();
	if (_clockPin != nullptr)
		_clockPin.Close();
}

#pragma endregion

#pragma region Private Methods

void GpioSwdPort::Clock()
{
	// Clock high
	_clockPin.Write(winrt::GpioPinValue::High);

	// Clock low
	_clockPin.Write(winrt::GpioPinValue::Low);
}

void GpioSwdPort::Turn(bool drive)
{
	// Get current drive mode
	auto current = _ioPin.GetDriveMode();

	// Do nothing when same
	auto requested = drive ? winrt::GpioPinDriveMode::Output : winrt::GpioPinDriveMode::Input;
	if (current == requested)
		return;

	// Change direction
	if (!drive) Clock();				// Clock cycle before when changing to read mode
	_ioPin.SetDriveMode(requested);
	if (drive) Clock();					// Clock cycle afterwards when changing to wrire mode
}

bool GpioSwdPort::ReadBit()
{
	// Read IO pin level as value
	bool value = _ioPin.Read() == winrt::GpioPinValue::High;

	// Cycle clock to complete transaction
	Clock();

	// Return result
	return value;
}

uint32 GpioSwdPort::ReadBits(byte count)
{
	// Read all bits
	uint32 result = 0, bit = 1;
	while (count--)
	{
		// Read bit and store in result when set
		if (ReadBit()) result |= bit;

		// Next bit...
		bit <<= 1;
	}

	// Return result
	return result;
}

bool GpioSwdPort::ReadBitsWithParity(uint32* readData, byte count)
{
	// Read all data bits
	uint32 result = 0, bit = 1;
	byte parity = 0;
	while (count--)
	{
		// Read bit, store in result and add partiy when set
		if (ReadBit())
		{
			result |= bit;
			parity ^= 1;
		}

		// Next bit...
		bit <<= 1;
	}

	// Read parity bit
	bool parityBit = ReadBit();
	bool valid = parityBit && (parity > 0);

	// Return result
	*readData = result;
	return valid;
}

void GpioSwdPort::WriteBit(bool value)
{
	// Write IO pin level as value
	auto level = value ? winrt::GpioPinValue::High : winrt::GpioPinValue::Low;
	_ioPin.Write(level);

	// Cycle clock to complete transaction
	Clock();
}

void GpioSwdPort::WriteBits(uint32 value, byte count)
{
	// Write all bits
	while (count--)
	{
		// Get bit value
		bool bit = value & 1;

		// Write bit
		WriteBit(bit);

		// Next bit
		value >>= 1;
	}
}

void GpioSwdPort::WriteBitsWithParity(uint32 value, byte count)
{
	// Read all data bits
	byte parity = 0;
	while (count--)
	{
		// Get bit value
		bool bit = (value & 1) > 0;

		// Write bit
		WriteBit(bit);

		// Add partiy
		parity ^= value;

		// Next bit
		value >>= 1;
	}

	// Write parity bit
	bool parityBit = (parity & 1) > 0;
	WriteBit(parityBit);
}

#pragma endregion

#pragma region IO

void GpioSwdPort::Reset()
{
	// Set write mode
	Turn(true);

	// Set IO pin high
	_ioPin.Write(winrt::GpioPinValue::High);

	// Send 50 clock cycles
	for (byte count = 0; count < 50; count++)
		Clock();

	// Send 2 idle cycles (with IO pin low)
	_ioPin.Write(winrt::GpioPinValue::Low);
	Clock();
	Clock();
}

/// <summary>
/// Reads one byte from the SWD port.
/// </summary>
byte GpioSwdPort::ReadByte()
{

	throw winrt::hresult_not_implemented();
}

/// <summary>
/// Reads multiple bytes from the SWD port.
/// </summary>
cx::Array<byte>^ GpioSwdPort::ReadBytes()
{
	throw winrt::hresult_not_implemented();
}

/// <summary>
/// Writes one byte from the SWD port.
/// </summary>
void GpioSwdPort::WriteByte(byte writeData)
{
	writeData++;	// Avoid code analysis warning=error
	throw winrt::hresult_not_implemented();
}

/// <summary>
/// Writes multiple bytes from the SWD port.
/// </summary>
void GpioSwdPort::WriteBytes(const cx::Array<byte>^ writeData)
{
	writeData->begin();	// Avoid code analysis warning=error
	throw winrt::hresult_not_implemented();
}

#pragma endregion
