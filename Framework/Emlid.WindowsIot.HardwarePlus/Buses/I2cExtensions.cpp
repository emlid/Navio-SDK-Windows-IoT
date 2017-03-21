#include "pch.h"
#include "I2cExtensions.h"

using namespace Emlid::WindowsIot::HardwarePlus::Buses;

#pragma region Connect

cx::I2cDevice^ I2cExtensions::Connect(int busNumber, int address, cx::I2cBusSpeed speedCX, cx::I2cSharingMode sharingModeCX)
{
	// TODO: Remove argument conversion when C++/WinRT supports components
	winrt::I2cBusSpeed speed = (winrt::I2cBusSpeed)speedCX;
	winrt::I2cSharingMode sharingMode = (winrt::I2cSharingMode)sharingModeCX;

	// C++/WinRT Start -----------------------------------------------------------

	// Validate
	if (busNumber < 0) throw winrt::hresult_invalid_argument(L"busNumber");
	if (address < 0 || address > 0x7f) throw winrt::hresult_invalid_argument(L"address");

	// Query bus information
	auto query = winrt::I2cDevice::GetDeviceSelector();
	auto busInformation = winrt::DeviceInformation::FindAllAsync(query).get();
	if (busInformation.Size() < 1)
		throw winrt::hresult_invalid_argument(L"busNumber");

	// Configure connection
	auto id = busInformation.GetAt(busNumber).Id();
	auto settings = winrt::I2cConnectionSettings(address);
	settings.BusSpeed(speed);
	settings.SharingMode(sharingMode);

	// Connect to device and return (null when failed)
	auto device = winrt::I2cDevice::FromIdAsync(id, settings).get();

	// C++/WinRT End -------------------------------------------------------------

	// Return connected device
	return reinterpret_cast<cx::I2cDevice^>(winrt::detach_abi(device));
}

#pragma endregion


#pragma region Read

byte I2cExtensions::ReadByte(cx::I2cDevice^ device)
{
	// Call overloaded method and return result
	return ReadBytes(device, 1)[0];
}

cx::Array<byte>^ I2cExtensions::ReadBytes(cx::I2cDevice^ device, int size)
{
	// Create buffer
	auto readBuffer = ref new cx::Array<byte>(size);

	// Call extended device method
	device->Read(readBuffer);

	// Return buffer
	return readBuffer;
}

byte I2cExtensions::WriteReadByte(cx::I2cDevice^ device, byte writeData)
{
	// Cast argument to correct type
	auto writeBuffer = ref new cx::Array<byte>(&writeData, 1);

	// Call overloaded method
	return WriteReadBytes(device, writeBuffer, 1)[0];
}

byte I2cExtensions::WriteReadByte(cx::I2cDevice^ device, const cx::Array<byte>^ writeData)
{
	// Call overloaded method
	auto readBuffer = WriteReadBytes(device, writeData, 1);

	// Return first byte in buffer
	return readBuffer[0];
}

cx::Array<byte>^ I2cExtensions::WriteReadBytes(cx::I2cDevice^ device, byte writeData, int size)
{
	// Cast data to correct type
	auto writeBuffer = ref new cx::Array<byte>(&writeData, 1);

	// Call overloaded method
	return WriteReadBytes(device, writeBuffer, size);
}

cx::Array<byte>^ I2cExtensions::WriteReadBytes(cx::I2cDevice^ device, const cx::Array<byte>^ writeData, int size)
{
	// Create buffer
	auto readBuffer = ref new cx::Array<byte>(size);

	// Call extended device method
	device->WriteRead(writeData, readBuffer);

	// Return buffer
	return readBuffer;
}

bool I2cExtensions::WriteReadBit(cx::I2cDevice^ device, byte writeData, byte mask)
{
	// Cast data to correct type
	auto writeBuffer = ref new cx::Array<byte>(&writeData, 1);

	// Call overloaded method and return result
	return WriteReadBit(device, writeBuffer, mask);
}

bool I2cExtensions::WriteReadBit(cx::I2cDevice^ device, const cx::Array<byte>^ writeData, byte mask)
{
	// Read byte
	auto readByte = WriteReadByte(device, writeData);

	// Apply mask and return true when set
	return (readByte & mask) != 0;
}

#pragma endregion

#pragma region Write

void I2cExtensions::WriteByte(cx::I2cDevice^ device, byte writeData)
{
	// Cast data to correct type
	auto writeBuffer = ref new cx::Array<byte>(&writeData, 1);

	// Call extended device function
	device->Write(writeBuffer);
}

/// <summary>
/// Writes one or more data bytes.
/// </summary>
/// <param name="device">Device to use.</param>
/// <param name="data">Data to write.</param>
void I2cExtensions::WriteBytes(cx::I2cDevice^ device, const cx::Array<byte>^ writeData)
{
	// Call extended device function
	device->Write(writeData);
}

/// <summary>
/// Joins two byte values then writes them.
/// </summary>
/// <param name="device">Device to use.</param>
/// <param name="data1">First part of data to write.</param>
/// <param name="data2">Second part of data to write.</param>
void I2cExtensions::WriteJoinByte(cx::I2cDevice^ device, byte writeData1, byte writeData2)
{
	// Create buffer
	auto writeBuffer = ref new cx::Array<byte>(2) { writeData1, writeData2 };

	// Call extended device method
	device->Write(writeBuffer);
}

/// <summary>
/// Joins two byte values then writes them.
/// </summary>
/// <param name="device">Device to use.</param>
/// <param name="data1">First part of data to write.</param>
/// <param name="data2">Second part of data to write.</param>
void I2cExtensions::WriteJoinByte(cx::I2cDevice^ device, const cx::Array<byte>^ writeData1, byte writeData2)
{
	// Cast data to correct type
	auto data2Array = ref new cx::Array<byte>(&writeData2, 1);

	// Call overloaded method
	WriteJoinBytes(device, writeData1, data2Array);
}

/// <summary>
/// Joins two byte values then writes them.
/// </summary>
/// <param name="device">Device to use.</param>
/// <param name="data1">First part of data to write.</param>
/// <param name="data2">Second part of data to write.</param>
void I2cExtensions::WriteJoinBytes(cx::I2cDevice^ device, byte writeData1, const cx::Array<byte>^ writeData2)
{
	// Cast data to correct type
	auto data1Array = ref new cx::Array<byte>(&writeData1, 1);

	// Call overloaded method
	WriteJoinBytes(device, data1Array, writeData2);
}

/// <summary>
/// Joins two byte values then writes them.
/// </summary>
/// <param name="device">Device to use.</param>
/// <param name="data1">First part of data to write.</param>
/// <param name="data2">Second part of data to write.</param>
void I2cExtensions::WriteJoinBytes(cx::I2cDevice^ device, const cx::Array<byte>^ writeData1, const cx::Array<byte>^ writeData2)
{
	// Join buffers
	auto addressLength = writeData1->Length;
	auto dataLength = writeData2->Length;
	auto buffer = ref new cx::Array<byte>(addressLength + dataLength);
	std::memmove(buffer->Data, writeData1->Data, addressLength);
	std::memmove(buffer->Data + addressLength, writeData2->Data, dataLength);

	// Call extended device method
	device->Write(buffer);
}

/// <summary>
/// Sets or clears one or more bits.
/// </summary>
/// <param name="device">Device to use.</param>
/// <param name="writeData">Data to write.</param>
/// <param name="mask">
/// Mask of the bit to set or clear according to value.
/// Supports setting or clearing multiple bits.
/// </param>
/// <param name="value">Value of the bits, i.e. set or clear.</param>
/// <returns>Value written.</returns>
/// <remarks>
/// Commonly used to set register flags.
/// Reads the current byte value, merges the positive or negative bit mask according to value,
/// then writes the modified byte back.
/// </remarks>
byte I2cExtensions::WriteReadWriteBit(cx::I2cDevice^ device, byte writeData, byte mask, bool value)
{
	// Read existing byte
	auto oldByte = WriteReadByte(device, writeData);

	// Merge bit (set or clear bit accordingly)
	auto newByte = value ? (byte)(oldByte | mask) : (byte)(oldByte & ~mask);

	// Write new byte
	WriteJoinByte(device, writeData, newByte);

	// Return the value written
	return newByte;
}

#pragma endregion