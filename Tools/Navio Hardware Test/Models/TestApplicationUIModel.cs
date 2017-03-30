using Emlid.UniversalWindows.UI.Models;
using Emlid.WindowsIot.Hardware.Boards.Navio;
using System;
using System.Threading.Tasks;

namespace Emlid.WindowsIot.Tools.NavioHardwareTest.Models
{
    /// <summary>
    /// Application UI model.
    /// </summary>
    public class TestApplicationUIModel : ApplicationUIModel
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        public TestApplicationUIModel(TaskFactory uiTaskFactory)
            : base(uiTaskFactory)
        {
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
                    Board?.Dispose();
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
        /// Navio board.
        /// </summary>
        public INavioBoard Board { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Attempts auto-detection of the currently installed Navio board.
        /// </summary>
        public void Detect()
        {
            // Detect model
            var model = NavioDeviceProvider.Detect();
            if (model.HasValue && Board?.Model != model)
            {
                // Connect to hardware when found and not already connected
                Board = NavioDeviceProvider.Connect(model.Value);

                // Fire changed event
                DoPropertyChanged(nameof(Board));
            }
        }

        #endregion
    }
}
