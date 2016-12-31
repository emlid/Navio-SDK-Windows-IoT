using Emlid.WindowsIot.Hardware.Boards.Navio;
using System.Diagnostics;
using Windows.ApplicationModel.Background;

namespace Emlid.WindowsIot.Samples.NavioBarometer
{
    /// <summary>
    /// Start-up task.
    /// </summary>
    public sealed class StartupTask : IBackgroundTask
    {
        /// <summary>
        /// Executes the task.
        /// </summary>
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Connect to hardware
            Debug.WriteLine("Connecting to Navio board.");
            using (var board = NavioDeviceProvider.Connect())
            {
                Debug.WriteLine("Navio board was detected as a \"{0}\".", board.Model);
                var barometer = board.Barometer;

                // Reset
                Debug.WriteLine("Resetting device.");
                barometer.Reset();

                // Infinite update loop
                Debug.WriteLine("Starting infinite update loop...");
                while (true)
                {
                    var measurement = barometer.Update();
                    Debug.WriteLine(measurement);
                }
            }
        }
    }
}
