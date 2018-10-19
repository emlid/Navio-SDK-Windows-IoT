using Emlid.WindowsIot.Hardware.Boards.Navio;
using Emlid.WindowsIot.Tools.NavioHardwareTest.Models;
using Emlid.WindowsIot.Tools.NavioHardwareTest.Models.Tests;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Emlid.WindowsIot.Tools.NavioHardwareTest.Views.Tests
{
    /// <summary>
    /// LED test page.
    /// </summary>
    public sealed partial class LedTestPage : LedTestPageBase
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

        #endregion Lifetime

        #region Protected Methods

        /// <summary>
        /// Creates the page model when it is displayed.
        /// </summary>
        protected override LedTestUIModel CreateModel(TestApplicationUIModel application)
        {
            return new LedTestUIModel(application);
        }

        #endregion Protected Methods

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

            // Set slider range
            SetRange(LedRedSlider);
            SetRange(LedGreenSlider);
            SetRange(LedBlueSlider);

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
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Event handlers must not use all parameters.")]
        private void OnResetButtonClick(object sender, RoutedEventArgs arguments)
        {
            Model.Reset();
        }

        /// <summary>
        /// Executes the <see cref="LedTestUIModel.Read"/> action when the related button is clicked.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Event handlers must not use all parameters.")]
        private void OnReadButtonClick(object sender, RoutedEventArgs arguments)
        {
            Model.Read();
        }

        /// <summary>
        /// Executes the <see cref="TestUIModel.Clear"/> action when the related button is clicked.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Event handlers must not use all parameters.")]
        private void OnClearButtonClick(object sender, RoutedEventArgs arguments)
        {
            Model.Clear();
        }

        /// <summary>
        /// Returns to the previous page when the close button is clicked.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Event handlers must not use all parameters.")]
        private void OnCloseButtonClick(object sender, RoutedEventArgs arguments)
        {
            Frame.GoBack();
        }

        #endregion Events

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

        #endregion Private Methods
    }
}