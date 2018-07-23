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
					/// ARM Serial Wire Debug (SWD) Debug Port Identification register bit definitions.
					/// </summary>
					[cx::Flags]
					public enum class SwdDebugPortIdRegisterBits : uint32
					{
						/// <summary>
						/// [0] RAO.
						/// </summary>
						Rao = 0x00000001,

						/// <summary>
						/// [1:11] Designer/manufacturer, set to 010001110111 for ARM.
						/// </summary>
						Designer = 0x00000ffe,

						/// <summary>
						/// [15:12] Architecture version.
						/// </summary>
						Version = 0x0000f000,

						/// <summary>
						/// [16] Minimal functions implemented (MINDP).
						/// </summary>
						/// <remarks>
						/// When set, transaction counter, pushed-verify and pushed-find operations are not supported.
						/// </remarks>
						Minimal = 0x00010000,

						/// <summary>
						/// [17:19] Reserved.
						/// </summary>
						Reserved = 0x000e0000,

						/// <summary>
						/// [20:27] Part number.
						/// </summary>
						PartNumber = 0x0ff00000,

						/// <summary>
						/// [28:31] Revision.
						/// </summary>
						Revision = 0xf0000000
					};
				}
			}
		}
	}
}
