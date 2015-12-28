using Emlid.WindowsIot.Hardware;
using System;
using System.Threading.Tasks;

namespace Emlid.WindowsIot.Tests.NavioHardwareTestApp.Models
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
            Device.ChannelsChanged += OnChannelsChanged;
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
                    Device.Dispose();
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

        #region Events

        /// <summary>
        /// Updates the display when the <see cref="Device"/> channels change.
        /// </summary>
        private void OnChannelsChanged(object sender, PwmFrame frame)
        {
            // Dump statistics to output
            WriteOutput(frame.ToString());

            // Update display
            DoPropertyChanged(nameof(Device));
        }

        #endregion
    }
}
