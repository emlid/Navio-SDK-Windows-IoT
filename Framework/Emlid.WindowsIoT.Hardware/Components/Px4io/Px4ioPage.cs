namespace Emlid.WindowsIot.Hardware.Components.Px4io
{
    /// <summary>
    /// PX4IO pages.
    /// </summary>
    /// <see href="https://github.com/emlid/navio-rcio-linux-driver/blob/master/protocol.h"/>
    public enum Px4ioPage : byte
    {
        /// <summary>
        /// Static configuration page.
        /// </summary>
        Configuration = 0,

        /// <summary>
        /// Dynamic status page.
        /// </summary>
        Status = 1,

        /// <summary>
        /// Array of post-mix actuator outputs (-10000 to 10000).
        /// </summary>
        /// <remarks>
        /// Size specified by <see cref="Px4ioConfigurationPage.ActuatorCountMaximum"/>.
        /// </remarks>
        Actuators = 2,

        /// <summary>
        /// Array of PWM servo output values in microseconds.
        /// </summary>
        /// <remarks>
        /// Size specified by <see cref="Px4ioConfigurationPage.ActuatorCountMaximum"/>.
        /// </remarks>
        Servos = 3,

        /// <summary>
        /// Array of raw RC input values in microseconds.
        /// </summary>
        RawRcInput = 4,

        /// <summary>
        /// Array of scaled RC input values (-10000 to 10000).
        /// </summary>
        RcInput = 5,

        /// <summary>
        /// Array of raw ADC values.
        /// </summary>
        /// <remarks>
        /// Size specified by <see cref="Px4ioConfigurationPage.AdcInputCountMaximum"/>.
        /// </remarks>
        RawAdcInput = 6,

        /// <summary>
        /// PWM servo information.
        /// </summary>
        Pwm = 7,

        /// <summary>
        /// Setup page.
        /// </summary>
        Setup = 50,

        /// <summary>
        /// Autopilot control values (-10000 to 10000).
        /// </summary>
        /// <remarks>
        /// Actuator control groups, one after the other, 8 wide.
        /// </remarks>
        Controls = 51,

        /// <summary>
        /// Raw text to load into the mixer parser (ignores offset).
        /// </summary>
        MixerLoad = 52,

        /// <summary>
        /// RC channel configuration.
        /// </summary>
        RCConfig = 53,

        /// <summary>
        /// PWM output, overrides mixer.
        /// </summary>
        /// <remarks>
        /// Size specified by <see cref="Px4ioConfigurationPage.ActuatorCountMaximum"/>.
        /// </remarks>
        DirectPwm = 54,

        /// <summary>
        /// PWM failsafe values (zero disables output).
        /// </summary>
        /// <remarks>
        /// Size specified by <see cref="Px4ioConfigurationPage.ActuatorCountMaximum"/>.
        /// </remarks>
        FailsafePwm = 55,

        /// <summary>
        /// Sensors connected to PX4IO.
        /// </summary>
        Sensors = 56,

        /// <summary>
        /// PWM minimum values for certain ESCs.
        /// </summary>
        /// <remarks>
        /// Size specified by <see cref="Px4ioConfigurationPage.ActuatorCountMaximum"/>.
        /// </remarks>
        ControlMinPwm = 106,

        /// <summary>
        /// PWM maximum values for certain ESCs.
        /// </summary>
        /// <remarks>
        /// Size specified by <see cref="Px4ioConfigurationPage.ActuatorCountMaximum"/>.
        /// </remarks>
        ControlMaxPwm = 107,

        /// <summary>
        /// PWM disarmed values that are active, even when safety switch is on.
        /// </summary>
        /// <remarks>
        /// Size specified by <see cref="Px4ioConfigurationPage.ActuatorCountMaximum"/>.
        /// </remarks>
        ControlDisarmedPwm = 108,

        /// <summary>
        /// Debug and test page (not used in normal operation).
        /// </summary>
        Test = 127
    }
}
