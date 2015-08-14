using Emlid.WindowsIoT.Hardware;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Emlid.WindowsIoT.Tests.HardwareTestApp.Models
{
    /// <summary>
    /// UI model for testing the <see cref="NavioPca9685Device"/>.
    /// </summary>
    public class LedPwmTestUIModel : TestUIModel
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        public LedPwmTestUIModel(TaskFactory uiThread) : base(uiThread)
        {
            // Initialize device
            Device = NavioPca9685Device.Initialize(NavioPca9685Device.ServoFrequencyDefault);
        }

        #region IDisposable

        /// <summary>
        /// Frees resources owned by this instance.
        /// </summary>
        /// <param name="disposing">
        /// True when called via <see cref="IDisposable.Dispose()"/>, false when called during finalization.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            // Dispose only once
            if (IsDisposed) return;

            // Dispose
            try
            {
                // Free managed resources
                if (disposing)
                {
                    Device.Dispose();
                }
            }
            finally
            {
                // Dispose base class
                base.Dispose(disposing);
            }
        }

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Device.
        /// </summary>
        public NavioPca9685Device Device { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Tests the <see cref="NavioPca9685Device.ReadAll"/> function.
        /// </summary>
        public void ReadAll()
        {
            RunTest(delegate { Device.ReadAll(); });
        }

        /// <summary>
        /// Tests the <see cref="NxpPca9685Device.Sleep"/> function.
        /// </summary>
        public void Sleep()
        {
            RunTest(delegate { Device.Sleep(); });
        }

        /// <summary>
        /// Tests the <see cref="NxpPca9685Device.Wake"/> function.
        /// </summary>
        public void Wake()
        {
            RunTest(delegate { Device.Wake(); });
        }

        /// <summary>
        /// Tests the <see cref="NavioPca9685Device.Restart"/> function.
        /// </summary>
        public void Restart()
        {
            RunTest(delegate { Device.Restart(); });
        }

        /// <summary>
        /// Tests the <see cref="NavioPca9685Device.SetLed(int, int, int)"/> function.
        /// </summary>
        public void SetLed(int red, int green, int blue)
        {
            RunTest(delegate { Device.SetLed(red, green, blue); });
        }

        /// <summary>
        /// Tests the <see cref="NxpPca9685Device.Clear"/> function.
        /// </summary>
        public void Clear()
        {
            RunTest(delegate { Device.Clear(); });
        }

        /// <summary>
        /// Cycles the LEDs.
        /// </summary>
        public void LedCycle()
        {
            Task.Factory.StartNew(() =>
            {
                WriteOutput("LED cycle starting on background thread...");

                ushort red = 0, green = 0, blue = 0;
                WriteOutput("LED cycle setting red {0} green {1} blue {2}...", red, green, blue);
                Device.SetLed(red, green, blue);
                DoPropertyChanged(nameof(Device));

                WriteOutput("Cycling red up...");
                for (red = 0; red < 4095; red++)
                {
                    Device.SetLed(red, green, blue);
                    DoPropertyChanged(nameof(Device));
                }

                WriteOutput("Cycling green up...");
                for (green = 0; green < 4095; green++)
                {
                    Device.SetLed(red, green, blue);
                    DoPropertyChanged(nameof(Device));
                }

                WriteOutput("Cycling blue up...");
                for (blue = 0; blue < 4095; blue++)
                {
                    Device.SetLed(red, green, blue);
                    DoPropertyChanged(nameof(Device));
                }

                WriteOutput("Cycling red down...");
                for (red = 4095; red > 0; red--)
                {
                    Device.SetLed(red, green, blue);
                    DoPropertyChanged(nameof(Device));
                }

                WriteOutput("Cycling green down...");
                for (green = 4095; green > 0; green--)
                {
                    Device.SetLed(red, green, blue);
                    DoPropertyChanged(nameof(Device));
                }

                WriteOutput("Cycling blue down...");
                for (blue = 4095; blue > 0; blue--)
                {
                    Device.SetLed(red, green, blue);
                    DoPropertyChanged(nameof(Device));
                }

                WriteOutput("LED cycle complete.");
            });
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Runs a test method with status and error output.
        /// </summary>
        /// <param name="test">Test delegate to run.</param>
        /// <param name="name">Name to use in the output.</param>
        protected override void RunTest(Action test, [CallerMemberName] string name = "")
        {
            // Call base class to run test
            base.RunTest(test, name);

            // Update properties
            DoPropertyChanged(nameof(Device));
        }

        #endregion
    }
}
