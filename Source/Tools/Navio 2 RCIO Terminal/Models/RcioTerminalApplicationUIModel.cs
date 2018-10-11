using CodeForDotNet.UI.Models;
using Emlid.WindowsIot.Hardware.Boards.Navio;
using Emlid.WindowsIot.Hardware.Boards.Navio.Internal;
using Emlid.WindowsIot.Tools.Navio2RcioTerminal.Resources;
using System;
using System.Threading.Tasks;

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
        public RcioTerminalApplicationUIModel(TaskFactory uiTaskFactory)
            : base(uiTaskFactory)
        {
            // Run on background thread (necessary for C++/WinRT hardware access)
            Task.Run(() =>
            {
                // Ensure we are running on a Navio 2
                if (NavioDeviceProvider.Detect() != NavioHardwareModel.Navio2)
                    throw new InvalidOperationException(Strings.UnsupportedModelError);

                // Initialize RCIO
                Rcio = new Navio2RcioDevice();
            })
            .Wait();
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
                    Rcio?.Dispose();
            }
            finally
            {
                // Dispose base class
                base.Dispose(disposing);
            }
        }

        #endregion IDisposable

        #endregion Lifetime

        #region Properties

        /// <summary>
        /// RCIO device.
        /// </summary>
        public Navio2RcioDevice Rcio { get; private set; }

        #endregion Properties
    }
}