using System;

namespace Emlid.WindowsIot.Hardware.Components.Px4io.Data
{
    /// <summary>
    /// Defines the flags in the <see cref="Px4ioStatusRegisters.Monitoring"/> register.
    /// </summary>
    /// <see href="https://github.com/emlid/navio-rcio-linux-driver/blob/master/protocol.h"/>
    [Flags]
    [CLSCompliant(false)]
    public enum Px4ioStatusMonitoringFlags : ushort
    {
        /// <summary>
        /// Armed okay and locally armed.
        /// </summary>
        Armed = (1 << 0),

        /// <summary>
        /// Manual override.
        /// </summary>
        ManualOverride = (1 << 1),

        /// <summary>
        /// RC input is valid.
        /// </summary>
        RCInput = (1 << 2),

        /// <summary>
        /// PPM input is valid.
        /// </summary>
        Ppm = (1 << 3),

        /// <summary>
        /// DSM input is valid.
        /// </summary>
        Dsm = (1 << 4),

        /// <summary>
        /// SBUS input is valid.
        /// </summary>
        Sbus = (1 << 5),

        /// <summary>
        /// Controls from FMU are valid.
        /// </summary>
        Fmu = (1 << 6),

        /// <summary>
        /// Raw PWM from FMU is bypassing the mixer.
        /// </summary>
        PwmRaw = (1 << 7),

        /// <summary>
        /// Mixer is OK.
        /// </summary>
        Mixer = (1 << 8),

        /// <summary>
        /// Arming state between IO and FMU is in sync.
        /// </summary>
        ArmedSync = (1 << 9),

        /// <summary>
        /// Initialization of the IO completed without error.
        /// </summary>
        IoInititalized = (1 << 10),

        /// <summary>
        /// Failsafe is active.
        /// </summary>
        Failsafe = (1 << 11),

        /// <summary>
        /// Safety is off.
        /// </summary>
        SafetyOff = (1 << 12),

        /// <summary>
        /// FMU was initialized and OK once.
        /// </summary>
        FmuInitialized = (1 << 13),

        /// <summary>
        /// ST24 input is valid.
        /// </summary>
        St24 = (1 << 14),

        /// <summary>
        /// SUMD input is valid.
        /// </summary>
        Sumd = (1 << 15)
    }
}
