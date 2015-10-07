using System.Collections.Concurrent;
using System.Threading;

namespace Emlid.WindowsIoT.Hardware
{
    /// <summary>
    /// Defines a common PWM decoder interface to support multiple protocols.
    /// </summary>
    public interface IPwmDecoder
    {
        /// <summary>
        /// Maximum number of channels which this decoder produces.
        /// </summary>
        int MaximumChannels { get; }
        
        /// <summary>
        /// Runs the decoder thread.
        /// </summary>
        /// <param name="inputBuffer">Buffer from which new PWM values are read.</param>
        /// <param name="inputTrigger">Trigger which is fired by the caller when new data arrives.</param>
        /// <param name="outputBuffer">Buffer into which decoded PWM frames are written.</param>
        /// <param name="outputTrigger">Trigger which is fired by this decoder when new data has been decoded.</param>
        /// <param name="stop">Signals when the decoder should stop.</param>
        void Decode(ConcurrentQueue<PwmValue> inputBuffer, AutoResetEvent inputTrigger, 
            ConcurrentQueue<PwmFrame> outputBuffer, AutoResetEvent outputTrigger, CancellationToken stop);
    }
}
