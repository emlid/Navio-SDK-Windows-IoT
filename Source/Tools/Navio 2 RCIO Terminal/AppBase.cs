using Emlid.UniversalWindows.UI.Views;
using Emlid.WindowsIot.Tools.Navio2RcioTerminal.Models;

namespace Emlid.WindowsIot.Tools.Navio2RcioTerminal
{
    /// <summary>
    /// Wraps the generic XAML base class so that it can be used in the UWP and XAML designer,
    /// which does not support generic or self-references base class type arguments.
    /// </summary>
    public abstract class AppBase : UIModelApplication<RcioTerminalApplicationUIModel>
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        protected AppBase()
        {
        }

        #endregion
    }
}
