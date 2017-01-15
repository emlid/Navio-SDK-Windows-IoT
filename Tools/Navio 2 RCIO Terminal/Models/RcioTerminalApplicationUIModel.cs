using Emlid.UniversalWindows.UI.Models;
using Emlid.WindowsIot.Hardware.Boards.Navio.Internal;
using System;

namespace Emlid.WindowsIot.Tools.Navio2RcioTerminal.Models
{
    /// <summary>
    /// Application UI model.
    /// </summary>
    public class RcioTerminalApplicationUIModel : ApplicationUIModel
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        public RcioTerminalApplicationUIModel()
        {
            Rcio = new Navio2RcioDevice();
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
                    Rcio?.Dispose();
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
        /// RCIO device.
        /// </summary>
        public Navio2RcioDevice Rcio { get; private set; }

        #endregion
    }
}
