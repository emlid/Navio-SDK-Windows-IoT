using Emlid.WindowsIot.Hardware;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
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
                // Receive notifcations when frames arrive
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
