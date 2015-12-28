using System.Collections.Concurrent;
using System.Threading;

namespace Emlid.WindowsIot.Hardware
{
    /// <summary>
    /// PWM decoder for the CPPM protocol.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A complete CPPM frame is about 22.5 ms (can vary between manufacturer). 
    /// Signal low state is about 0.3 ms but sometimes a bit longer (<see cref="PwmLowLimit"/>).
    /// It begins with a start pulse (state high for more than 2 ms <see cref="PwmSyncLengthMinium"/>). 
    /// Each channel (up to 8) is encoded by the time of the high state 
    /// (CPPM high state + 0.3 x (PPM low state) = servo PWM pulse width).
    /// </para>
    /// See https://en.wikipedia.org/wiki/Pulse-position_modulation for more information.
    /// </remarks>
    public class CppmDecoder : IPwmDecoder
    {
        #region Constants

        /// <summary>
        /// Number of channels in a standard CPPM frame.
        /// </summary>
        public const int ChannelCount = 8;

        /// <summary>
        /// Minimum sync (PWM cycle) length in microseconds.
        /// </summary>
        public const int PwmSyncLengthMinium = 4000;

        /// <summary>
        /// Maximum time in microseconds which a PWM signal may be low before it is considered invalid for CPPM.
        /// </summary>
        /// <remarks>
        /// Some specifications state 0.3ms is expected, but 0.4-0.5ms has been observed.
        /// We also add a bit more time for inaccuracies and differences between manufacturers.
        /// </remarks>
        public const int PwmLowLimit = 600;

        #endregion

        #region Fields

        /// <summary>
        /// Indicates the decoder is currently processing a channel (when not null)
        /// and the index of the channel.
        /// </summary>
        private int? _channel;

        /// <summary>
        /// Current CPPM frame being decoded.
        /// </summary>
        private PwmFrame _frame;

        #endregion

        #region Properties

        /// <summary>
        /// Maximum number of channels which this decoder produces.
        /// </summary>
        public int MaximumChannels { get { return ChannelCount; } }

        #endregion

        #region Methods

        private PwmCycle _cycle;

        /// <summary>
        /// Runs the decoder thread.
        /// </summary>
        /// <param name="inputBuffer">Buffer from which new PWM values are read.</param>
        /// <param name="inputTrigger">Trigger which is fired by the caller when new data arrives.</param>
        /// <param name="outputBuffer">Buffer into which decoded PWM frames are written.</param>
        /// <param name="outputTrigger">Trigger which is fired by this decoder when new data has been decoded.</param>
        /// <param name="stop">Signals when the decoder should stop.</param>
        public void Decode(ConcurrentQueue<PwmValue> inputBuffer, AutoResetEvent inputTrigger, 
            ConcurrentQueue<PwmFrame> outputBuffer, AutoResetEvent outputTrigger, CancellationToken stop)
        {
            // Decode until stopped...
            while (!stop.IsCancellationRequested)
            {
                // Wait for value in queue...
                PwmValue value;
                if (!inputBuffer.TryDequeue(out value))
                {
                    inputTrigger.WaitOne(PwmLowLimit);
                    continue;
                }

                // Decode values into cycles...
                var time = value.Time;
                if (value.Level)
                {
                    // Low -> high (mid-cycle)
                    _cycle.LowLength = time - _cycle.LowTime;

                    // Prepare next value
                    _cycle.HighTime = time;
                    _cycle.HighLength = 0;
                }
                else
                {
                    // High -> low (end of cycle)
                    _cycle.HighLength = time - _cycle.HighTime;
                    var cycle = _cycle;

                    // Prepare next value
                    _cycle.LowTime = time;
                    _cycle.LowLength = 0;

                    // Decode cycles into frames...
                    var frame = Decode(cycle);
                    if (frame != null)
                    {
                        // Output frame when decoding complete
                        outputBuffer.Enqueue(frame);
                        outputTrigger.Set();
                    }
                }
            }
        }

        /// <summary>
        /// Decodes the incoming PWM signal (each complete cycle) using the CPPM protocol.
        /// </summary>
        /// <param name="cycle">PWM cycle to decode.</param>
        /// <returns>
        /// <see cref="PwmFrame"/> when complete else null whilst decoding or skipping invalid cycles.
        /// </returns>
        private PwmFrame Decode(PwmCycle cycle)
        {
            // Validate
            // TODO: This dirty filter is necessary because the user mode GPIO updates are erratic and have too much latency.
            // There appears to be a problem with the Microsoft IoT build and GPIO4 and/or changing drive modes.
            // In fact the Navio pin 4 is not officially supported right now!!!
            if (cycle.LowLength >= PwmLowLimit || cycle.HighLength <= PwmLowLimit)
            {
                // Discard frame
                _channel = null;
                return null;
            }

            // Detect start frame
            if (cycle.HighLength >= PwmSyncLengthMinium)
            {
                // Start decoding from channel 0 at next pulse
                _channel = 0;
                _frame = new PwmFrame(cycle.LowTime, new int[ChannelCount]);
                return null;
            }

            // Do nothing when not decoding
            if (!_channel.HasValue)
                return null;
            var decodeIndex = _channel.Value;

            // Store channel value whilst decoding
            if (decodeIndex < ChannelCount)
            {
                // Store channel value
                _frame.Channels[decodeIndex] = (int)cycle.Length;

                // Wait for next channel...
                _channel = decodeIndex + 1;

                // Complete frame when all channels decoded...
                if (decodeIndex == ChannelCount - 1)
                {
                    var frame = _frame;
                    _frame = null;
                    return frame;
                }
            }

            // Continue...
            return null;
        }

        #endregion
    }
}
