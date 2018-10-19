using CodeForDotNet;
using CodeForDotNet.Diagnostics;
using Emlid.WindowsIot.Hardware.Protocols.Ppm;
using Emlid.WindowsIot.HardwarePlus.Buses;
using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace Emlid.WindowsIot.Hardware.Boards.Navio.Internal
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
    public sealed class Navio1RCInputDevice : DisposableObject, INavioRCInputDevice
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

        #endregion Constants

        #region Lifetime

        /// <summary>
        /// Creates and initializes an instance.
        /// </summary>
        public Navio1RCInputDevice()
        {
            try
            {
                // Initialize buffers
                _pulseBuffer = new ConcurrentQueue<PpmPulse>();
                _pulseTrigger = new AutoResetEvent(false);
                _frameBuffer = new ConcurrentQueue<PpmFrame>();
                _frameTrigger = new AutoResetEvent(false);

                // Configure GPIO
                _inputPin = GpioExtensions.Connect(GpioControllerIndex, GpioInputPinNumber, GpioPinDriveMode.Input, GpioSharingMode.Exclusive);
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
                _decoderTask = Task.Factory.StartNew(() => { _decoder.DecodePulse(_pulseBuffer, _pulseTrigger, _frameBuffer, _frameTrigger, _stop.Token); },
                    CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);

                // Create receiver thread
                _receiverTask = Task.Factory.StartNew(() => { Receiver(); },
                    CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);

                // Hook events
                _inputPin.ValueChanged += OnInputPinValueChanged;

                // Start buffered event handling
                // TODO: Support buffered GPIO when possible
                //_inputPin.CreateInterruptBuffer();
                //_inputPin.StartInterruptBuffer();
                //_inputPin.StopInterruptCount();
            }
            catch
            {
                // Close device in case partially initialized
                _inputPin?.Dispose();

                // Continue error
                throw;
            }
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

            // Unhook events
            _inputPin.ValueChanged -= OnInputPinValueChanged;

            // Stop interrupts
            // TODO: Support buffered GPIO when possible
            //_inputPin.StopInterruptCount();
            //_inputPin.StopInterruptBuffer();

            // Stop background tasks
            if (_stop != null)
            {
                _stop.Cancel();
                _stop.Dispose();
            }

            // Stop events
            _pulseTrigger?.Dispose();
            _frameTrigger?.Dispose();

            //  Close device
            _inputPin?.Dispose();
        }

        #endregion IDisposable

        #endregion Lifetime

        #region Private Fields

        /// <summary>
        /// GPIO RC input pin.
        /// </summary>
        private readonly GpioPin _inputPin;

        /// <summary>
        /// Decoder called when each PPM cycle is detected.
        /// </summary>
        private readonly IPpmDecoder _decoder;

        /// <summary>
        /// Background decoder task.
        /// </summary>
        private readonly Task _decoderTask;

        /// <summary>
        /// Background receiver task.
        /// </summary>
        private readonly Task _receiverTask;

        /// <summary>
        /// Cancellation token used to signal worker threads to stop.
        /// </summary>
        private readonly CancellationTokenSource _stop;

        /// <summary>
        /// Buffer containing raw PPM pulses.
        /// </summary>
        private readonly ConcurrentQueue<PpmPulse> _pulseBuffer;

        /// <summary>
        /// Event used to signal the decoder that new captured PPM values are waiting to decode.
        /// </summary>
        private readonly AutoResetEvent _pulseTrigger;

        /// <summary>
        /// Buffer containing decoded PPM frames.
        /// </summary>
        private readonly ConcurrentQueue<PpmFrame> _frameBuffer;

        /// <summary>
        /// Event used to signal the consumer that new decoded PPM frames are ready to use.
        /// </summary>
        private readonly AutoResetEvent _frameTrigger;

        #endregion Private Fields

        #region Properties

        /// <summary>
        /// Channel values in microseconds.
        /// </summary>
        public ReadOnlyCollection<int> Channels { get; private set; }

        private int[] _channels;

        /// <summary>
        /// Returns false because multiple protocols are not supported, only CPPM.
        /// </summary>
        public bool Multiprotocol { get { return false; } }

        #endregion Properties

        #region Events

        /// <summary>
        /// Handles GPIO changes (rising and falling PPM signal), recording them to the decoder queue.
        /// </summary>
        /// <param name="sender">Event source, the <see cref="GpioPin"/> which changed.</param>
        /// <param name="arguments">Information about the GPIO pin value change.</param>
        /// <remarks>
        /// Main hardware routine which triggers the input translation process.
        /// This code must run as quickly as possible else we could miss the next event!
        /// </remarks>
        private void OnInputPinValueChanged(GpioPin sender, GpioPinValueChangedEventArgs arguments)
        {
            // Get PPM value
            var time = StopwatchExtensions.GetTimestampInMicroseconds();
            var level = arguments.Edge == GpioPinEdge.RisingEdge;
            var value = new PpmPulse(time, level);

            // Queue for processing
            _pulseBuffer.Enqueue(value);
            _pulseTrigger.Set();
        }

        /// <summary>
        /// Fired after a new frame of data has been received and decoded into <see cref="Channels"/>.
        /// </summary>
        public event EventHandler<PpmFrame> ChannelsChanged;

        #endregion Events

        #region Private Methods

        /// <summary>
        /// Waits for decoded frames, updates the <see cref="Channels"/> property and fires
        /// the <see cref="ChannelsChanged"/> event on a separate thread.
        /// </summary>
        private void Receiver()
        {
            while (!_stop.IsCancellationRequested)
            {
                // Run until stopped...
                // Wait for frame
                if (!_frameBuffer.TryDequeue(out PpmFrame frame))
                {
                    _frameTrigger.WaitOne(1000);
                    continue;
                }

                // Validate
                var channelCount = frame.Channels.Count;
                if (channelCount > _channels.Length)
                {
                    // Too many channels
                    Debug.WriteLine(Resources.Strings.NavioRCInputDecoderChannelOverflow, channelCount, _channels.Length);
                    continue;
                }

                // Copy new channel data
                for (var index = 0; index < channelCount; index++)
                    _channels[index] = frame.Channels[index];

                // Fire event
                ChannelsChanged?.Invoke(this, frame);
            }
        }

        #endregion Private Methods
    }
}