using Emlid.WindowsIot.Hardware.Protocols.Pwm;
using System;
using System.Collections.Generic;
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
        /// Enables or disables PWM when possible.
        /// </summary>
        /// <remarks>
        /// Do not try to set false when <see cref="CanDisable"/> is not true.
        /// </remarks>
        /// <exception cref="NotSupportedException">Thrown when attempting to set the value to disabled when it is not supported.</exception>
        bool Enabled { get; set; }

        /// <summary>
        /// Indicates a different frequency can be set for each channel.
        /// </summary>
        bool FrequencyPerChannel { get; }

        /// <summary>
        /// Frequency for all channels in Hz.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When <see cref="FrequencyPerChannel"/> is true, reading this value returns the highest of all frequencies
        /// and writing it sets all channels to the same frequency.
        /// When <see cref="FrequencyPerChannel"/> is false, the device does not support multiple frequencies so
        /// all values are tied together.
        /// </para>
        /// <para>
        /// When setting the frequency, the device clock and oscillator characteristics may cause the resulting
        /// frequency to be different to what was set. Read back the value after setting to get the actual value.
        /// </para>
        /// <para>
        /// Some PWM devices do not tolerate high values and could be damaged if this is set too high,
        /// e.g. analog servos operate at much lower frequencies than digital servos.
        /// See <see cref="PwmPulse.ServoSafeFrequency"/> for more information.
        /// </para>
        /// </remarks>
        int Frequency { get; set; }

        /// <summary>
        /// Minimum frequency supported in Hz.
        /// </summary>
        int FrequencyMinimum { get; }

        /// <summary>
        /// Maximum frequency supported in Hz.
        /// </summary>
        int FrequencyMaximum { get; }

        /// <summary>
        /// Minimum pulse width in fractions of milliseconds, based on the current <see cref="Frequency"/>.
        /// </summary>
        decimal WidthMinimum { get; }

        /// <summary>
        /// Maximum pulse width in fractions of milliseconds, based on the current <see cref="Frequency"/>.
        /// </summary>
        decimal WidthMaximum { get; }

        /// <summary>
        /// PWM channel pulse widths in milliseconds.
        /// </summary>
        /// <remarks>
        /// The <see cref="PwmPulse.Frequency"/> can only be changed when the device supports independent
        /// frequencies per channel, i.e. <see cref="FrequencyPerChannel"/> is true.
        /// </remarks>
        ReadOnlyCollection<PwmPulse> Channels { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Clears all values and resets the device state to default (disabled).
        /// </summary>
        void Reset();

        /// <summary>
        /// Reads the PWM channels from the device then updates the related properties.
        /// </summary>
        void Read();

        /// <summary>
        /// Sets a single channel value.
        /// </summary>
        void SetChannel(int number, PwmPulse value);

        /// <summary>
        /// Sets multiple channel values at once.
        /// </summary>
        void SetChannels(int number, IList<PwmPulse> values, int count);

        #endregion
    }
}
