using Emlid.WindowsIot.Hardware.Boards.Navio;
using Emlid.WindowsIot.Hardware.Components.NxpPca9685;
using System.Diagnostics;
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
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Initialize PWM device at servo frequency (50Hz) with output disabled
            var pwm = NavioLedPwmDevice.Initialize(NavioLedPwmDevice.ServoFrequencyDefault);

            // Log start
            Debug.WriteLine("Navio LED test start.");

            // Enable oscillator and output
            pwm.Wake();
            pwm.OutputEnabled = true;

            // Fade LEDs to blue
            Debug.WriteLine("Fading to blue.");
            var maximum = NxpPca9685ChannelValue.Maximum;
            bool fade;
            do
            {
                fade = false;
                if (pwm.LedRed > 0)
                {
                    pwm.LedRed--;
                    fade = true;
                }
                if (pwm.LedGreen > 0)
                {
                    pwm.LedGreen--;
                    fade = true;
                }
                if (pwm.LedBlue < maximum)
                {
                    pwm.LedBlue++;
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
                while (pwm.LedRed < maximum)
                    pwm.LedRed++;

                // Blue down via property...
                Debug.WriteLine("Blue down via property...");
                while (pwm.LedBlue > 0)
                    pwm.LedBlue--;

                // Green up via propety..
                Debug.WriteLine("Green up via property...");
                while (pwm.LedGreen < maximum)
                    pwm.LedGreen++;

                // Red down via set RGB method...
                Debug.WriteLine("Red down via set RGB method...");
                int red = maximum, green = maximum, blue = 0;
                for (red = maximum; red > 0; red--)
                    pwm.SetLed(red, green, blue);

                // Blue upvia set RGB method...
                Debug.WriteLine("Blue up via set RGB method...");
                for (blue = 0; blue < maximum; blue++)
                    pwm.SetLed(red, green, blue);

                // Green downvia set RGB method...
                Debug.WriteLine("Green down via set RGB method...");
                for (green = maximum; green > 0; green--)
                    pwm.SetLed(red, green, blue);
            }
        }
    }
}
