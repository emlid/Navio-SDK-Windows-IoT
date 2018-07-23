using System.Collections.Concurrent;
using System.Threading;

namespace Emlid.WindowsIot.Hardware.Protocols.Ppm
{
    /// <summary>
    /// Defines a common PPM decoder interface to support multiple protocols.
    /// </summary>
    public interface IPpmDecoder
    {
        /// <summary>
        /// Maximum number of channels which this decoder produces.
        /// </summary>
        int MaximumChannels { get; }

        /// <summary>
        /// Runs the decoder thread.
        /// </summary>
        /// <param name="inputBuffer">Buffer from which new PPM values are read.</param>
        /// <param name="inputTrigger">Trigger which is fired by the caller when new data arrives.</param>
        /// <param name="outputBuffer">Buffer into which decoded PPM frames are written.</param>
        /// <param name="outputTrigger">Trigger which is fired by this decoder when new data has been decoded.</param>
        /// <param name="stop">Signals when the decoder should stop.</param>
        void DecodePulse(ConcurrentQueue<PpmPulse> inputBuffer, AutoResetEvent inputTrigger,
            ConcurrentQueue<PpmFrame> outputBuffer, AutoResetEvent outputTrigger, CancellationToken stop);
    }
}
