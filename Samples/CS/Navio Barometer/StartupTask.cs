using Emlid.WindowsIot.Hardware;
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
            using (var barometer = new NavioBarometerDevice())
            {
                barometer.Reset();
                while (true)
                { 
                    barometer.Update();
                    Debug.WriteLine(barometer.Measurement);
                }
            }
        }
    }
}
