using System;

namespace Emlid.WindowsIot.Hardware.Components.Px4io.Data
{
    /// <summary>
    /// <see cref="Px4ioPage.RCInputRaw"/> page register offsets.
    /// </summary>
    /// <see href="https://github.com/emlid/navio-rcio-linux-driver/blob/master/protocol.h"/>
    [CLSCompliant(false)]
    public enum Px4ioRCInputRawRegisterOffset : byte
    {
        /// <summary>
        /// Number of valid channels.
        /// </summary>
        ChannelCount = 0,

        /// <summary>
        /// RC detail status flags.
        /// </summary>
        StatusFlags = 1,

        /// <summary>
        /// Normalized RSSI value, 0: no reception, 255: perfect reception.
        /// </summary>
        /// <remarks>
        /// Hardware version 2 only.
        /// </remarks>
        RssiNormal = 2,

        /// <summary>
        /// Details about the RC source (PPM frame length, Spektrum protocol type).
        /// </summary>
        /// <remarks>
        /// Hardware versions 1 and 2.
        /// </remarks>
        Data = 3,

        /// <summary>
        /// Number of total received frames (wrapping counter).
        /// </summary>
        FrameCounter = 4,

        /// <summary>
        /// Number of total dropped frames (wrapping counter).
        /// </summary>
        FrameLostCounter = 5,

        /// <summary>
        /// <see cref="ChannelCount"/> channel data from here.
        /// </summary>
        ChannelsStart = 6
    }
}
