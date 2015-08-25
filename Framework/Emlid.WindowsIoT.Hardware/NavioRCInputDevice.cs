using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.Devices.Gpio;

namespace Emlid.WindowsIoT.Hardware
{
    /// <summary>
    /// Navio Remote Control input hardware device.
    /// </summary>
    /// <remarks>
    /// Navio provides RC (receiver) input via a connector on it's servo rail mapped to GPIO pin 4.
    /// Navio+ has a logic level converter and you can connect receivers which generate both 3.3V and 5V signals.
    /// The older Navio model only has a built-in voltage divider in PPM Input that lowers the voltage level from 5V to 3.3V.
    /// So if you connect a 3.3V PPM device (which is rare) to the original Navio, no signal will not be detected. 
    /// </remarks>
    public class NavioRCInputDevice : IDisposable
    {
        #region Constants

        /// <summary>
        /// GPIO pin number which is mapped to the RC input connector.
        /// </summary>
        public const int InputGpioPin = 4;

        /// <summary>
        /// Number of decimal places to which raw PWM values (fractions of a millisecond) are rounded
        /// before storing as channel values.
        /// </summary>
        /// <remarks>
        /// The default value of 3 rounds to the nearest microsecond.
        /// </remarks>
        public const int PwmChannelAccuracy = 3;

        /// <summary>
        /// Number of channels in a PPM frame.
        /// </summary>
        public const int PpmChannelCount = 8;

        /// <summary>
        /// PPM frame start delay in milliseconds.
        /// </summary>
        /// <remarks>
        /// Actual value is 2ms but we add some time for inaccuracy and differences between manufacturers.
        /// </remarks>
        public const double PpmStartLength = 2.5;

        /// <summary>
        /// Maximum time in milliseconds which a PPM signal may be low before it is considered invalid.
        /// </summary>
        /// <remarks>
        /// Actual specification is 0.3ms but we add some time for inaccuracy and differences between manufacturers.
        /// </remarks>
        public const double PpmLowLimit = 0.5;

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates and initializes an instance.
        /// </summary>
        public NavioRCInputDevice()
        {
            // Initialize
            _timer = new Stopwatch();
            _inputPin = GpioController.GetDefault().OpenPin(InputGpioPin);
            _ppmFrame = new double[8];
            _channels = new double[8];
            Channels = new ReadOnlyCollection<double>(_channels);

            // PPM only in current implementation
            Mode = NavioRCInputMode.PPM;

            // Configure GPIO
            if (_inputPin.GetDriveMode() != GpioPinDriveMode.Input)
                _inputPin.SetDriveMode(GpioPinDriveMode.Input);
            _inputPin.ValueChanged += OnInputPinValueChanged;
        }

        #region IDisposable

        /// <summary>
        /// Indicates this instance has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Frees resources owned by this instance.
        /// </summary>
        /// <param name="disposing">
        /// True when called via <see cref="Dispose()"/>, false when called from the finalizer.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            // Do nothing when already disposed
            if (IsDisposed) return;

            // Dispose
            try
            {
                // Dispose managed resource during dispose
                if (disposing)
                {
                    _inputPin.Dispose();
                    _timer.Stop();
                }
            }
            finally
            {
                // Flag disposed
                IsDisposed = true;
            }
        }

        /// <summary>
        /// Finalizer which calls <see cref="Dispose(bool)"/> with false when it has not been disabled
        /// by a proactive call to <see cref="Dispose()"/>.
        /// </summary>
        ~NavioRCInputDevice()
        {
            // Partial dispose
            Dispose(false);
        }

        /// <summary>
        /// Proactively frees resources owned by this instance.
        /// </summary>
        public void Dispose()
        {
            try
            {
                // Full managed dispose
                Dispose(true);
            }
            finally
            {
                // Suppress finalizer (we already cleaned-up)
                GC.SuppressFinalize(this);
            }
        }

        #endregion

        #endregion

        #region Private Fields

        /// <summary>
        /// GPIO RC input pin.
        /// </summary>
        private GpioPin _inputPin;

        /// <summary>
        /// High resolution timer used to measure PWM duty cycle.
        /// </summary>
        private Stopwatch _timer;

        /// <summary>
        /// Indicates the decoder is currently processing a channel (when not null)
        /// and the index of the channel.
        /// </summary>
        private int? _decodeChannel;

        /// <summary>
        /// Current PPM frame.
        /// </summary>
        private double[] _ppmFrame;

        #endregion

        #region Properties

        /// <summary>
        /// Input mode detected by the decoder.
        /// </summary>
        public NavioRCInputMode Mode { get; protected set; }

        /// <summary>
        /// Channel values in fractions of a millisecond, rounded to <see cref="PwmChannelAccuracy"/>
        /// </summary>
        public ReadOnlyCollection<double> Channels { get; private set; }
        private double[] _channels;

        #endregion

        #region Protected Methods

        /// <summary>
        /// Decodes the incoming PWM signal and updates properties.
        /// </summary>
        /// <param name="value">PWM value. True when high, false when low.</param>
        /// <param name="duty">PWM duty, the time since last transition.</param>
        protected virtual void Decode(bool value, TimeSpan duty)
        {
            // TODO: Automatic mode detection.
            // TODO: Extract decoders into external classes and add generic interface
            switch (Mode)
            {
                case NavioRCInputMode.PPM:
                    DecodePpm(value, duty);
                    break;

                case NavioRCInputMode.SBUS:
                    DecodeSbus(value, duty);
                    break;
            }
        }

        /// <summary>
        /// Decodes the incoming PWM signal using the PPM protocol.
        /// </summary>
        /// <param name="value">PWM value. True when high, false when low.</param>
        /// <param name="duty">PWM duty, the time since last transition.</param>
        protected virtual void DecodePpm(bool value, TimeSpan duty)
        {
            if (value)
            {
                // Detect start frame
                if (duty.TotalMilliseconds >= PpmStartLength)
                {
                    // Start decoding from channel 0 at next pulse
                    _decodeChannel = 0;
                    return;
                }

                // Do nothing when not decoding
                if (!_decodeChannel.HasValue)
                    return;
                var decodeIndex = _decodeChannel.Value;

                // Store channel value whilst decoding
                if (decodeIndex < PpmChannelCount - 1)
                {
                    // Store channel value
                    _ppmFrame[decodeIndex] = duty.TotalMilliseconds;

                    // Wait for next channel...
                    _decodeChannel = decodeIndex + 1;
                    return;
                }

                // Complete frame when all channels decoded...

                // Copy frame to stack (in case we take too long to update)
                var frame = new double[PpmChannelCount];
                Array.Copy(_ppmFrame, frame, PpmChannelCount);

                // Stop decoding (until next valid start)
                _decodeChannel = null;

                // Update values (with automatic change detection)
                for (var index = 0; index < _ppmFrame.Length; index++)
                {
                    // Round value to prevent unwanted change detection
                    var rawValue = _ppmFrame[index];
                    var roundValue = Math.Round(rawValue, PwmChannelAccuracy);

                    // Write value (detecting any change in setter)
                    _channels[index] = roundValue;
                    Debug.Write(String.Format("RC{0}={1} ", index + 1, roundValue));
                }
                Debug.WriteLine("");
            }
            else
            {
                // Detect lost signal or invalid frame
                if (duty.TotalMilliseconds >= PpmLowLimit)
                {
                    // Discard frame and stop decoding (until next valid start)
                    _decodeChannel = null;
                    return;
                }
            }
        }

        /// <summary>
        /// Decodes the incoming PWM signal using the SBUS protocol.
        /// </summary>
        /// <param name="value">PWM value. True when high, false when low.</param>
        /// <param name="duty">PWM duty, the time since last transition.</param>
        protected virtual void DecodeSbus(bool value, TimeSpan duty)
        {
            // TODO: Implement SBUS protocol decoder
            throw new NotImplementedException();
        }

        #endregion

        #region Events

        /// <summary>
        /// Handles GPIO changes (rising and falling PWM signal), calculates duty cycle (time between change).
        /// Main hardware routine which triggers the input translation process.
        /// </summary>
        private void OnInputPinValueChanged(GpioPin sender, GpioPinValueChangedEventArgs arguments)
        {
            // Get PWM duty and restart timer as quickly as possible (to be accurate)
            var duty = _timer.Elapsed;
            _timer.Restart();

            // Do nothing at first start
            if (duty == TimeSpan.Zero)
                return;

            // Get PWM value before transition (high means we have low time, and vice versa)
            var value = arguments.Edge != GpioPinEdge.RisingEdge;

            // Call decoder
            Decode(value, duty);
        }

        #endregion
    }
}
