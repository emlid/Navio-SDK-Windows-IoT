using System;

namespace Emlid.WindowsIot.Hardware.Components.Px4io.Data
{
    /// <summary>
    /// Defines the flags in the <see cref="Px4ioSetupRegisters.Arming"/> register.
    /// </summary>
    /// <see href="https://github.com/emlid/navio-rcio-linux-driver/blob/master/protocol.h"/>
    [Flags]
    [CLSCompliant(false)]
    public enum Px4ioSetupArmingFlags : ushort
    {
        /// <summary>
        /// OK to arm the IO side.
        /// </summary>
        IOArmOk = (1 << 0),

        /// <summary>
        /// FMU is already armed.
        /// </summary>
        FmuArmed = (1 << 1),

        /// <summary>
        /// OK to switch to manual override via override RC channel.
        /// </summary>
        ManualOverrideOk = (1 << 2),

        /// <summary>
        /// Use custom failsafe values, not 0 values of mixer.
        /// </summary>
        FailsafeCustom = (1 << 3),

        /// <summary>
        /// OK to try in-air restart.
        /// </summary>
        InAirRestartOk = (1 << 4),

        /// <summary>
        /// Output of PWM right after startup enabled to help ESCs initialize and prevent them from beeping.
        /// </summary>
        PwmEnabledAtStartup = (1 << 5),

        /// <summary>
        /// Disable the IO-internal evaluation of the RC.
        /// </summary>
        RCHandlingDisabled = (1 << 6),

        /// <summary>
        /// If set, the system operates normally, but won't actuate any servos.
        /// </summary>
        ServosDisabled = (1 << 7),

        /// <summary>
        /// If set, the system will always output the failsafe values.
        /// </summary>
        ForceFailsafe = (1 << 8),

        /// <summary>
        /// If set, the system will never return from a failsafe, but remain in failsafe once triggered.
        /// </summary>
        FailsafePermanent = (1 << 9),

        /// <summary>
        /// If set then on FMU failure override is immediate. Otherwise it waits for the mode switch to go past the override threshold.
        /// </summary>
        OverrideImmediate = (1 << 10)
    }
}
