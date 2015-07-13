using Emlid.WindowsIoT.Hardware;
using System;
using Windows.ApplicationModel.Background;

namespace Emlid.WindowsIoT.Samples.NavioLed
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
            var pwm = NavioPca9685Device.Initialize(NavioPca9685Device.ServoFrequencyDefault);

            // Start with yellow LED
            var maximum = NxpPca9685ChannelValue.Maximum;

            // Enable oscillator and output
            pwm.Wake();
            pwm.OutputEnabled = true;

            // Fade LEDs to blue
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
            while (true)
            {
                // Red up...
                while (pwm.LedRed < maximum)
                    pwm.LedRed++;

                // Blue down...
                while (pwm.LedBlue > 0)
                    pwm.LedBlue--;

                // Green up..
                while (pwm.LedGreen < maximum)
                    pwm.LedGreen++;

                // Red down (test via RGB)...
                int red = maximum, green = maximum, blue = 0;
                for (red = maximum; red > 0; red--)
                    pwm.SetLed(red, green, blue);

                // Blue up...
                for (blue = 0; blue < maximum; blue++)
                    pwm.SetLed(red, green, blue);

                // Green down...
                for (green = maximum; green > 0; green--)
                    pwm.SetLed(red, green, blue);
            }
        }
    }
}
