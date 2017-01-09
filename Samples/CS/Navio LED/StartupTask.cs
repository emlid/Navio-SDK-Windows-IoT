using Emlid.WindowsIot.Hardware.Boards.Navio;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Emlid.WindowsIot.Samples.NavioLed
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
            var led = _board.Led;

            // Log start
            Debug.WriteLine("Navio LED test start.");

            // Enable oscillator and output if necessary
            if (!led.Enabled)
                led.Enabled = true;

            // Fade LEDs to blue
            Debug.WriteLine("Fading to blue.");
            var maximum = led.MaximumValue;
            bool fade;
            do
            {
                fade = false;
                if (led.Red > 0)
                {
                    led.Red--;
                    fade = true;
                }
                if (led.Green > 0)
                {
                    led.Green--;
                    fade = true;
                }
                if (led.Blue < maximum)
                {
                    led.Blue++;
                    fade = true;
                }
            }
            while (fade);

            // Cycle LED in infinite loop...
            Debug.WriteLine("Cycling in infinite loop...");
            while (true)
            {
                // Red up via property...
                Debug.WriteLine("Red up via property...");
                while (led.Red < maximum) led.Red++;

                // Wait a bit...
                Task.Delay(TimeSpan.FromSeconds(1)).Wait();

                // Blue down via property...
                Debug.WriteLine("Blue down via property...");
                while (led.Blue > 0) led.Blue--;

                // Wait a bit...
                Task.Delay(TimeSpan.FromSeconds(1)).Wait();

                // Green up via property..
                Debug.WriteLine("Green up via property...");
                while (led.Green < maximum) led.Green++;

                // Wait a bit...
                Task.Delay(TimeSpan.FromSeconds(1)).Wait();

                // Red down via set RGB method...
                Debug.WriteLine("Red down via set RGB method...");
                var red = maximum; var green = maximum; var blue = 0;
                do { led.SetRgb(--red, green, blue); } while (red > 0);

                // Wait a bit...
                Task.Delay(TimeSpan.FromSeconds(1)).Wait();

                // Blue up via set RGB method...
                Debug.WriteLine("Blue up via set RGB method...");
                do { led.SetRgb(red, green, ++blue); } while (blue < maximum);

                // Wait a bit...
                Task.Delay(TimeSpan.FromSeconds(1)).Wait();

                // Green down via set RGB method...
                Debug.WriteLine("Green down via set RGB method...");
                do { led.SetRgb(red, --green, blue); } while (green > 0);

                // Wait a bit...
                Task.Delay(TimeSpan.FromSeconds(1)).Wait();
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
