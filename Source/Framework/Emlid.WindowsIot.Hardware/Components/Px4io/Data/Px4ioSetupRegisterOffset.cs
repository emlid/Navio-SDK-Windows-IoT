using System;

namespace Emlid.WindowsIot.Hardware.Components.Px4io.Data
{
    /// <summary>
    /// <see cref="Px4ioPage.Setup"/> page register offsets.
    /// </summary>
    /// <see href="https://github.com/emlid/navio-rcio-linux-driver/blob/master/protocol.h"/>
    /// <remarks>
    /// Offset 8 is undefined.
    /// </remarks>
    [CLSCompliant(false)]
    public enum Px4ioSetupRegisterOffset : byte
    {
        /// <summary>
        /// Features.
        /// </summary>
        Features = 0,

        /// <summary>
        /// Arming controls.
        /// </summary>
        Arming = 1,

        /// <summary>
        /// Bitmask, 0 = low rate, 1 = high rate.
        /// </summary>
        PwmRates = 2,

        /// <summary>
        /// Low PWM frame output rate in Hz.
        /// </summary>
        PwmDefaultRate = 3,

        /// <summary>
        /// High PWM frame output rate in Hz.
        /// </summary>
        PwmAlternateRate = 4,

        /// <summary>
        /// Bitmask of relay/switch outputs, 0 = off, 1 = on.
        /// </summary>
        /// <remarks>
        /// Hardware version 1 only.
        /// </remarks>
        Relays = 5,

        /// <summary>
        /// Bitmask of relay/switch outputs, 0 = off, 1 = on.
        /// </summary>
        /// <remarks>
        /// Hardware version 2 only.
        /// </remarks>
        RelaysPad = 5,

        /// <summary>
        /// Battery voltage correction factor (float).
        /// </summary>
        /// <remarks>
        /// Hardware version 1 only.
        /// </remarks>
        BatteryVoltageScale = 6,

        /// <summary>
        /// Servo voltage correction factor (float).
        /// </summary>
        /// <remarks>
        /// Hardware version 2 only.
        /// </remarks>
        ServoVoltageScale = 6,

        /// <summary>
        /// DSM bind state.
        /// </summary>
        DsmBindState = 7,

        /// <summary>
        /// Debug level for IO board.
        /// </summary>
        Debug = 9,

        /// <summary>
        /// Reboot IO into boot-loader.
        /// </summary>
        RebootBootLoader = 10,

        /// <summary>
        /// Get CRC of IO firmware.
        /// </summary>
        /// <remarks>
        /// Storage space of <see cref="ForceSafetyOff"/> occupied by CRC.
        /// </remarks>
        Crc1 = 11,

        /// <summary>
        /// Second part of CRC.
        /// </summary>
        /// <remarks>
        /// Read-only, space shared with <see cref="ForceSafetyOff"/>:
        /// </remarks>
        Crc2 = 12,

        /// <summary>
        /// Force safety switch into 'armed' (PWM enabled) state.
        /// </summary>
        /// <remarks>
        /// Write-only.
        /// Read space taken by <see cref="Crc2"/>.
        /// </remarks>
        ForceSafetyOff = 12,

        /// <summary>
        /// Throttle failsafe pulse length in microseconds.
        /// </summary>
        RCThrottleFailsafeUs = 13,

        /// <summary>
        /// Force safety switch into 'disarmed' (PWM disabled state).
        /// </summary>
        ForceSafetyOn = 14
    }
}
