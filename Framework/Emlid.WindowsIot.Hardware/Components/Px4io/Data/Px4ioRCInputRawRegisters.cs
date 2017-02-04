using System;

namespace Emlid.WindowsIot.Hardware.Components.Px4io.Data
{
    /// <summary>
    /// <see cref="Px4ioPage.RCInputRaw"/> page register data.
    /// </summary>
    /// <see href="https://github.com/emlid/navio-rcio-linux-driver/blob/master/protocol.h"/>
    [CLSCompliant(false)]
    public sealed class Px4ioRCInputRawRegisters
    {
        #region Constants

        /// <summary>
        /// Number of registers on this page.
        /// </summary>
        public const byte RegisterCount = 7;

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates an instance from register values.
        /// </summary>
        /// <param name="data">Register values read from the device.</param>
        public Px4ioRCInputRawRegisters(ushort[] data)
        {
            // Validate
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            var count = data[0];
            if (data.Length < RegisterCount + count - 1)
                throw new ArgumentOutOfRangeException(nameof(data));

            // Set properties from data
            ChannelCount = data[0];
            Status = (Px4ioRCInputRawStatusFlags)data[1];
            RssiNormal = data[2];
            Data = data[3];
            FrameCounter = data[4];
            FrameLostCounter = data[5];
            Channels = new ushort[count];
            Array.ConstrainedCopy(data, (int)Px4ioRCInputRawRegisterOffsets.ChannelsStart, Channels, 0, count);
        }

        #endregion

        #region Public Fields

        /// <summary>
        /// Number of valid channels.
        /// </summary>
        public ushort ChannelCount;

        /// <summary>
        /// RC detail status flags.
        /// </summary>
        public Px4ioRCInputRawStatusFlags Status;

        /// <summary>
        /// Normalized RSSI value, 0: no reception, 255: perfect reception.
        /// </summary>
        /// <remarks>
        /// Hardware version 2 only.
        /// </remarks>
        public ushort RssiNormal;

        /// <summary>
        /// Details about the RC source (PPM frame length, Spektrum protocol type).
        /// </summary>
        /// <remarks>
        /// Hardware versions 1 and 2.
        /// </remarks>
        public ushort Data;

        /// <summary>
        /// Number of total received frames (wrapping counter).
        /// </summary>
        public ushort FrameCounter;

        /// <summary>
        /// Number of total dropped frames (wrapping counter).
        /// </summary>
        public ushort FrameLostCounter;

        /// <summary>
        /// Channel data from here.
        /// </summary>
        public ushort[] Channels;

        #endregion
    }
}
