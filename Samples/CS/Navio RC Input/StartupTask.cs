using Emlid.WindowsIot.Hardware.Boards.Navio;
using Emlid.WindowsIot.Hardware.Protocols.Pwm;
using System.Diagnostics;
using Windows.ApplicationModel.Background;

namespace Emlid.WindowsIot.Samples.NavioRCInput
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
            // Create the RC input device
            using (var rcInput = new NavioRCInputDevice())
            {
                // Log start
                Debug.WriteLine("Navio RC input test start.");

                // Start receiving frames
                Debug.WriteLine("Waiting for valid PWM frames...");
                rcInput.ChannelsChanged += OnChannelsChanged;

                // Wait forever (this background task has no GUI)
                rcInput.Stopped.WaitOne();
            }
        }

        /// <summary>
        /// Receives updates when new PWM frames arrive.
        /// </summary>
        private void OnChannelsChanged(object sender, PwmFrame frame)
        {
            // Write contents to the debug output
            Debug.WriteLine(frame);
        }
    }
}
