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
        #region Private Fields

        /// <summary>
        /// Hardware.
        /// </summary>
        INavioBoard _board;

        /// <summary>
        /// Background task deferral, allowing the task to continue executing after the <see cref="Run(IBackgroundTaskInstance)"/> method has completed.
        /// </summary>
        BackgroundTaskDeferral _taskDeferral;

        #endregion

        #region Public Methods

        /// <summary>
        /// Application start-up.
        /// </summary>
        /// <param name="taskInstance">Task instance.</param>
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Initialize task
            _taskDeferral = taskInstance.GetDeferral();
            taskInstance.Canceled += OnTaskCancelled;

            // Connect to hardware
            Debug.WriteLine("Connecting to Navio board.");
            _board = NavioDeviceProvider.Connect();
            Debug.WriteLine("Navio board was detected as a \"{0}\".", _board.Model);
            var barometer = _board.Barometer;

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

        #endregion

        #region Events

        /// <summary>
        /// Completes the task gracefully when canceled.
        /// </summary>
        /// <param name="sender">Background task instance.</param>
        /// <param name="reason">Cancellation reason.</param>
        private void OnTaskCancelled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            // Release hardware resources
            Debug.WriteLine("Disconnecting from Navio board.");
            _board?.Dispose();

            // End execution
            Debug.WriteLine("Application finished.");
            _taskDeferral.Complete();
        }

        #endregion
    }
}
