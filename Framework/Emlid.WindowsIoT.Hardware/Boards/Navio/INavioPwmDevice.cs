using Emlid.WindowsIot.Hardware.Protocols.Pwm;
using System;
using System.Collections.ObjectModel;

namespace Emlid.WindowsIot.Hardware.Boards.Navio
{
    /// <summary>
    /// Navio PWM output device.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Difference Navio models use different methods for PWM output. This interface provides a hardware
    /// model agnostic way to control the PWM output and detect the channel capabilities.
    /// </para>
    /// <para>
    /// Navio and Navio Plus use a PCA9685 hardware PWM chip which is limited to a single frequency
    /// and loses 3 of the 16 possible outputs by connecting to the RGB LED, although gaining
    /// hardware PWM for the LED (68 million colors).
    /// </para>
    /// <para>
    /// Navio 2 uses an STM32 co-processor for "hardware" (microprocessor firmware) PWM output of
    /// up to 14 channels, but loses fidelity of the RGB LED (only 16 colors) because that
    /// is then connected to GPIO pins.
    /// </para>
    /// </remarks>
    public interface INavioPwmDevice
    {
        #region Properties

        /// <summary>
        /// Indicates whether PWM can be disabled, i.e. must be enabled before it will generate output.
        /// </summary>
        bool CanDisable { get; }

        /// <summary>
        /// Indicates whether sleep mode (<see cref="Sleep"/> and <see cref="Wake"/>) is supported.
        /// </summary>
        bool CanSleep { get; }

        /// <summary>
        /// Indicates whether <see cref="Restart"/> is supported.
        /// </summary>
        bool CanRestart { get; }

        /// <summary>
        /// Enables or disables PWM when possible.
        /// </summary>
        /// <remarks>
        /// Do not try to set false when <see cref="CanDisable"/> is not true.
        /// </remarks>
        /// <exception cref="NotSupportedException">Thrown when attempting to set the value to disabled when it is not supported.</exception>
        bool Enabled { get; set; }

        /// <summary>
        /// PWM channels.
        /// </summary>
        Collection<PwmCycle> Channels { get; }

        /// <summary>
        /// Indicates a different frequency can be set for each channel.
        /// </summary>
        bool FrequencyPerChannel { get; }

        /// <summary>
        /// Frequency in Hz.
        /// </summary>
        /// <remarks>
        /// Some PWM devices do not tolerate high values and could be damaged if this is set too high,
        /// e.g. analog servos operate at much lower frequencies than digital servos.
        /// See <see cref="PwmCycle.ServoSafeFrequency"/> for more information.
        /// </remarks>
        float Frequency { get; }

        /// <summary>
        /// Minimum frequency supported.
        /// </summary>
        float FrequencyMinimum { get; }

        /// <summary>
        /// Maximum frequency supported.
        /// </summary>
        float FrequencyMaximum { get; }

        /// <summary>
        /// Minimum PWM length in milliseconds, based on the current <see cref="Frequency"/>.
        /// </summary>
        float LengthMinimum { get; }

        /// <summary>
        /// Maximum PWM length in milliseconds, based on the current <see cref="Frequency"/>.
        /// </summary>
        float LengthMaximum { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Clears all values.
        /// </summary>
        void Clear();

        /// <summary>
        /// Reads the PWM channels from the device then updates the related properties.
        /// </summary>
        void Read();

        /// <summary>
        /// Puts the device into sleep mode when supported.
        /// </summary>
        /// <returns>
        /// True when mode was changed, false when already set.
        /// </returns>
        bool Sleep();

        /// <summary>
        /// Resumes from sleep when supported.
        /// </summary>
        /// <returns>
        /// True when mode was changed, false when already set.
        /// </returns>
        bool Wake();

        /// <summary>
        /// Restarts the device when supported.
        /// </summary>
        void Restart();

        /// <summary>
        /// Sets the frequency in Hz.
        /// </summary>
        /// <param name="frequency">Frequency to set in Hz.</param>
        /// <returns>
        /// Effective frequency in Hz, read-back and recalculated after setting the desired frequency.
        /// Frequency in Hz. Related properties are also updated.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="frequency"/> is less than <see cref="FrequencyMinimum"/> or greater than
        /// <see cref="FrequencyMaximum"/>.
        /// </exception>
        float SetFrequency(float frequency);

        #endregion
    }
}
