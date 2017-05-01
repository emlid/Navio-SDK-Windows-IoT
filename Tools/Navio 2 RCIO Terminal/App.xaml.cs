using Emlid.WindowsIot.Tools.Navio2RcioTerminal.Models;
using Emlid.WindowsIot.Tools.Navio2RcioTerminal.Views;
using System;
using System.Threading.Tasks;

namespace Emlid.WindowsIot.Tools.Navio2RcioTerminal
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : AppBase
    {
        #region Lifetime

        /// <summary>
        /// Initializes the singleton application object. This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            // Initialize component
            InitializeComponent();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates an application specific model at runtime.
        /// </summary>
        protected override RcioTerminalApplicationUIModel CreateModel(TaskFactory uiTaskFactory)
        {
            // Create UI model
            return new RcioTerminalApplicationUIModel(uiTaskFactory);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Start-up page type.
        /// </summary>
        public override Type StartPageType => typeof(StartPage);

        #endregion
    }
}
