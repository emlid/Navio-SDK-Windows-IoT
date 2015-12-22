/*++

Module Name:

    driver.h

Abstract:

    This file contains the driver definitions.

Environment:

    Kernel-mode Driver Framework

--*/

#define INITGUID

#include <ntddk.h>
#include <wdf.h>
#include <gpio.h>
#define RESHUB_USE_HELPER_ROUTINES
#include <reshub.h>

#include "Device.h"
#include "Queue.h"
#include "Trace.h"

EXTERN_C_START

//
// WDFDRIVER Events
//

DRIVER_INITIALIZE DriverEntry;
EVT_WDF_DRIVER_DEVICE_ADD OnDeviceAdd;
EVT_WDF_OBJECT_CONTEXT_CLEANUP OnContextCleanup;

EXTERN_C_END
