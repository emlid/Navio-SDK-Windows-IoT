using System;

namespace Emlid.WindowsIot.Hardware.Components.Px4io.Data
{
    /// <summary>
    /// <see cref="Px4ioPage.Sensors"/> page register offsets.
    /// </summary>
    /// <see href="https://github.com/emlid/navio-rcio-linux-driver/blob/master/protocol.h"/>
    [CLSCompliant(false)]
    public enum Px4ioSensorRegisterOffset : byte
    {
        /// <summary>
        /// Altitude of an external sensor (HoTT or SBUS2).
        /// </summary>
        Altitude = 0
    }
}
