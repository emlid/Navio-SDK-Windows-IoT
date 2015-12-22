using System;

namespace Emlid.WindowsIot.Hardware
{
    /// <summary>
    /// Bitmask for the <see cref="NxpPca9685Register.Mode1"/> register.
    /// </summary>
    [Flags]
    public enum NxpPca9685Mode1Bits : byte
    {
        /// <summary>
        /// No bits set.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Enables or disables the <see cref="NxpPca9685Register.AllCall"/>.
        /// </summary>
        AllCall = 0x01,

        /// <summary>
        /// Enables or disables the <see cref="NxpPca9685Register.SubAddress3"/>.
        /// </summary>
        SubAddress3 = 0x02,

        /// <summary>
        /// Enables or disables the <see cref="NxpPca9685Register.SubAddress2"/>
        /// </summary>
        SubAddress2 = 0x04,

        /// <summary>
        /// Enables or disables the <see cref="NxpPca9685Register.SubAddress1"/>
        /// </summary>
        SubAddress1 = 0x08,

        /// <summary>
        /// Enables or disables low-power mode (oscillator off).
        /// </summary>
        /// <remarks>
        /// <para>
        /// It takes 500 µs maximum for the oscillator to be up and running once sleep bit has been set to 0. Timings on LED# outputs are not
        /// guaranteed if PWM control registers are accessed within the 500 µs window. There is no start-up delay required when using the
        /// external clock pin as the PWM clock.
        /// </para>
        /// <para>
        /// No PWM control is possible when the oscillator is off.
        /// </para>
        /// <para>
        /// When the oscillator is off (sleep mode) the LED# outputs cannot be turned on, off or dimmed/blinked.
        /// </para>
        /// </remarks>
        Sleep = 0x10,

        /// <summary>
        /// When the Auto Increment flag is set the control register is automatically incremented
        /// after a read or write. This allows the user to program the registers sequentially.
        /// </summary>
        AutoIncrement = 0x20,

        /// <summary>
        /// Enables or disables the <see cref="NxpPca9685Register.SubAddress2"/>
        /// </summary>
        /// <remarks>
        /// <para>
        /// To use the external clock pin, this bit must be set by the following sequence:
        /// </para>
        /// <list type="number">
        /// <item>
        /// Set the sleep bit in mode 1. This turns off the internal oscillator.
        /// </item>
        /// <item>
        /// Set both the sleep and external clock bits in mode 1.The switch is now made.
        /// The external clock can be active during the switch because the sleep bit is set.
        /// </item>
        /// </list>
        /// <para>
        /// This bit is a "sticky bit", that is, it cannot be cleared by writing a 0 to it. The
        /// external clock bit can only be cleared by a power cycle or software reset.
        /// External clock range is DC to 50 MHz. The frequency is calculated as:
        /// <code>refresh rate = external clock / (4096 * (prescale + 1))</code>
        /// </para>
        /// </remarks>
        ExternalClock = 0x40,

        /// <summary>
        /// Enables or disables the <see cref="NxpPca9685Register.SubAddress2"/>
        /// </summary>
        /// <remarks>
        /// Reading shows the state of restart logic.
        /// Write 1 to this bit to clear it. Writing 0 has no effect.
        /// </remarks>
        Restart = 0x80
    }
}
