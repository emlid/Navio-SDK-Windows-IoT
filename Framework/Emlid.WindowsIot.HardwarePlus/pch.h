#pragma once

#pragma comment(lib, "windowsapp")

#include "winrt\base.h"
#include "winrt\ppl.h"
#include "winrt\Windows.Foundation.h"
#include "winrt\Windows.Foundation.Collections.h"
#include "winrt\Windows.Foundation.Metadata.h"
#include "winrt\Windows.Devices.h"
#include "winrt\Windows.Devices.Enumeration.h"
#include "winrt\Windows.Devices.Gpio.h"
#include "winrt\Windows.Devices.I2c.h"
#include "winrt\Windows.Devices.Spi.h"

#include <collection.h>
//#include <experimental\resumable>
//#include <ppltasks.h>
//#include <pplawait.h>
#define _CRT_SECURE_DEPRECATE_MEMORY
#include <tchar.h>
//#include <stdio.h>
//#include <iostream>
#include <memory>
//#include <sstream>

namespace cx
{
	using namespace Platform;
	using namespace Windows::Foundation;
	using namespace Windows::Foundation::Collections;
	using namespace Windows::Foundation::Metadata;
	using namespace Windows::Devices::Enumeration;
	using namespace Windows::Devices::I2c;
	using namespace Windows::Devices::Spi;
	using namespace Windows::Devices::Gpio;
}

namespace winrt
{
	using namespace winrt::Windows::Foundation;
	using namespace winrt::Windows::Foundation::Metadata;
	using namespace winrt::Windows::Devices::Enumeration;
	using namespace winrt::Windows::Devices::I2c;
	using namespace winrt::Windows::Devices::Spi;
	using namespace winrt::Windows::Devices::Gpio;
}