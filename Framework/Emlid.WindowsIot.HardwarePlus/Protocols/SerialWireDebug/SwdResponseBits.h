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
					/// ARM Serial Wire Debug (SWD) protocol acknowledgement response bits.
					/// </summary>
					[cx::Flags]
					public enum class SwdResponseBits : byte
					{
						/// <summary>
						/// Okay.
						/// </summary>
						Ok = 0x01,

						/// <summary>
						/// Wait.
						/// </summary>
						Wait = 0x02,

						/// <summary>
						/// Fault.
						/// </summary>
						Fault = 0x04
					};
				}
			}
		}
	}
}