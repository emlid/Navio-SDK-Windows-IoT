using System;

namespace Emlid.WindowsIot.Hardware.Components.Px4io.Data
{
    /// <summary>
    /// Defines the flags in the <see cref="Px4ioSetupRegisters.Features"/> register.
    /// </summary>
    /// <see href="https://github.com/emlid/navio-rcio-linux-driver/blob/master/protocol.h"/>
    [Flags]
    [CLSCompliant(false)]
    public enum Px4ioSetupFeatureFlags : ushort
    {
        /// <summary>
        /// Enable S.Bus v1 output.
        /// </summary>
        Sbus1Out = (1 << 0),

        /// <summary>
        /// Enable S.Bus v2 output.
        /// </summary>
        Sbus2Out = (1 << 1),

        /// <summary>
        /// Enable PWM RSSI parsing.
        /// </summary>
        PwmRssi = (1 << 2),

        /// <summary>
        /// Enable ADC RSSI parsing.
        /// </summary>
        AdcRssi = (1 << 3)
    }
}
