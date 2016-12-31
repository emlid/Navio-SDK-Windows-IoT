using Emlid.WindowsIot.Hardware.Boards.Navio;
using System;
using System.Runtime.CompilerServices;

namespace Emlid.WindowsIot.Tests.NavioHardwareTestApp.Views.Tests
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
        public PwmTestUIModel(ApplicationUIModel application) : base(application)
        {
            // Initialize members
            Device = application.Board.Pwm;
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
        /// Clears any existing output then tests the <see cref="INavioPwmDevice.Clear"/> function.
        /// </summary>
        public override void Clear()
        {
            // Call base class to clear output
            base.Clear();

            // Run device clear test
            RunTest(delegate { Device.Clear(); });
        }

        /// <summary>
        /// Tests the <see cref="INavioPwmDevice.Read"/> function.
        /// </summary>
        public void Read()
        {
            RunTest(delegate { Device.Read(); });
        }

        /// <summary>
        /// Tests the <see cref="INavioPwmDevice.Sleep"/> function.
        /// </summary>
        public void Sleep()
        {
            RunTest(delegate { Device.Sleep(); });
        }

        /// <summary>
        /// Tests the <see cref="INavioPwmDevice.Wake"/> function.
        /// </summary>
        public void Wake()
        {
            RunTest(delegate { Device.Wake(); });
        }

        /// <summary>
        /// Tests the <see cref="INavioPwmDevice.Restart"/> function.
        /// </summary>
        public void Restart()
        {
            RunTest(delegate { Device.Restart(); });
        }

        #endregion

        #region Non-Public Methods

        /// <summary>
        /// Runs a test method with status and error output.
        /// </summary>
        /// <param name="test">Test delegate to run.</param>
        /// <param name="name">Name to use in the output.</param>
        protected override void RunTest(Action test, [CallerMemberName] string name = "")
        {
            // Call base class to run test
            base.RunTest(test, name);

            // Update properties
            DoPropertyChanged(nameof(Device));
        }

        /// <summary>
        /// Enables output if necessary.
        /// </summary>
        private void EnsureOutputEnabled()
        {
            if (!Device.Enabled)
            {
                WriteOutput("Enabling output.");
                Device.Enabled = true;
                DoPropertyChanged(nameof(Device));
            }
        }

        #endregion
    }
}
