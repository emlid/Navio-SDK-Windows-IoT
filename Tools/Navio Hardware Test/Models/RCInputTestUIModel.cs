using Emlid.WindowsIot.Hardware.Boards.Navio;
using Emlid.WindowsIot.Hardware.Protocols.Pwm;
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
            // Only managed resources to dispose
            if (!disposing)
                return;

            // Un-hook events
            Device.ChannelsChanged -= OnChannelsChanged;

            // Close device
            Device?.Dispose();
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
