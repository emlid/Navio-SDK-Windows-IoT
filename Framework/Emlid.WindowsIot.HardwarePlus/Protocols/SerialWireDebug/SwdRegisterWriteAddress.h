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
					/// ARM Serial Wire Debug (SWD) Debug Port (SW-DP) register addresses for write operations (different for read).
					/// </summary>
					public enum class SwdRegisterWriteAddress : byte
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
