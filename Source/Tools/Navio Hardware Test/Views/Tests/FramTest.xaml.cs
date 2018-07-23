using Emlid.WindowsIot.Tools.NavioHardwareTest.Models;
using Emlid.WindowsIot.Tools.NavioHardwareTest.Models.Tests;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Emlid.WindowsIot.Tools.NavioHardwareTest.Views.Tests
{
    /// <summary>
    /// FRAM test page.
    /// </summary>
    public sealed partial class FramTestPage : FramTestPageBase
    {
        #region Lifetime

        /// <summary>
        /// Initializes the page.
        /// </summary>
        public FramTestPage()
        {
            // Initialize view
            InitializeComponent();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates the page model when it is displayed.
        /// </summary>
        protected override FramTestUIModel CreateModel(TestApplicationUIModel application)
        {
            return new FramTestUIModel(application);
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
        /// Executes the <see cref="FramTestUIModel.Read"/> action when the related button is clicked.
        /// </summary>
        private void OnReadButtonClick(object sender, RoutedEventArgs arguments)
        {
            Model.Read();
        }

        /// <summary>
        /// Executes the <see cref="FramTestUIModel.Erase"/> action when the related button is clicked.
        /// </summary>
        private void OnEraseButtonClick(object sender, RoutedEventArgs arguments)
        {
            Model.Erase();
        }

        /// <summary>
        /// Executes the <see cref="FramTestUIModel.Fill"/> action when the related button is clicked.
        /// </summary>
        private void OnFillButtonClick(object sender, RoutedEventArgs arguments)
        {
            Model.Fill();
        }

        /// <summary>
        /// Executes the <see cref="FramTestUIModel.Sequence"/> action when the related button is clicked.
        /// </summary>
        private void OnSequenceButtonClick(object sender, RoutedEventArgs arguments)
        {
            Model.Sequence();
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