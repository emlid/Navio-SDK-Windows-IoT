using Emlid.WindowsIot.Hardware.Protocols.Pwm;
using System;
using Windows.UI.Xaml.Controls;

namespace Emlid.UniversalWindows.UI.Views
{
    /// <summary>
    /// XAML <see cref="PwmPulse"/> editor control.
    /// </summary>
    [CLSCompliant(false)]
    public sealed partial class PwmChannelSlider : UserControl
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        public PwmChannelSlider()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Channel number.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// <see cref="PwmPulse"/> value which this control edits.
        /// </summary>
        public PwmPulse Pulse
        {
            get { return _pulse; }

            set
            {
                // Do nothing when same
                if (value == _pulse)
                    return;

                // Set new value
                var oldValue = _pulse;
                _pulse = value;

                // Set related properties
                SetRange();

                // Update view
                Bindings.Update();

                // Fire changed event
                PulseChanged?.Invoke(this, new ValueChangedEventArgs<PwmPulse>(oldValue, value));
            }
        }
        PwmPulse _pulse;

        /// <summary>
        /// Gets or sets the <see cref="PwmPulse"/>.<see cref="PwmPulse.Width"/>.
        /// </summary>
        public decimal PulseWidth
        {
            get
            {
                // Return overloaded value
                return Pulse.Width;
            }

            set
            {
                // Set overloaded value (struct properties cannot be set individually)
                Pulse = PwmPulse.FromWidth(_pulse.Frequency, value);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets the slider range according to the current <see cref="Pulse"/> value.
        /// </summary>
        private void SetRange()
        {
            // Set range of slider according to channel configuration
            var range = Convert.ToDouble(_pulse.Interval);
            WidthSlider.Maximum = range;
            WidthSlider.SmallChange = range / 100d;
            WidthSlider.LargeChange = range / 10d;
            WidthSlider.StepFrequency = range / 100d;
            WidthSlider.TickFrequency = range / 10d;
        }

        #endregion

        #region Events

        /// <summary>
        /// Fired when the <see cref="Pulse"/> changes.
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<PwmPulse>> PulseChanged;

        #endregion
    }
}
