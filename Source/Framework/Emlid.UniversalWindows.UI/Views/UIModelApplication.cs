using Emlid.UniversalWindows.UI.Models;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Emlid.UniversalWindows.UI.Views
{
    /// <summary>
    /// Base class for all XAML applications which support the UI model framework.
    /// </summary>
    public abstract partial class UIModelApplication<TApplicationUIModel> : Application
        where TApplicationUIModel : ApplicationUIModel
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance with the specified model.
        /// </summary>
        protected UIModelApplication()
        {
            // Hook events
            Suspending += OnSuspending;
            UnhandledException += OnError;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Application UI model.
        /// </summary>
        public TApplicationUIModel Model { get; private set; }

        /// <summary>
        /// Start-up page type.
        /// </summary>
        public abstract Type StartPageType { get; }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates the application model when it is started.
        /// </summary>
        protected abstract TApplicationUIModel CreateModel(TaskFactory factory);

        /// <summary>
        /// Loads saved state when the application is launched.
        /// </summary>
        protected virtual void Load(LaunchActivatedEventArgs arguments)
        {
        }

        /// <summary>
        /// Saves state when the application is launched.
        /// </summary>
        protected virtual void Save(SuspendingEventArgs arguments)
        {
        }

        #endregion

        #region Events

        /// <summary>
        /// Starts the application.
        /// </summary>
        /// <param name="arguments">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs arguments)
        {
            try
            {
                // Call base class method to initialize model
                base.OnLaunched(arguments);

                // Create UI
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
                        // Load state from previously suspended application
                        Load(arguments);
                    }

                    // Place the frame in the current Window
                    Window.Current.Content = rootFrame;
                }

                // Create UI model
                var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
                var factory = new TaskFactory(scheduler);
                Model = CreateModel(factory);

                // Create and navigate to start page when no content
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(StartPageType, arguments.Arguments);

                    // Ensure the current window is active
                    Window.Current.Activate();
                }
            }
            catch (Exception error)
            {
                // Ensure the current window is initialized and visible
                var window = Window.Current;
                if (window.Content == null) window.Content = new Frame();
                if (!window.Visible) window.Activate();

                // Show error dialog
                var dialog = new MessageDialog(error.Message, "Launch Error");
                dialog.Commands.Add(new UICommand("Close"));
                await dialog.ShowAsync();

                // Quit
                Exit();
            }
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

            // Save application state and stop any background activity
            Save(arguments);

            deferral.Complete();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="arguments">Details about the navigation failure</param>
        protected virtual void OnNavigationFailed(object sender, NavigationFailedEventArgs arguments)
        {
            arguments.Handled = true;
            throw new InvalidOperationException("Failed to load Page " + arguments.SourcePageType.FullName +
                Environment.NewLine + arguments.Exception.Message);
        }

        /// <summary>
        /// Error handler.
        /// </summary>
        protected virtual async void OnError(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs arguments)
        {
            // Show error dialog
            var dialog = new MessageDialog(arguments.Message, "Runtime Error");
            dialog.Commands.Add(new UICommand("Ignore", null, "Ignore"));
            dialog.Commands.Add(new UICommand("Close", null, "Close"));
            var result = await dialog.ShowAsync();
            if ((string)result.Id == "Ignore")
            {
                // Flag handled when "ignore" clicked, so application can continue
                arguments.Handled = true;
            }
        }

        #endregion
    }
}
