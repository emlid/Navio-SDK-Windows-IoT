using Emlid.WindowsIoT.Tests.NavioHardwareTestApp.Views.Tests;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Emlid.WindowsIoT.Tests.NavioHardwareTestApp.Views
{
    /// <summary>
    /// Start page of the application.
    /// </summary>
    public sealed partial class StartPage : Page
    {
        #region Lifetime

        /// <summary>
        /// Creates the page.
        /// </summary>
        public StartPage()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        /// <summary>
        /// Navigates to the <see cref="LedPwmTestPage"/> when the corresponding button is clicked.
        /// </summary>
        private void OnLedPwmTestButtonClick(object sender, RoutedEventArgs arguments)
        {
            Frame.Navigate(typeof(LedPwmTestPage));
        }

        /// <summary>
        /// Navigates to the <see cref="RCInputTestPage"/> when the corresponding button is clicked.
        /// </summary>
        private void OnRCInputTestButtonClick(object sender, RoutedEventArgs arguments)
        {
            Frame.Navigate(typeof(RCInputTestPage));
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
