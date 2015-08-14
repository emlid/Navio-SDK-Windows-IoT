namespace Emlid.WindowsIoT.Hardware
{
    /// <summary>
    /// Supported RC input modes of the <see cref="NavioRCInputDevice"/>.
    /// </summary>
    public enum NavioRCInputMode
    {
        /// <summary>
        /// PPM analog input mode.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A complete PPM frame is about 22.5 ms (can vary between manufacturer). 
        /// Signal low state is always 0.3 ms. 
        /// It begins with a start frame (state high for more than 2 ms). 
        /// Each channel (up to 8) is encoded by the time of the high state 
        /// (PPM high state + 0.3 x (PPM low state) = servo PWM pulse width).
        /// </para>
        /// See https://en.wikipedia.org/wiki/Pulse-position_modulation for more information.
        /// </remarks>
        PPM,

        /// <summary>
        /// SBUS digital input mode.
        /// </summary>
        SBUS
    }
}
