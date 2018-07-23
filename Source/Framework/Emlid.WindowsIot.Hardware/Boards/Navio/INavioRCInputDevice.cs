using Emlid.WindowsIot.Hardware.Protocols.Ppm;
using System;
using System.Collections.ObjectModel;

namespace Emlid.WindowsIot.Hardware.Boards.Navio
{
    /// <summary>
    /// Navio RC input device.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Difference Navio models use different methods for RC input. This interface provides a hardware
    /// model agnostic way to control the RC input and supported protocols.
    /// </para>
    /// <para>
    /// Navio and Navio Plus use a GPIO pin which requires software PPM conversion, consuming
    /// significant processor resources and introducing latencies and inaccuracies. Additionally
    /// it is not feasible to support any other protocols than CPPM because alternative
    /// protocols such as SBUS usually require a much faster cycle detection rate.
    /// </para>
    /// <para>
    /// Navio 2 uses an STM32 co-processor for "hardware" (microprocessor firmware) RC input
    /// with multiple protocols including CPPM and SBUS.
    /// </para>
    /// </remarks>
    public interface INavioRCInputDevice
    {
        #region Properties

        /// <summary>
        /// PPM channel values.
        /// </summary>
        ReadOnlyCollection<int> Channels { get; }

        /// <summary>
        /// Indicates multiple protocols are supported, otherwise only CPPM is possible.
        /// </summary>
        bool Multiprotocol { get; }

        #endregion

        #region Events

        /// <summary>
        /// Fired after a new frame of data has been received and decoded into <see cref="Channels"/>.
        /// </summary>
        event EventHandler<PpmFrame> ChannelsChanged;

        #endregion
    }
}
