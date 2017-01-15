using Emlid.WindowsIot.Hardware.Boards.Navio;
using Emlid.WindowsIot.Hardware.Protocols.Pwm;
using System;

namespace Emlid.WindowsIot.Tools.NavioHardwareTest.Models.Tests
{
    /// <summary>
    /// UI model for testing the <see cref="INavioPwmDevice"/>.
    /// </summary>
    public sealed class PwmTestUIModel : TestUIModel
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        public PwmTestUIModel(TestApplicationUIModel application) : base(application)
        {
            // Initialize members
            Device = application.Board.Pwm;

            // Start at a safe frequency to avoid damage
            Device.Frequency = PwmPulse.ServoSafeFrequency;
        }

        #region IDisposable

        /// <summary>
        /// Frees resources owned by this instance.
        /// </summary>
        /// <param name="disposing">
        /// True when called via <see cref="IDisposable.Dispose()"/>, false when called during finalization.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                // Dispose resources when possible
                if (disposing)
                {
                    // Disable device if required
                    if (Device?.CanDisable == true)
                        Device.Enabled = false;
                }
            }
            finally
            {
                // Dispose base class
                base.Dispose(disposing);
            }
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        /// Device.
        /// </summary>
        public INavioPwmDevice Device { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Tests the <see cref="INavioPwmDevice.Reset"/> function.
        /// </summary>
        public void Reset()
        {
            // Run test
            RunTest(delegate { Device.Reset(); });

            // Update view
            DoPropertyChanged(nameof(Device));
        }

        /// <summary>
        /// Tests the <see cref="INavioPwmDevice.Read"/> function.
        /// </summary>
        public void Read()
        {
            // Run test
            RunTest(delegate { Device.Read(); });

            // Update view
            DoPropertyChanged(nameof(Device));
        }

        #endregion
    }
}
