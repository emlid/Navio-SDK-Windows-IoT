using Emlid.WindowsIoT.Hardware;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Emlid.WindowsIoT.Tests.HardwareTestApp.Models
{
    /// <summary>
    /// UI model for testing the <see cref="NavioRCInputDevice"/>.
    /// </summary>
    public class RCInputTestUIModel : TestUIModel
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        public RCInputTestUIModel(TaskFactory uiThread) : base(uiThread)
        {
            // Initialize device
            Device = new NavioRCInputDevice();
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
            // Dispose only once
            if (IsDisposed) return;

            // Dispose
            try
            {
                // Free managed resources
                if (disposing)
                {
                    Device.Dispose();
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

        #region Properties

        /// <summary>
        /// Device.
        /// </summary>
        public NavioRCInputDevice Device { get; private set; }

        #endregion

        #region Protected Methods

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

        #endregion
    }
}
