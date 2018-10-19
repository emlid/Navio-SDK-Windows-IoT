using System;

namespace Emlid.WindowsIot.Hardware.Components.Px4io.Data
{
    /// <summary>
    /// <see cref="Px4ioPage.Status"/> page register data.
    /// </summary>
    /// <see href="https://github.com/emlid/navio-rcio-linux-driver/blob/master/protocol.h"/>
    [CLSCompliant(false)]
    public sealed class Px4ioStatusRegisters
    {
        #region Constants

        /// <summary>
        /// Number of registers on this page.
        /// </summary>
        public const byte RegisterCount = 10;

        #endregion Constants

        #region Lifetime

        /// <summary>
        /// Creates an instance from register values.
        /// </summary>
        /// <param name="data">Register values read from the device.</param>
        public Px4ioStatusRegisters(ushort[] data)
        {
            // Validate
            if (data == null || data.Length < RegisterCount)
                throw new ArgumentOutOfRangeException(nameof(data));

            // Set properties from data
            FreeMemory = data[0];
            CpuLoad = data[1];
            Monitoring = (Px4ioStatusMonitoringFlags)data[2];
            Alarms = (Px4ioStatusAlarmFlags)data[3];
            BatteryVoltage = data[4];
            BatteryCurrent = data[5];
            ServoVoltage = data[6];
            RssiVoltage = data[7];
            RssiPwm = data[8];
            Mixer = (Px4ioStatusMixerFlags)data[9];
        }

        #endregion Lifetime

        #region Public Fields

        /// <summary>
        /// Free memory.
        /// </summary>
        public ushort FreeMemory { get; set; }

        /// <summary>
        /// CPU load.
        /// </summary>
        public ushort CpuLoad { get; set; }

        /// <summary>
        /// Monitoring flags.
        /// </summary>
        public Px4ioStatusMonitoringFlags Monitoring { get; set; }

        /// <summary>
        /// Alarm flags.
        /// </summary>
        /// <remarks>
        /// Alarms latch, write 1 to a bit to clear it.
        /// </remarks>
        public Px4ioStatusAlarmFlags Alarms { get; set; }

        /// <summary>
        /// Battery voltage in mV.
        /// </summary>
        /// <remarks>
        /// Hardware version 1 only.
        /// </remarks>
        public ushort BatteryVoltage { get; set; }

        /// <summary>
        /// Battery current (raw ADC).
        /// </summary>
        /// <remarks>
        /// Hardware version 1 only.
        /// </remarks>
        public ushort BatteryCurrent { get; set; }

        /// <summary>
        /// Servo rail voltage in mV.
        /// </summary>
        /// <remarks>
        /// Hardware version 2 only.
        /// </remarks>
        public ushort ServoVoltage { get; set; }

        /// <summary>
        /// RSSI voltage.
        /// </summary>
        /// <remarks>
        /// Hardware version 2 only.
        /// </remarks>
        public ushort RssiVoltage { get; set; }

        /// <summary>
        /// RSSI PWM value.
        /// </summary>
        /// <remarks>
        /// Hardware version 2 only.
        /// </remarks>
        public ushort RssiPwm { get; set; }

        /// <summary>
        /// Mixer actuator limit flags.
        /// </summary>
        public Px4ioStatusMixerFlags Mixer { get; set; }

        #endregion Public Fields
    }
}