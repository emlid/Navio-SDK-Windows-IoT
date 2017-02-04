#include "pch.h"

int main(Array<String^>^ args)
{
	byte address = 0xec >> 1;
	auto device = I2cExtensions::Connect(0, address, I2cBusSpeed::FastMode, I2cSharingMode::Exclusive);
	auto id = device->DeviceId;
	auto settings = device->ConnectionSettings;
	delete device;
}
