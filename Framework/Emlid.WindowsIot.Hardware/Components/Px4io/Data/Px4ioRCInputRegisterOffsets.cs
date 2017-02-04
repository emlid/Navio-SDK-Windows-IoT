using System;

namespace Emlid.WindowsIot.Hardware.Components.Px4io.Data
{
    /// <summary>
    /// <see cref="Px4ioPage.RCInput"/> page register offsets.
    /// </summary>
    /// <see href="https://github.com/emlid/navio-rcio-linux-driver/blob/master/protocol.h"/>
    [CLSCompliant(false)]
    public enum Px4ioRCInputRegisterOffsets : byte
    {
        /// <summary>
        /// Bitmask of valid controls.
        /// </summary>
        Valid = 0,

        /// <summary>
        /// <see cref="Px4ioConfigRegisters.RCInputCount"/> controls from here.
        /// </summary>
        ControlsStart = 1
    }
}
