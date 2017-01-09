using Emlid.WindowsIot.Hardware.Boards.Navio;
using Emlid.WindowsIot.Tests.NavioHardwareTestApp.Views.Shared;
using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Emlid.WindowsIot.Tests.NavioHardwareTestApp.Views.Tests
{
    /// <summary>
    /// LED test page.
    /// </summary>
    public sealed partial class LedTestPage : UIModelPage
    {
        #region Lifetime

        /// <summary>
        /// Initializes the page.
        /// </summary>
        public LedTestPage()
        {
            // Initialize view
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// UI model.
        /// </summary>
        public LedTestUIModel Model { get; private set; }

        #endregion

        #region Events

        /// <summary>
        /// Initializes the page when it is loaded.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs arguments)
        {
            // Initialize model and bind
            DataContext = Model = new LedTestUIModel(ApplicationModel);
            Model.PropertyChanged += OnModelChanged;

            // Set slider range
            SetRange(LedRedSlider);
            SetRange(LedGreenSlider);
            SetRange(LedBlueSlider);

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
                case nameof(Model.Device):
                    Bindings.Update();
                    break;

                case nameof(Model.Output):
                    OutputScroller.UpdateLayout();
                    OutputScroller.ChangeView(null, OutputScroller.ScrollableHeight, null);
                    break;
            }
        }

        /// <summary>
        /// Executes the <see cref="LedTestUIModel.Reset"/> action when the related button is clicked.
        /// </summary>
        private void OnResetButtonClick(object sender, RoutedEventArgs arguments)
        {
            Model.Reset();
        }

        /// <summary>
        /// Executes the <see cref="LedTestUIModel.Read"/> action when the related button is clicked.
        /// </summary>
        private void OnReadButtonClick(object sender, RoutedEventArgs arguments)
        {
            Model.Read();
        }

        /// <summary>
        /// Executes the <see cref="TestUIModel.Clear"/> action when the related button is clicked.
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

        #region Private Methods

        /// <summary>
        /// Sets the slider range according to the <see cref="INavioLedDevice.MaximumValue"/>.
        /// </summary>
        private void SetRange(Slider slider)
        {
            // Get range supported by device
            var range = Convert.ToDouble(Model.Device.MaximumValue);
            slider.Maximum = range;

            // Set steps and small change to 100th of the range
            var small = (int)(range / 100d);
            if (small < 1) small = 1;
            slider.SmallChange = small;
            slider.StepFrequency = small;

            // Set ticks and large change to 10th of the range
            var large = (int)(range / 10d);
            if (large < 1) large = 1;
            slider.LargeChange = large;
            slider.TickFrequency = large;
        }

        #endregion
    }
}
