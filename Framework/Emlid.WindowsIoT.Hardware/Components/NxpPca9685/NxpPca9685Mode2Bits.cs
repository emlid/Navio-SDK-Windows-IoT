using System;

namespace Emlid.WindowsIot.Hardware.Components.NxpPca9685
{
    /// <summary>
    /// Bitmask for the <see cref="NxpPca9685Register.Mode1"/> register.
    /// </summary>
    [Flags]
    public enum NxpPca9685Mode2Bits : byte
    {
        /// <summary>
        /// No bits set.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Active low output enable (OUTNE) bit 0.
        /// </summary>
        OutputActiveLow0 = 0x01,

        /// <summary>
        /// Active low output enable (OUTNE) bit 1.
        /// </summary>
        OutputActiveLow1 = 0x02,

        /// <summary>
        /// Selects the LED drive mode.
        /// </summary>
        /// <remarks>
        /// When set, the 16 LED# outputs are configured with a totem pole structure.
        /// When cleared, the 16 LED# outputs are configured with an open-drain structure.
        /// Power on default is set.
        /// </remarks>
        OutputDrive = 0x04,

        /// <summary>
        /// Selects the output change mode.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When set, outputs change on ACK. Update on ACK requires all 4 PWM channel registers
        /// to be loaded before outputs will change on the last ACK.
        /// </para>
        /// <para>
        /// When cleared, outputs change on STOP. Change of the outputs at the STOP command allows
        /// synchronizing outputs of more than one PCA9685. Applicable to registers from
        /// 06h (LED0_ON_L) to 45h (LED15_OFF_H) only. 1 or more registers can be written,
        /// in any order, before STOP.
        /// </para>
        /// <para>
        /// Power on default is cleared.
        /// </para>
        /// </remarks>
        OutputChange = 0x08,

        /// <summary>
        /// Inverts output when OE set.
        /// Sets the value to use when an external driver is used.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Normal LEDs can be driven directly in either mode. Some newer LEDs include integrated
        /// Zener diodes to limit voltage transients, reduce EMI, protect the LEDs and these must be
        /// driven only in the open-drain mode to prevent overheating the IC.
        /// </para>
        /// <para>
        /// Power on reset default state of LED# output pins is LOW and this option is not enabled.
        /// </para>
        /// </remarks>
        OutputInvert = 0x10
    }
}
