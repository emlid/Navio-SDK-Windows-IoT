using System;
using System.ComponentModel;
using Emlid.WindowsIot.Tests.NavioHardwareTestApp.Views.Shared;
using Emlid.WindowsIot.Tests.NavioHardwareTestApp.Views.Tests;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Emlid.WindowsIot.Tests.NavioHardwareTestApp.Views
{
    /// <summary>
    /// Start page of the application.
    /// </summary>
    public sealed partial class StartPage : UIModelPage
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

        #region Properties

        /// <summary>
        /// UI model.
        /// </summary>
        public StartUIModel Model { get; private set; }

        #endregion

        #region Events

        /// <summary>
        /// Initializes the page when it is loaded.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs arguments)
        {
            // Initialize model
            DataContext = Model = new StartUIModel(ApplicationUIModel);
            Model.PropertyChanged += OnModelChanged;

            // Initial layout
            UpdateLayout();

            // Call base class method
            base.OnNavigatedTo(arguments);
        }

        /// <summary>
        /// Cleans-up when navigating away from the page.
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs arguments)
        {
            // Dispose model
            Model?.Dispose();

            // Call base class method
            base.OnNavigatedFrom(arguments);
        }

        /// <summary>
        /// Updates view elements when the model changes and no automatic
        /// method is currently available.
        /// </summary>
        private void OnModelChanged(object sender, PropertyChangedEventArgs arguments)
        {
            switch (arguments.PropertyName)
            {
                case nameof(Model.Model):
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
