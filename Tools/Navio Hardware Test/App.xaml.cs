using Emlid.UniversalWindows.UI.Views;
using Emlid.WindowsIot.Tools.NavioHardwareTest.Models;
using Emlid.WindowsIot.Tools.NavioHardwareTest.Views;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Emlid.WindowsIot.Tools.NavioHardwareTest
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : AppBase
    {
        #region Lifetime

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            // Initialize component
            InitializeComponent();

            // Hook events
            Suspending += OnSuspending;
            UnhandledException += OnError;
        }

        #endregion

        #region Events

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="arguments">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs arguments)
        {
            // Call base class method
            base.OnLaunched(arguments);

            // Start frame counter when debugging
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
                DebugSettings.EnableFrameRateCounter = true;
#endif
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (arguments.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(StartPage), arguments.Arguments);
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Creates an application specific model at runtime.
        /// </summary>
        protected override TestApplicationUIModel CreateModel()
        {
            // Create UI model
            return new TestApplicationUIModel();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="arguments">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs arguments)
        {
            arguments.Handled = true;
            throw new InvalidOperationException("Failed to load Page " + arguments.SourcePageType.FullName +
                Environment.NewLine + arguments.Exception.Message);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="arguments">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs arguments)
        {
            var deferral = arguments.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        /// <summary>
        /// Error handler.
        /// </summary>
        private void OnError(object sender, UnhandledExceptionEventArgs error)
        {
            // Create error dialog
            var dialog = new MessageDialog("Error", error.Message);

            // Show dialog
            // TODO: Get this working
            var uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            var uiFactory = new TaskFactory(uiScheduler);
            uiFactory.StartNew(() => { dialog.ShowAsync().AsTask().Wait(); });

            // Flag handled so application can continue
            error.Handled = false;
        }

        #endregion
    }
}
