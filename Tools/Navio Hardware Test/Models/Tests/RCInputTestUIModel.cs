using Emlid.WindowsIot.Hardware.Boards.Navio;
using Emlid.WindowsIot.Hardware.Protocols.Pwm;
using System;

namespace Emlid.WindowsIot.Tests.NavioHardwareTestApp.Views.Tests
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
        public RCInputTestUIModel(ApplicationUIModel application) : base(application)
        {
            // Initialize device
            Device = Application.Board.RCInput;
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
            try
            {
                // Dispose resources when possible
                if (disposing)
                {
                    // Unhook events
                    Device.ChannelsChanged -= OnChannelsChanged;
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
        public INavioRCInputDevice Device { get; private set; }

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
