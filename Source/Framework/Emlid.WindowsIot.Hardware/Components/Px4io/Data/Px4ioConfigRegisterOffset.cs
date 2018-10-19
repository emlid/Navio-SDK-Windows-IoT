using System;
using System.Diagnostics.CodeAnalysis;

namespace Emlid.WindowsIot.Hardware.Components.Px4io.Data
{
    /// <summary>
    /// PX4IO configuration page register offsets.
    /// </summary>
    /// <see href="https://github.com/emlid/navio-rcio-linux-driver/blob/master/protocol.h"/>
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Design", "CA1028", Justification = "Non-integer enumeration better suited to match the hardware specification in a hardware library.")]
    public enum Px4ioConfigRegisterOffset : byte
    {
        /// <summary>
        /// Protocol version.
        /// </summary>
        ProtocolVersion = 0,

        /// <summary>
        /// Magic numbers representing hardware revisions (to be defined).
        /// </summary>
        HardwareVersion = 1,

        /// <summary>
        /// Boot-loader version.
        /// </summary>
        BootLoaderVersion = 2,

        /// <summary>
        /// Maximum transfer size.
        /// </summary>
        TransferLimit = 3,

        /// <summary>
        /// Hard coded max control count supported.
        /// </summary>
        ControlCount = 4,

        /// <summary>
        /// Hard coded max actuator output count.
        /// </summary>
        ActuatorCount = 5,

        /// <summary>
        /// Hard coded max R/C input count supported.
        /// </summary>
        RCInputCount = 6,

        /// <summary>
        /// Hard coded max ADC inputs.
        /// </summary>
        AdcInputCount = 7,

        /// <summary>
        /// Hard coded # of relay outputs.
        /// </summary>
        RelayCount = 8,

        /// <summary>
        /// Hard coded # of control groups.
        /// </summary>
        ControlGroupCount = 9
    }
}