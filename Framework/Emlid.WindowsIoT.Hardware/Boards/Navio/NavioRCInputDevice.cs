using Emlid.WindowsIot.Common;
using Emlid.WindowsIot.Hardware.Protocols.Pwm;
using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Emlid.WindowsIot.Hardware.Boards.Navio
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
    public class NavioRCInputDevice : DisposableObject
    {
        #region Constants

        /// <summary>
        /// GPIO controller index of the chip on the Navio board.
        /// </summary>
        public const int GpioControllerIndex = 0;

        /// <summary>
        /// GPIO pin number which is mapped to the RC input connector.
        /// </summary>
        public const int GpioInputPinNumber = 4;

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates and initializes an instance.
        /// </summary>
        public NavioRCInputDevice()
        {
            // Initialize buffers
            _valueBuffer = new ConcurrentQueue<PwmValue>();
            _valueTrigger = new AutoResetEvent(false);
            _frameBuffer = new ConcurrentQueue<PwmFrame>();
            _frameTrigger = new AutoResetEvent(false);

            // Configure GPIO
            _inputPin = NavioHardwareProvider.ConnectGpio(0, GpioInputPinNumber, GpioPinDriveMode.Input, exclusive: true);
            if (_inputPin == null)
            {
                // Initialization error
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                    Resources.Strings.GpioErrorDeviceNotFound, GpioInputPinNumber, GpioControllerIndex));
            }
            if (_inputPin.DebounceTimeout != TimeSpan.Zero)
                _inputPin.DebounceTimeout = TimeSpan.Zero;

            // Create decoder thread (CPPM only to start with, SBus desired)
            _decoder = new CppmDecoder();
            _stop = new CancellationTokenSource();
            _channels = new int[_decoder.MaximumChannels];
            Channels = new ReadOnlyCollection<int>(_channels);
            _decoderTask = Task.Factory.StartNew(() => { _decoder.DecodePulse(_valueBuffer, _valueTrigger, _frameBuffer, _frameTrigger, _stop.Token); });

            // Create receiver thread
            _receiverTask = Task.Factory.StartNew(() => { Receiver(); });

            // Hook events
            _inputPin.ValueChanged += OnInputPinValueChanged;
        }

        #region IDisposable

        /// <summary>
        /// Frees resources owned by this instance.
        /// </summary>
        /// <param name="disposing">
        /// True when called via <see cref="IDisposable.Dispose()"/>, false when called from the finalizer.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            // Only managed resources to dispose
            if (!disposing)
                return;

            // Un-hook events
            _inputPin.ValueChanged -= OnInputPinValueChanged;

            // Stop background tasks
            _stop?.Cancel();

            // Stop events
            _valueTrigger?.Dispose();
            _frameTrigger?.Dispose();

            //  Close device
            _inputPin?.Dispose();
        }

        #endregion

        #endregion

        #region Private Fields

        /// <summary>
        /// GPIO RC input pin.
        /// </summary>
        private GpioPin _inputPin;

        /// <summary>
        /// Decoder called when each PWM cycle is detected.
        /// </summary>
        private IPwmDecoder _decoder;

        /// <summary>
        /// Background decoder task.
        /// </summary>
        private Task _decoderTask;

        /// <summary>
        /// Background receiver task.
        /// </summary>
        private Task _receiverTask;

        /// <summary>
        /// Cancellation token used to signal worker threads to stop.
        /// </summary>
        private CancellationTokenSource _stop;

        /// <summary>
        /// Buffer containing raw PWM values.
        /// </summary>
        private ConcurrentQueue<PwmValue> _valueBuffer;

        /// <summary>
        /// Event used to signal the decoder that new captured PWM values are waiting to decode.
        /// </summary>
        private AutoResetEvent _valueTrigger;

        /// <summary>
        /// Buffer containing decoded PWM frames.
        /// </summary>
        private ConcurrentQueue<PwmFrame> _frameBuffer;

        /// <summary>
        /// Event used to signal the consumer that new decoded PWM frames are ready to use.
        /// </summary>
        private AutoResetEvent _frameTrigger;

        #endregion

        #region Properties

        /// <summary>
        /// Channel values in microseconds.
        /// </summary>
        public ReadOnlyCollection<int> Channels { get; private set; }
        private int[] _channels;

        /// <summary>
        /// Used to wait until the device is stopped.
        /// </summary>
        public WaitHandle Stopped { get { return _stop.Token.WaitHandle; } }

        #endregion

        #region Events

        /// <summary>
        /// Handles GPIO changes (rising and falling PWM signal), recording them to the decoder queue.
        /// </summary>
        /// <remarks>
        /// Main hardware routine which triggers the input translation process.
        /// This code must run as quickly as possible else we could miss the next event!
        /// </remarks>
        private void OnInputPinValueChanged(GpioPin sender, GpioPinValueChangedEventArgs arguments)
        {
            // Get PWM value
            var time = StopwatchExtensions.GetTimestampInMicroseconds();
            var level = arguments.Edge == GpioPinEdge.RisingEdge;
            var value = new PwmValue(time, level);

            // Queue for processing
            _valueBuffer.Enqueue(value);
            _valueTrigger.Set();
        }

        /// <summary>
        /// Fired after a new frame of data has been received and decoded into <see cref="Channels"/>.
        /// </summary>
        public EventHandler<PwmFrame> ChannelsChanged;

        #endregion

        #region Private Methods

        /// <summary>
        /// Waits for decoded frames, updates the <see cref="Channels"/> property and fires
        /// the <see cref="ChannelsChanged"/> event on a separate thread.
        /// </summary>
        private void Receiver()
        {
            // Run until stopped...
            while (!_stop.IsCancellationRequested)
            {
                // Wait for frame
                PwmFrame frame;
                if (!_frameBuffer.TryDequeue(out frame))
                {
                    _frameTrigger.WaitOne(1000);
                    continue;
                }

                // Validate
                var channelCount = frame.Channels.Length;
                if (channelCount > _channels.Length)
                {
                    // Too many channels
                    Debug.WriteLine(Resources.Strings.NavioRCInputDecoderChannelOverflow, channelCount, _channels.Length);
                    continue;
                }

                // Copy new channel data
                Array.Copy(frame.Channels, _channels, channelCount);

                // Fire event
                ChannelsChanged?.Invoke(this, frame);
            }
        }

        #endregion
    }
}
