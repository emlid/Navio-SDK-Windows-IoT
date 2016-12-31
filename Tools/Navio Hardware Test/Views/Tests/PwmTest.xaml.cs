using Emlid.WindowsIot.Tests.NavioHardwareTestApp.Views.Shared;
using System;
using System.ComponentModel;
using System.Globalization;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Emlid.WindowsIot.Tests.NavioHardwareTestApp.Views.Tests
{
    /// <summary>
    /// PWM test page.
    /// </summary>
    public sealed partial class PwmTestPage : UIModelPage
    {
        #region Lifetime

        /// <summary>
        /// Initializes the page.
        /// </summary>
        public PwmTestPage()
        {
            // Initialize view
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// UI model.
        /// </summary>
        public PwmTestUIModel Model { get; private set; }

        #endregion

        #region Events

        /// <summary>
        /// Initializes the page when it is loaded.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs arguments)
        {
            // Initialize model and bind
            DataContext = Model = new PwmTestUIModel(ApplicationUIModel);
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
                case nameof(Model.Output):
                    OutputScroller.UpdateLayout();
                    OutputScroller.ChangeView(null, OutputScroller.ScrollableHeight, null);
                    break;
            }
        }

        /// <summary>
        /// Executes the <see cref="PwmTestUIModel.Read"/> action when the related button is clicked.
        /// </summary>
        private void OnReadButtonClick(object sender, RoutedEventArgs arguments)
        {
            Model.Read();
        }

        /// <summary>
        /// Executes the <see cref="PwmTestUIModel.Sleep"/> action when the related button is clicked.
        /// </summary>
        private void OnSleepButtonClick(object sender, RoutedEventArgs arguments)
        {
            Model.Sleep();
        }

        /// <summary>
        /// Executes the <see cref="PwmTestUIModel.Wake"/> action when the related button is clicked.
        /// </summary>
        private void OnWakeButtonClick(object sender, RoutedEventArgs arguments)
        {
            Model.Wake();
        }

        /// <summary>
        /// Executes the <see cref="PwmTestUIModel.Restart"/> action when the related button is clicked.
        /// </summary>
        private void OnRestartButtonClick(object sender, RoutedEventArgs arguments)
        {
            Model.Restart();
        }

        /// <summary>
        /// Executes the <see cref="PwmTestUIModel.Clear"/> action when the related button is clicked.
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

        /// <summary>
        /// Changes the frequency when the user presses enter and the value has changed.
        /// </summary>
        private void OnFrequencyKeyUp(object sender, KeyRoutedEventArgs arguments)
        {
            if (arguments.Key == VirtualKey.Enter)
                SetFrequency();
        }

        /// <summary>
        /// Changes the frequency when the user moves out of the text box and the value has changed.
        /// </summary>
        private void OnFrequencyLostFocus(object sender, RoutedEventArgs arguments)
        {
            SetFrequency();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets the frequency if the value in the <see cref="FrequencyTextBox"/> has changed.
        /// </summary>
        private void SetFrequency()
        {
            var textBox = FrequencyTextBox;
            var frequency = Convert.ToSingle(textBox.Text, CultureInfo.CurrentCulture);
            if (Model.Device.Frequency != frequency)
                Model.Device.SetFrequency(frequency);
        }

        #endregion
    }
}
