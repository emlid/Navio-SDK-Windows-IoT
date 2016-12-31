using Emlid.WindowsIot.Hardware.Boards.Navio;
using System;

namespace Emlid.WindowsIot.Tests.NavioHardwareTestApp.Views
{
    /// <summary>
    /// Application UI model.
    /// </summary>
    public class ApplicationUIModel : UIModel
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        public ApplicationUIModel()
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
                {
                    Board?.Dispose();
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
            // Clear existing model
            if (Board != null)
            {
                Board.Dispose();
                Board = null;
            }

            // Detect model
            var model = NavioDeviceProvider.Detect();
            if (model.HasValue)
            {
                // Create model when found
                Board = NavioDeviceProvider.Connect(model.Value);
            }

            // Fire changed event
            DoPropertyChanged(nameof(Board));
        }

        #endregion
    }
}
