using Emlid.WindowsIot.Hardware.Boards.Navio;

namespace Emlid.WindowsIot.Tests.NavioHardwareTestApp.Views
{
    /// <summary>
    /// Start page UI model.
    /// </summary>
    /// <remarks>
    /// Provides the selection or detection of Navio board and lists the available
    /// components which can be tested.
    /// </remarks>
    public class StartUIModel : PageUIModel
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        public StartUIModel(ApplicationUIModel application) : base(application)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Currently detected hardware model.
        /// </summary>
        public NavioHardwareModel? Model => Application.Board?.Model;

        /// <summary>
        /// Indicates whether the ADC device is available to test.
        /// </summary>
        public bool AdcAvailable => Application.Board?.Adc != null;

        /// <summary>
        /// Indicates whether the barometer device is available to test.
        /// </summary>
        public bool BarometerAvailable => Application.Board?.Barometer != null;

        /// <summary>
        /// Indicates whether the FRAM device is available to test.
        /// </summary>
        public bool FramAvailable => Application.Board?.Fram != null;

        /// <summary>
        /// Indicates whether the GPS device is available to test.
        /// </summary>
        public bool GpsAvailable => Application.Board?.Gps != null;

        /// <summary>
        /// Indicates whether the primary IMU device is available to test.
        /// </summary>
        public bool Imu1Available => Application.Board?.Imu1 != null;

        /// <summary>
        /// Indicates whether the secondary IMU device is available to test.
        /// </summary>
        public bool Imu2Available => Application.Board?.Imu2 != null;

        /// <summary>
        /// Indicates whether the LED device is available to test.
        /// </summary>
        public bool LedAvailable => Application.Board?.Led != null;

        /// <summary>
        /// Indicates whether the PWM device is available to test.
        /// </summary>
        public bool PwmAvailable => Application.Board?.Pwm != null;

        /// <summary>
        /// Indicates whether the RC input device is available to test.
        /// </summary>
        public bool RCInputAvailable => Application.Board?.RCInput != null;

        #endregion

        #region Public Methods

        /// <summary>
        /// Attempts auto-detection of the currently installed Navio board.
        /// </summary>
        public void Detect()
        {
            // Call application method
            Application.Detect();

            // Fire property changed event
            DoPropertyChanged(nameof(Model));
        }

        #endregion
    }
}
