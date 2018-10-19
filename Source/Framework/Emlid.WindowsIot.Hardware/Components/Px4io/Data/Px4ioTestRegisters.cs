using System;

namespace Emlid.WindowsIot.Hardware.Components.Px4io.Data
{
    /// <summary>
    /// <see cref="Px4ioPage.Test"/> page register data.
    /// </summary>
    /// <see href="https://github.com/emlid/navio-rcio-linux-driver/blob/master/protocol.h"/>
    [CLSCompliant(false)]
    public sealed class Px4ioTestRegisters
    {
        #region Constants

        /// <summary>
        /// Number of registers on this page.
        /// </summary>
        public const byte RegisterCount = 1;

        #endregion Constants

        #region Lifetime

        /// <summary>
        /// Creates an instance from register values.
        /// </summary>
        /// <param name="data">Register values read from the device.</param>
        public Px4ioTestRegisters(ushort[] data)
        {
            // Validate
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (data.Length < RegisterCount)
                throw new ArgumentOutOfRangeException(nameof(data));

            // Set properties from data
            Led = data[0];
        }

        #endregion Lifetime

        #region Public Fields

        /// <summary>
        /// Sets the amber LED on/off.
        /// </summary>
        public ushort Led { get; set; }

        #endregion Public Fields
    }
}