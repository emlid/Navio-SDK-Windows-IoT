using System;

namespace Emlid.WindowsIot.Hardware.Components.Px4io.Data
{
    /// <summary>
    /// <see cref="Px4ioPage.RCInput"/> page register data.
    /// </summary>
    /// <see href="https://github.com/emlid/navio-rcio-linux-driver/blob/master/protocol.h"/>
    [CLSCompliant(false)]
    public sealed class Px4ioRCInputRegisters
    {
        #region Constants

        /// <summary>
        /// Number of registers on this page.
        /// </summary>
        public const byte RegisterCount = 2;

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates an instance from register values.
        /// </summary>
        /// <param name="data">Register values read from the device.</param>
        public Px4ioRCInputRegisters(ushort[] data)
        {
            // Validate
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (data.Length < RegisterCount)
                throw new ArgumentOutOfRangeException(nameof(data));

            // Set properties from data
            Valid = data[0];
            var count = data.Length - 1;
            Controls = new ushort[count];
            Array.ConstrainedCopy(data, (int)Px4ioRCInputRegisterOffsets.ControlsStart, Controls, 0, count);
        }

        #endregion

        #region Public Fields

        /// <summary>
        /// Bitmask of valid controls.
        /// </summary>
        public ushort Valid;

        /// <summary>
        /// <see cref="Px4ioConfigRegisters.RCInputCount"/> controls from here.
        /// </summary>
        public ushort[] Controls;

        #endregion
    }
}
