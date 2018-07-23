using System;

namespace Emlid.WindowsIot.Hardware.Components.Px4io.Data
{
    /// <summary>
    /// <see cref="Px4ioPage.Test"/> page register offsets.
    /// </summary>
    /// <see href="https://github.com/emlid/navio-rcio-linux-driver/blob/master/protocol.h"/>
    [CLSCompliant(false)]
    public enum Px4ioTestRegisterOffset : byte
    {
        /// <summary>
        /// Sets the amber LED on/off.
        /// </summary>
        Led = 0
    }
}
