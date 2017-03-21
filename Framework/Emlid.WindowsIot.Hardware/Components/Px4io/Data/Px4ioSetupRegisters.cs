using System;

namespace Emlid.WindowsIot.Hardware.Components.Px4io.Data
{
    /// <summary>
    /// <see cref="Px4ioPage.Setup"/> page register data.
    /// </summary>
    /// <see href="https://github.com/emlid/navio-rcio-linux-driver/blob/master/protocol.h"/>
    [CLSCompliant(false)]
    public sealed class Px4ioSetupRegisters
    {
        #region Constants

        /// <summary>
        /// Number of registers on this page.
        /// </summary>
        public const byte RegisterCount = 15;

        /// <summary>
        /// "Magic" (random) number required to activate the function of the <see cref="RebootBootLoader"/> register.
        /// </summary>
        public const ushort RebootBootLoaderMagic = 14662;

        /// <summary>
        /// "Magic" (random) number required to activate the function of the <see cref="ForceSafetyOn"/> register.
        /// </summary>
        public const ushort ForceSafetyOnMagic = 22027;	/* required argument for force safety (random) */


        #endregion

        #region Lifetime

        /// <summary>
        /// Creates an instance from register values.
        /// </summary>
        /// <param name="data">Register values read from the device.</param>
        public Px4ioSetupRegisters(ushort[] data)
        {
            // Validate
            if (data == null || data.Length < RegisterCount)
                throw new ArgumentOutOfRangeException(nameof(data));

            // Set properties from data
            Features = (Px4ioSetupFeatureFlags)data[0];
            Arming = (Px4ioSetupArmingFlags)data[1];
            PwmRates = data[2];
            PwmDefaultRate = data[3];
            PwmAlternateRate = data[4];
            RelaysPad = data[5];
            VoltageScale = data[6];
            DsmBindState = (Px4ioSetupDsmBindState)data[7];
            Debug = data[9];
            RebootBootLoader = data[10];
            Crc = (((uint)data[11] << 16) | data[12]);
            RCThrottleFailsafe = data[12];
            ForceSafetyOn = data[13];
        }

        #endregion

        #region Public Fields

        /// <summary>
        /// Features.
        /// </summary>
        public Px4ioSetupFeatureFlags Features;

        /// <summary>
        /// Arming controls.
        /// </summary>
        public Px4ioSetupArmingFlags Arming;

        /// <summary>
        /// Bitmask, 0 = low rate, 1 = high rate.
        /// </summary>
        public ushort PwmRates;

        /// <summary>
        /// Low PWM frame output rate in Hz.
        /// </summary>
        public ushort PwmDefaultRate;

        /// <summary>
        /// High PWM frame output rate in Hz.
        /// </summary>
        public ushort PwmAlternateRate;

        /// <summary>
        /// Bitmask of relay/switch outputs, 0 = off, 1 = on.
        /// </summary>
        /// <remarks>
        /// Hardware version 1 only.
        /// </remarks>
        public Px4ioSetupRelayFlags Relays => (Px4ioSetupRelayFlags)RelaysPad;

        /// <summary>
        /// Bitmask of relay/switch outputs, 0 = off, 1 = on.
        /// </summary>
        /// <remarks>
        /// Hardware version 2 only.
        /// </remarks>
        public ushort RelaysPad;

        /// <summary>
        /// Servo or battery voltage correction factor (float).
        /// </summary>
        /// <remarks>
        /// Hardware version 1 = Battery voltage scale.
        /// Hardware version 2 = Servo voltage scale.
        /// </remarks>
        public ushort VoltageScale;

        /// <summary>
        /// DSM bind state.
        /// </summary>
        public Px4ioSetupDsmBindState DsmBindState;

        /// <summary>
        /// Debug level for IO board.
        /// </summary>
        public ushort Debug;

        /// <summary>
        /// Reboot IO into boot-loader.
        /// </summary>
        /// <remarks>
        /// Set to the the <see cref="RebootBootLoaderMagic"/> value to initiate.
        /// </remarks>
        public ushort RebootBootLoader;

        /// <summary>
        /// Get CRC of IO firmware.
        /// </summary>
        public uint Crc;

        /// <summary>
        /// Throttle failsafe pulse length in microseconds.
        /// </summary>
        public ushort RCThrottleFailsafe;

        /// <summary>
        /// Force safety switch into 'disarmed' (PWM disabled state).
        /// </summary>
        /// <remarks>
        /// Set to the <see cref="ForceSafetyOnMagic"/> value to initiate.
        /// </remarks>
        public ushort ForceSafetyOn;

        #endregion
    }
}
