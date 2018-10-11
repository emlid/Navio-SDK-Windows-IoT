using CodeForDotNet.WindowsUniversal.UI.Models;
using Emlid.WindowsIot.Tools.Navio2RcioTerminal.Models;

namespace Emlid.WindowsIot.Tools.Navio2RcioTerminal.Views
{
    /// <summary>
    /// Wraps the generic XAML base class so that it can be used in the UWP and XAML designer,
    /// which does not support generic or self-references base class type arguments.
    /// </summary>
    public abstract class StartPageBase : UIModelPage<RcioTerminalApplicationUIModel, StartUIModel>
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        protected StartPageBase()
        {
        }

        #endregion Lifetime
    }
}