/*++

Module Name:

	NavioRCInputDriver.h

Abstract:

	This module contains the common declarations shared by driver
	and user applications.

Environment:

	User and kernel.

--*/

// Include only once
#ifndef _NAVIORCINPUT_PUBLIC_H_
#define _NAVIORCINPUT_PUBLIC_H_

// Device Interface ID for appliation access
DEFINE_GUID (GUID_DEVINTERFACE_NavioRCInput,
	0x9d3b9fcf,0x65e4,0x4736,0xb4,0x33,0x15,0x29,0xa5,0x90,0xa3,0x80);
// {9d3b9fcf-65e4-4736-b433-1529a590a380}


// PWM Cycle
typedef struct _NAVIO_PWM_CYCLE
{
	LARGE_INTEGER LowTimestamp;
	LARGE_INTEGER LowLength;
	LARGE_INTEGER HighTimestamp;
	LARGE_INTEGER HighLength;
} NAVIO_PWM_CYCLE;

#endif // __NAVIORCINPUT_PUBLIC_H_
