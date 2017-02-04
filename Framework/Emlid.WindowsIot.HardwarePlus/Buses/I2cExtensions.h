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
				/// Extensions for work with I2C devices.
				/// </summary>
				public ref class I2cExtensions sealed
				{

				private:

					I2cExtensions();	// Abstract class

				public:

					#pragma region Constants

					/// <summary>
					/// Maximum transfer size for I2C requests on Windows IoT or Raspberry Pi 2.
					/// This is a confirmed soft limitation by Microsoft, it should be 64K.
					/// </summary>
					/// <seealso href="https://social.msdn.microsoft.com/Forums/en-US/e938900f-b732-41dc-95f6-058a39dac31d/i2c-transfer-limit-of-16384-bytes-on-raspberry-pi-2?forum=WindowsIoT"/>
					static property int MaximumTransferSize { int get() { return 16384; } }

					#pragma endregion

					#pragma region Connect

					/// <summary>
					/// Connects to an I2C device if it exists.
					/// </summary>
					/// <param name="busNumber">Bus controller number, zero based.</param>
					/// <param name="address">7-bit I2C slave address (8 bit addresses must be shifted down to exclude the read/write bit).</param>
					/// <param name="speed">Bus speed.</param>
					/// <param name="sharingMode">Sharing mode.</param>
					/// <returns>Device when the bus controller and device exist, otherwise null.</returns>
					static cx::I2cDevice^ Connect(int busNumber, int address, cx::I2cBusSpeed speed, cx::I2cSharingMode sharingMode);

					#pragma endregion

					#pragma region Read

					/// <summary>
					/// Reads one data byte.
					/// </summary>
					/// <param name="device">Device to use.</param>
					/// <returns>Read data byte.</returns>
					static byte ReadByte(cx::I2cDevice^ device);

					/// <summary>
					/// Reads one or more data bytes.
					/// </summary>
					/// <param name="device">Device to use.</param>
					/// <param name="size">Amount of data to read.</param>
					/// <returns>Read data bytes.</returns>
					static cx::Array<byte>^ ReadBytes(cx::I2cDevice^ device, int size);

					/// <summary>
					/// Writes data then reads a single byte result.
					/// </summary>
					/// <param name="device">Device to use.</param>
					/// <param name="writeData">Data to write.</param>
					/// <returns>Read data byte.</returns>
					[cx::DefaultOverload]
					static byte WriteReadByte(cx::I2cDevice^ device, byte writeData);

					/// <summary>
					/// Writes data then reads a single byte result.
					/// </summary>
					/// <param name="device">Device to use.</param>
					/// <param name="writeData">Data to write.</param>
					/// <returns>Read data byte.</returns>
					static byte WriteReadByte(cx::I2cDevice^ device, const cx::Array<byte>^ writeData);

					/// <summary>
					/// Writes data then reads a single byte result.
					/// </summary>
					/// <param name="device">Device to use.</param>
					/// <param name="writeData">Data to write.</param>
					/// <param name="size">Amount of data to read.</param>
					/// <returns>Read data bytes.</returns>
					[cx::DefaultOverload]
					static cx::Array<byte>^ WriteReadBytes(cx::I2cDevice^ device, byte writeData, int size);

					/// <summary>
					/// Writes data then reads one or more bytes.
					/// </summary>
					/// <param name="device">Device to use.</param>
					/// <param name="writeData">Data to write.</param>
					/// <param name="size">Amount of data to read.</param>
					/// <returns>Read data bytes.</returns>
					static cx::Array<byte>^ WriteReadBytes(cx::I2cDevice^ device, const cx::Array<byte>^ writeData, int size);

					/// <summary>
					/// Writes data, reads a byte result then tests on or more bits.
					/// </summary>
					/// <remarks>
					/// Commonly used to test register flags.
					/// </remarks>
					/// <param name="device">Device to use.</param>
					/// <param name="writeData">Data to write.</param>
					/// <param name="mask">Index of the bit to read, zero based.</param>
					/// <returns>True when the result was positive (any bits in the mask were set).</returns>
					[cx::DefaultOverload]
					static bool WriteReadBit(cx::I2cDevice^ device, byte writeData, byte mask);

					/// <summary>
					/// Writes data, reads a byte result then tests on or more bits.
					/// </summary>
					/// <remarks>
					/// Commonly used to test register flags.
					/// </remarks>
					/// <param name="device">Device to use.</param>
					/// <param name="writeData">Data to write.</param>
					/// <param name="mask">Index of the bit to read, zero based.</param>
					/// <returns>True when the result was positive (any bits in the mask were set).</returns>
					static bool WriteReadBit(cx::I2cDevice^ device, const cx::Array<byte>^ writeData, byte mask);

					#pragma endregion

					#pragma region Write

					/// <summary>
					/// Writes one data byte.
					/// </summary>
					/// <param name="device">Device to use.</param>
					/// <param name="data">Data to write.</param>
					static void WriteByte(cx::I2cDevice^ device, byte data);

					/// <summary>
					/// Writes one or more data bytes.
					/// </summary>
					/// <param name="device">Device to use.</param>
					/// <param name="data">Data to write.</param>
					static void WriteBytes(cx::I2cDevice^ device, const cx::Array<byte>^ data);

					/// <summary>
					/// Joins two byte values then writes them.
					/// </summary>
					/// <param name="device">Device to use.</param>
					/// <param name="data1">First part of data to write.</param>
					/// <param name="data2">Second part of data to write.</param>
					[cx::DefaultOverload]
					static void WriteJoinByte(cx::I2cDevice^ device, byte data1, byte data2);

					/// <summary>
					/// Joins two byte values then writes them.
					/// </summary>
					/// <param name="device">Device to use.</param>
					/// <param name="data1">First part of data to write.</param>
					/// <param name="data2">Second part of data to write.</param>
					static void WriteJoinByte(cx::I2cDevice^ device, const cx::Array<byte>^ data1, byte data2);

					/// <summary>
					/// Joins two byte values then writes them.
					/// </summary>
					/// <param name="device">Device to use.</param>
					/// <param name="data1">First part of data to write.</param>
					/// <param name="data2">Second part of data to write.</param>
					[cx::DefaultOverload]
					static void WriteJoinBytes(cx::I2cDevice^ device, byte data1, const cx::Array<byte>^ data2);

					/// <summary>
					/// Joins two byte values then writes them.
					/// </summary>
					/// <param name="device">Device to use.</param>
					/// <param name="data1">First part of data to write.</param>
					/// <param name="data2">Second part of data to write.</param>
					static void WriteJoinBytes(cx::I2cDevice^ device, const cx::Array<byte>^ data1, const cx::Array<byte>^ data2);

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
					static byte WriteReadWriteBit(cx::I2cDevice^ device, byte writeData, byte mask, bool value);

					#pragma endregion
				};
			}
		}
	}
}
