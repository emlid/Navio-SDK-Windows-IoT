using System;
using System.Diagnostics.CodeAnalysis;

namespace Emlid.WindowsIot.Hardware.Components.Px4io.Data
{
    /// <summary>
    /// Defines the flags in the <see cref="Px4ioStatusRegisters.Alarms"/> register.
    /// </summary>
    /// <see href="https://github.com/emlid/navio-rcio-linux-driver/blob/master/protocol.h"/>
    [Flags]
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Design", "CA1028", Justification = "Non-integer enumeration better suited to match the hardware specification in a hardware library.")]
    public enum Px4ioStatusAlarmFlags : ushort
    {
        /// <summary>
        /// Battery voltage is very close to regulator dropout.
        /// </summary>
        /// <remarks>
        /// Hardware version 1 only.
        /// </remarks>
        BatteryLow = (1 << 0),

        /// <summary>
        /// Board temperature is high.
        /// </summary>
        TemperatureHigh = (1 << 1),

        /// <summary>
        /// Servo current limit was exceeded.
        /// </summary>
        /// <remarks>
        /// Hardware version 1 only.
        /// </remarks>
        ServoCurrent = (1 << 2),

        /// <summary>
        /// Accessory current limit was exceeded.
        /// </summary>
        /// <remarks>
        /// Hardware version 1 only.
        /// </remarks>
        AccessoryCurrent = (1 << 3),

        /// <summary>
        /// Timed-out waiting for controls from FMU.
        /// </summary>
        FmuTimeout = (1 << 4),

        /// <summary>
        /// Timed out waiting for RC input.
        /// </summary>
        RCInputLost = (1 << 5),

        /// <summary>
        /// PWM configuration or output was bad.
        /// </summary>
        PwmError = (1 << 6),

        /// <summary>
        /// Servo voltage was out of the valid range (2.5 - 5.5 V).
        /// </summary>
        /// <remarks>
        /// Hardware version 2 only.
        /// </remarks>
        ServoVoltage = (1 << 7)
    }
}