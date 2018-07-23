using Emlid.WindowsIot.Hardware.Boards.Navio;
using Emlid.WindowsIot.Hardware.Protocols.Ppm;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Windows.ApplicationModel.Background;

namespace Emlid.WindowsIot.Samples.NavioRCInput
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
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Initialize task
            _taskDeferral = taskInstance.GetDeferral();
            taskInstance.Canceled += OnTaskCancelled;

            // Connect to hardware
            Debug.WriteLine("Connecting to Navio board.");
            _board = NavioDeviceProvider.Connect();
            Debug.WriteLine("Navio board was detected as a \"{0}\".", _board.Model);
            var rcInput = _board.RCInput;

            // Log start
            Debug.WriteLine("Navio RC input test start.");

            // Start receiving frames
            Debug.WriteLine("Waiting for valid PWM frames...");
            rcInput.ChannelsChanged += OnChannelsChanged;
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

        /// <summary>
        /// Displays PWM frames as they arrive.
        /// </summary>
        private void OnChannelsChanged(object sender, PpmFrame frame)
        {
            // Write contents to the debug output
            Debug.WriteLine(frame);
        }

        #endregion
    }
}
