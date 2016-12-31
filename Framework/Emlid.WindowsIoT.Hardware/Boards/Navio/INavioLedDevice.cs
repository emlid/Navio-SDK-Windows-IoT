using System;

namespace Emlid.WindowsIot.Hardware.Boards.Navio
{
    /// <summary>
    /// Navio RGB LED device interface.
    /// </summary>
    /// <remarks>
    /// Different Navio models support varying color ranges. This interface provides a hardware
    /// model agnostic way to control the LED color and determine the color range.
    /// </remarks>
    public interface INavioLedDevice
    {
        #region Properties

        /// <summary>
        /// Indicates whether the LED can be disabled, i.e. must be enabled before it will show color.
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
        /// Enables or disables the LED when possible.
        /// </summary>
        /// <remarks>
        /// Do not try to set false when <see cref="CanDisable"/> is not true.
        /// </remarks>
        /// <exception cref="NotSupportedException">Thrown when attempting to set the value to disabled when it is not supported.</exception>
        bool Enabled { get; set; }

        /// <summary>
        /// Maximum value of any color component.
        /// </summary>
        /// <remarks>
        /// The color range is calculated as <see cref="MaximumValue"/> + 1 ^3.
        /// </remarks>
        int MaximumValue { get; }

        /// <summary>
        /// Intensity of the red LED component.
        /// </summary>
        /// <remarks>
        /// Value in the range 0-<see cref="MaximumValue"/>.
        /// </remarks>
        int Red { get; set; }

        /// <summary>
        /// Intensity of the green LED component.
        /// </summary>
        /// <remarks>
        /// Value in the range 0-<see cref="MaximumValue"/>.
        /// </remarks>
        int Green { get; set; }

        /// <summary>
        /// Intensity of the green LED component.
        /// </summary>
        /// <remarks>
        /// Value in the range 0-<see cref="MaximumValue"/>.
        /// </remarks>
        int Blue { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Clears all values.
        /// </summary>
        void Clear();

        /// <summary>
        /// Reads the LED values from the device then updates the related properties.
        /// </summary>
        void Read();

        /// <summary>
        /// Sets the LED <see cref="Red"/>, <see cref="Green"/> and <see cref="Blue"/> values together (in one operation).
        /// </summary>
        /// <param name="red">Red value in the range 0-<see cref="MaximumValue"/>.</param>
        /// <param name="green">Green value in the range 0-<see cref="MaximumValue"/>.</param>
        /// <param name="blue">Blue value in the range 0-<see cref="MaximumValue"/>.</param>
        void SetRgb(int red, int green, int blue);

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

        #endregion
    }
}
