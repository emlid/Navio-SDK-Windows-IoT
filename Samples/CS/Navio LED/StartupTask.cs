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
        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <param name="taskInstance">Background task instance.</param>
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Connect to hardware
            Debug.WriteLine("Connecting to Navio board.");
            using (var board = NavioDeviceProvider.Connect())
            {
                Debug.WriteLine("Navio board was detected as a \"{0}\".", board.Model);
                var led = board.Led;

                // Log start
                Debug.WriteLine("Navio LED test start.");

                // Enable oscillator and output if necessary
                if (led.CanSleep) led.Wake();
                if (led.CanDisable) led.Enabled = true;

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
        }
    }
}
