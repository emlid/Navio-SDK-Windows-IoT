using Emlid.WindowsIot.Tools.NavioHardwareTest.Models;
using Emlid.WindowsIot.Tools.NavioHardwareTest.Views.Tests;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Emlid.WindowsIot.Tools.NavioHardwareTest.Views
{
    /// <summary>
    /// Start page of the application.
    /// </summary>
    public partial class StartPage : StartPageBase
    {
        #region Lifetime

        /// <summary>
        /// Creates the page.
        /// </summary>
        public StartPage()
        {
            // Initialize view
            InitializeComponent();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates the page model when it is displayed.
        /// </summary>
        protected override StartUIModel CreateModel(TestApplicationUIModel application)
        {
            return new StartUIModel(application);
        }

        #endregion

        #region Events

        /// <summary>
        /// Initializes the page when it is loaded.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs arguments)
        {
            // Call base class method
            base.OnNavigatedTo(arguments);

            // Hook events
            Model.PropertyChanged += OnModelChanged;

            // Update bindings
            Bindings.Update();

            // Initial layout
            UpdateLayout();
        }

        /// <summary>
        /// Updates view elements when the model changes and no automatic
        /// method is currently available.
        /// </summary>
        private void OnModelChanged(object sender, PropertyChangedEventArgs arguments)
        {
            switch (arguments.PropertyName)
            {
                case nameof(Model.Application):
                    Bindings.Update();
                    break;
            }
        }

        /// <summary>
        /// Calls the model <see cref="StartUIModel.Detect"/> method when the detect button is clicked.
        /// </summary>
        private void OnDetectButtonClick(object sender, RoutedEventArgs e)
        {
            Model?.Detect();
        }

        /// <summary>
        /// Navigates to the <see cref="LedTestPage"/> when the corresponding button is clicked.
        /// </summary>
        private void OnLedTestButtonClick(object sender, RoutedEventArgs arguments)
        {
            Frame.Navigate(typeof(LedTestPage));
        }

        /// <summary>
        /// Navigates to the <see cref="PwmTestPage"/> when the corresponding button is clicked.
        /// </summary>
        private void OnPwmTestButtonClick(object sender, RoutedEventArgs arguments)
        {
            Frame.Navigate(typeof(PwmTestPage));
        }

        /// <summary>
        /// Navigates to the <see cref="RCInputTestPage"/> when the corresponding button is clicked.
        /// </summary>
        private void OnRCInputTestButtonClick(object sender, RoutedEventArgs arguments)
        {
            Frame.Navigate(typeof(RCInputTestPage));
        }

        /// <summary>
        /// Navigates to the <see cref="BarometerTestPage"/> when the corresponding button is clicked.
        /// </summary>
        private void OnBarometerTestButtonClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(BarometerTestPage));
        }

        /// <summary>
        /// Navigates to the <see cref="FramTestPage"/> when the corresponding button is clicked.
        /// </summary>
        private void OnFramTestButtonClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(FramTestPage));
        }

        /// <summary>
        /// Exits the application when the corresponding button is clicked.
        /// </summary>
        private void OnExitButtonClick(object sender, RoutedEventArgs arguments)
        {
            Application.Current.Exit();
        }

        #endregion
    }
}
