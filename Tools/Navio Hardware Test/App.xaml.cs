using Emlid.WindowsIot.Tools.NavioHardwareTest.Models;
using Emlid.WindowsIot.Tools.NavioHardwareTest.Views;
using System;
using System.Threading.Tasks;

namespace Emlid.WindowsIot.Tools.NavioHardwareTest
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : AppBase
    {
        #region Lifetime

        /// <summary>
        /// Initializes the singleton application object.This is the first line of authored code
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
        protected override Task<TestApplicationUIModel> CreateModel(TaskScheduler scheduler)
        {
            // Create UI model
            return Task.Run(() => new TestApplicationUIModel(new TaskFactory(scheduler)));
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
