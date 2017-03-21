using System;

namespace Emlid.WindowsIot.Hardware.Components.Px4io.Data
{
    /// <summary>
    /// <see cref="Px4ioPage.Status"/> page register offsets.
    /// </summary>
    /// <see href="https://github.com/emlid/navio-rcio-linux-driver/blob/master/protocol.h"/>
    [CLSCompliant(false)]
    public enum Px4ioStatusRegisterOffset : byte
    {
        /// <summary>
        /// Free memory.
        /// </summary>
        FreeMemory = 0,

        /// <summary>
        /// CPU load.
        /// </summary>
        CpuLoad = 1,

        /// <summary>
        /// Monitoring flags.
        /// </summary>
        Monitoring = 2,

        /// <summary>
        /// Alarm flags.
        /// </summary>
        /// <remarks>
        /// Alarms latch, write 1 to a bit to clear it.
        /// </remarks>
        Alarms = 3,

        /// <summary>
        /// Battery voltage in mV.
        /// </summary>
        /// <remarks>
        /// Hardware version 1 only.
        /// </remarks>
        BatteryVoltage = 4,

        /// <summary>
        /// Battery current (raw ADC).
        /// </summary>
        /// <remarks>
        /// Hardware version 1 only.
        /// </remarks>
        BatteryCurrent = 5,

        /// <summary>
        /// Servo rail voltage in mV.
        /// </summary>
        /// <remarks>
        /// Hardware version 2 only.
        /// </remarks>
        ServoVoltage = 6,

        /// <summary>
        /// RSSI voltage.
        /// </summary>
        /// <remarks>
        /// Hardware version 2 only.
        /// </remarks>
        RssiVoltage = 7,

        /// <summary>
        /// RSSI PWM value.
        /// </summary>
        /// <remarks>
        /// Hardware version 2 only.
        /// </remarks>
        RssiPwm = 8,

        /// <summary>
        /// Mixer actuator limit flags.
        /// </summary>
        Mixer = 9
    }
}
