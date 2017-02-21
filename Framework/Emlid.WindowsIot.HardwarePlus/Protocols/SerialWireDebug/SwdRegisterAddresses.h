#pragma once

#include "pch.h"

namespace Emlid
{
	namespace WindowsIot
	{
		namespace HardwarePlus
		{
			namespace Protocols
			{
				namespace SerialWireDebug
				{
					/// <summary>
					/// ARM Serial Wire Debug (SWD) Debug Port (SW-DP) register addresses for read operations (different for write).
					/// </summary>
					public enum class SwdRegisterReadAddresses : byte
					{
						/// <summary>
						/// "DPIR" Debug Port Identification register.
						/// </summary>
						DebugPortId = 0x00,

						/// <summary>
						/// "CTRL/STAT" Control/Status register (0x0# = when bank 0 is selected).
						/// </summary>
						ControlStatus = 0x04,

						/// <summary>
						/// "DLCR" Data Link Control register (0x1# = when bank 1 is selected).
						/// </summary>
						DataLinkControl = 0x14,

						/// <summary>
						/// "TARGETID" Target Identification register (0x2# = when bank 2 is selected).
						/// </summary>
						TargetId = 0x24,

						/// <summary>
						/// "DLPIDR" Data Link Protocol Identification register (0x3# = when bank 3 is selected).
						/// </summary>
						DataLinkProtocolId = 0x34,

						/// <summary>
						/// "EVENTSTAT" Event Status register (0x4# = when bank 4 is selected).
						/// </summary>
						EventStatus = 0x44,

						/// <summary>
						/// "SELECT" register selects Access Port and Debug Port banks.
						/// </summary>
						Select = 0x08,

						/// <summary>
						/// "RDBUFF" Read Buffer register.
						/// </summary>
						ReadBuffer = 0x0c
					};

					/// <summary>
					/// ARM Serial Wire Debug (SWD) Debug Port (SW-DP) register addresses for write operations (different for read).
					/// </summary>
					public enum class SwdRegisterWriteAddresses : byte
					{
						/// <summary>
						/// "ABORT" Access Port abort register.
						/// </summary>
						Abort = 0x00,

						/// <summary>
						/// "CTRL/STAT" Control/Status register (0x0# = when bank 0 is selected).
						/// </summary>
						ControlStatus = 0x04,

						/// <summary>
						/// "DLCR" Data Link Control register (0x1# = when bank 1 is selected).
						/// </summary>
						DataLinkControl = 0x14,

						/// <summary>
						/// "RESEND" register.
						/// </summary>
						Resend = 0x08,

						/// <summary>
						/// "TARGETSEL" Target Selection register.
						/// </summary>
						TargetSelect = 0x0c
					};
				}
			}
		}
	}
}
