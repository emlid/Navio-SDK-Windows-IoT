#pragma once

// ----------------------------------------------------------------------------
// Disable Visual Studio Code Analysis warnings for external include files
#include <codeanalysis\warnings.h>
#pragma warning(push)
#pragma warning(disable : ALL_CODE_ANALYSIS_WARNINGS)
// ----------------------------------------------------------------------------

// C++/CX headers
#include <collection.h>
#include <ppltasks.h>
#include <agile.h>

// ----------------------------------------------------------------------------
// Resume Visual Studio Code Analysis warnings after external includes
#pragma warning(pop)
// ----------------------------------------------------------------------------

// Driver
#include <initguid.h>
#include <NavioRCInputDriver.h>

// C++/CX namespaces (isolated for side-by-side use)
using namespace Platform;
using namespace Emlid::WindowsIot::HardwarePlus;
using namespace Emlid::WindowsIot::HardwarePlus::Buses;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::Devices::I2c;
using namespace Windows::Foundation::Metadata;
