using Emlid.WindowsIot.Tools.NavioHardwareTest.Models;
using Emlid.WindowsIot.Tools.NavioHardwareTest.Models.Tests;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Emlid.WindowsIot.Tools.NavioHardwareTest.Views.Tests
{
    /// <summary>
    /// Main page.
    /// </summary>
    public sealed partial class RCInputTestPage : RCInputTestPageBase
    {
        #region Lifetime

        /// <summary>
        /// Initializes the page.
        /// </summary>
        public RCInputTestPage()
        {
            // Initialize view
            InitializeComponent();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates the page model when it is displayed.
        /// </summary>
        protected override RCInputTestUIModel CreateModel(TestApplicationUIModel application)
        {
            return new RCInputTestUIModel(application);
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
                case nameof(Model.Output):
                    OutputScroller.UpdateLayout();
                    OutputScroller.ChangeView(null, OutputScroller.ScrollableHeight, null);
                    break;
            }
        }

        /// <summary>
        /// Executes the <see cref="TestUIModel.Clear"/> when the related button is clicked.
        /// </summary>
        private void OnClearButtonClick(object sender, RoutedEventArgs arguments)
        {
            Model.Clear();
        }

        /// <summary>
        /// Returns to the previous page when the close button is clicked.
        /// </summary>
        private void OnCloseButtonClick(object sender, RoutedEventArgs arguments)
        {
            Frame.GoBack();
        }

        #endregion
    }
}