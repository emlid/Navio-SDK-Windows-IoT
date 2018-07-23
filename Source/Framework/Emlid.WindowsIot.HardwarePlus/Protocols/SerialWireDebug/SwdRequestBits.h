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
					/// ARM Serial Wire Debug (SWD) protocol packet request bits.
					/// </summary>
					[cx::Flags]
					public enum class SwdRequestBits : byte
					{
						/// <summary>
						/// Start bit, always 1.
						/// </summary>
						Start = 0x01,

						/// <summary>
						/// "APnDP" bit, selecting an "Access Port" register access "APACC" when 1
						/// or a "Debug Port" register access "DPACC" when 0.
						/// </summary>
						AccessOrDebug = 0x02,

						/// <summary>
						/// "RnW" bit, specifying a read operation when 1 or a write when 0.
						/// </summary>
						ReadOrWrite = 0x04,

						/// <summary>
						/// Register address bit 2 (zero based).
						/// </summary>
						Address2 = 0x08,

						/// <summary>
						/// Register address bit 3 (zero based).
						/// </summary>
						Address3 = 0x10,

						/// <summary>
						/// Masks which extracts the address bits 2 and 3 (zero based) from the request.
						/// </summary>
						/// <remarks>
						/// Bits 0 and 1 are always zero. Mask then shift down one to get the address (address bit 2 starts at request bit 3).
						/// </remarks>
						AddressBits = 0x18,

						/// <summary>
						/// Parity bit, the even partiy calculated from the <see cref="ParitySourceBits" />.
						/// </summary>
						Parity = 0x20,

						/// <summary>
						/// Parity calculation source bits.
						/// </summary>
						ParitySourceBits = (AccessOrDebug & ReadOrWrite & AddressBits),

						/// <summary>
						/// Stop bit, always zero.
						/// </summary>
						Stop = 0x40,

						/// <summary>
						/// Park helps with turnaround, set high because the pull-up resistor on SWD is weak.
						/// </summary>
						Park = 0x80
					};
				}
			}
		}
	}
}