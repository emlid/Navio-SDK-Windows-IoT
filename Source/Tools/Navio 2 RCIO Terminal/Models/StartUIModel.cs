using CodeForDotNet.UI.Models;

namespace Emlid.WindowsIot.Tools.Navio2RcioTerminal.Models
{
    /// <summary>
    /// Start page UI model.
    /// </summary>
    /// <remarks>
    /// Provides the selection or detection of Navio board and lists the available
    /// components which can be tested.
    /// </remarks>
    public sealed class StartUIModel : PageUIModel<RcioTerminalApplicationUIModel>
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        public StartUIModel(RcioTerminalApplicationUIModel application) : base(application)
        {
        }

        #endregion Lifetime
    }
}