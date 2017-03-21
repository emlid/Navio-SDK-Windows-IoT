using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Emlid.WindowsIot.Hardware.Protocols.Ppm
{
    /// <summary>
    /// PPM decoder for the CPPM protocol.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A complete CPPM frame is about 22.5 ms (can vary between manufacturer).
    /// Signal low state is about 0.3 ms but sometimes a bit longer (<see cref="LowLimit"/>).
    /// It begins with a start pulse (state high for more than 2 ms <see cref="SyncLengthMinimum"/>).
    /// Each channel (up to 8) is encoded by the time of the high state
    /// (CPPM high state + 0.3 x (PPM low state) = servo PPM pulse width).
    /// </para>
    /// See https://en.wikipedia.org/wiki/Pulse-position_modulation for more information.
    /// </remarks>
    public class CppmDecoder : IPpmDecoder
    {
        #region Constants

        /// <summary>
        /// Number of channels in a standard CPPM frame.
        /// </summary>
        public const int ChannelCount = 8;

        /// <summary>
        /// Minimum sync (PPM cycle) length in microseconds.
        /// </summary>
        public const int SyncLengthMinimum = 4000;

        /// <summary>
        /// Maximum time in microseconds which a PPM signal may be low before it is considered invalid for CPPM.
        /// </summary>
        /// <remarks>
        /// Some specifications state 0.3ms is expected, but 0.4-0.5ms has been observed.
        /// We also add a bit more time for inaccuracies and differences between manufacturers.
        /// </remarks>
        public const int LowLimit = 600;

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        public CppmDecoder()
        {
            _frame = new PpmFrame();
            _cycle = new PpmCycle();
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Indicates the decoder is currently processing a channel (when not null)
        /// and the index of the channel.
        /// </summary>
        private int? _channel;

        /// <summary>
        /// Current CPPM frame being decoded.
        /// </summary>
        private PpmFrame _frame;

        /// <summary>
        /// Current CPPM cycle being decoded.
        /// </summary>
        private PpmCycle _cycle;

        #endregion

        #region Properties

        /// <summary>
        /// Maximum number of channels which this decoder produces.
        /// </summary>
        public int MaximumChannels { get { return ChannelCount; } }

        #endregion

        #region Methods

        /// <summary>
        /// Runs the decoder thread.
        /// </summary>
        /// <param name="inputBuffer">Buffer from which new PPM values are read.</param>
        /// <param name="inputTrigger">Trigger which is fired by the caller when new data arrives.</param>
        /// <param name="outputBuffer">Buffer into which decoded PPM frames are written.</param>
        /// <param name="outputTrigger">Trigger which is fired by this decoder when new data has been decoded.</param>
        /// <param name="stop">Signals when the decoder should stop.</param>
        public void DecodePulse(ConcurrentQueue<PpmPulse> inputBuffer, AutoResetEvent inputTrigger,
            ConcurrentQueue<PpmFrame> outputBuffer, AutoResetEvent outputTrigger, CancellationToken stop)
        {
            // Validate
            if (inputBuffer == null) throw new ArgumentNullException(nameof(inputBuffer));
            if (inputTrigger == null) throw new ArgumentNullException(nameof(inputTrigger));
            if (outputBuffer == null) throw new ArgumentNullException(nameof(outputBuffer));
            if (outputTrigger == null) throw new ArgumentNullException(nameof(outputTrigger));
            
            // Decode until stopped...
            while (!stop.IsCancellationRequested)
            {
                // Wait for value in queue...
                if (!inputBuffer.TryDequeue(out PpmPulse value))
                {
                    inputTrigger.WaitOne(SyncLengthMinimum * 2);
                    continue;
                }

                // Decode values into cycles...
                var time = value.Time;
                if (value.Level)
                {
                    // Low -> high (mid-cycle)
                    _cycle.HighTime = time;
                    _cycle.LowLength = time - _cycle.LowTime;
                }
                else
                {
                    // High -> low (end of cycle)
                    _cycle.HighLength = time - _cycle.HighTime;

                    // Prepare next value
                    var cycle = _cycle;
                    _cycle = new PpmCycle(time);

                    // Decode cycles into frames...
                    var frame = DecodeCycle(cycle);
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
        /// Decodes the incoming PPM signal (each complete cycle) using the CPPM protocol.
        /// </summary>
        /// <param name="cycle">PPM cycle to decode.</param>
        /// <returns>
        /// <see cref="PpmFrame"/> when complete else null whilst decoding or skipping invalid cycles.
        /// </returns>
        private PpmFrame DecodeCycle(PpmCycle cycle)
        {
            // Validate
            if (!cycle.IsValid() || cycle.LowLength >= LowLimit)
            {
                // Discard frame
                _channel = null;
                return null;
            }

            // Detect start frame
            if (cycle.HighLength >= SyncLengthMinimum)
            {
                // Start decoding from channel 0 at next pulse
                _channel = 0;
                _frame = new PpmFrame(cycle.LowTime, new int[ChannelCount]);
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
