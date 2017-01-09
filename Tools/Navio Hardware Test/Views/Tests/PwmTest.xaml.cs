using Emlid.WindowsIot.Hardware.Protocols.Pwm;
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
            DataContext = Model = new PwmTestUIModel(ApplicationModel);
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
        /// Executes the <see cref="PwmTestUIModel.Reset"/> action when the related button is clicked.
        /// </summary>
        private void OnResetButtonClick(object sender, RoutedEventArgs arguments)
        {
            Model.Reset();
        }

        /// <summary>
        /// Executes the <see cref="PwmTestUIModel.Read"/> action when the related button is clicked.
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

        /// <summary>
        /// Changes the channel value when it's value is changed in the UI.
        /// </summary>
        private void OnChannelChanged(object sender, ValueChangedEventArgs<PwmPulse> value)
        {
            var slider = (PwmChannelSlider)sender;
            Model.Device.SetChannel(slider.Number, value.NewValue);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets the frequency if the value in the <see cref="FrequencyTextBox"/> has changed.
        /// </summary>
        private void SetFrequency()
        {
            // Get frequency text from input
            var frequencyText = FrequencyTextBox.Text;

            // Reset value when invalid
            int frequency;
            if (!int.TryParse(frequencyText, out frequency) ||
                frequency < Model.Device.FrequencyMinimum ||
                frequency > Model.Device.FrequencyMaximum)
            {
                FrequencyTextBox.Text = Model.Device.Frequency.ToString(CultureInfo.CurrentCulture);
                return;
            }

            // Do nothing when same
            if (frequency == Model.Device.Frequency)
                return;

            // Set new frequency
            Model.Device.Frequency = frequency;

            // Update bindings
            Bindings.Update();
        }

        #endregion
    }
}
