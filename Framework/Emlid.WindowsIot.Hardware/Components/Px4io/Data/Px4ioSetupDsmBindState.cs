using System;

namespace Emlid.WindowsIot.Hardware.Components.Px4io.Data
{
    /// <summary>
    /// Defines the flags in the <see cref="Px4ioSetupRegisters.DsmBindState"/> register.
    /// </summary>
    /// <see href="https://github.com/emlid/navio-rcio-linux-driver/blob/master/protocol.h"/>
    [CLSCompliant(false)]
    public enum Px4ioSetupDsmBindState : ushort
    {
        /// <summary>
        /// Power down.
        /// </summary>
        PowerDown = 0,

        /// <summary>
        /// Power up.
        /// </summary>
        PowerUp = 1,

        /// <summary>
        /// Set receiver out.
        /// </summary>
        SetReceiverOut = 2,

        /// <summary>
        /// Send pulses.
        /// </summary>
        SendPulses = 3,

        /// <summary>
        /// Re-initialize UART.
        /// </summary>
        ReinitializeUart = 4
    }
}
