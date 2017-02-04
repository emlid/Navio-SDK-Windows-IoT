using System;

namespace Emlid.WindowsIot.Hardware.Components.Px4io.Data
{
    /// <summary>
    /// <see cref="Px4ioPage.Controls"/> page register offsets.
    /// </summary>
    /// <see href="https://github.com/emlid/navio-rcio-linux-driver/blob/master/protocol.h"/>
    [CLSCompliant(false)]
    public enum Px4ioControlRegisterOffsets : byte
    {
        /// <summary>
        /// Control group 0.
        /// </summary>
        Group0 = (Px4ioControlRegisters.ControlsMaximum * 0),

        /// <summary>
        /// Control group 1.
        /// </summary>
        Group1 = (Px4ioControlRegisters.ControlsMaximum * 1),

        /// <summary>
        /// Control group 2.
        /// </summary>
        Group2 = (Px4ioControlRegisters.ControlsMaximum * 2),

        /// <summary>
        /// Control group 3.
        /// </summary>
        Group3 = (Px4ioControlRegisters.ControlsMaximum * 3),

        /// <summary>
        /// Group validation flags.
        /// </summary>
        GroupsValid = 64
    }
}
