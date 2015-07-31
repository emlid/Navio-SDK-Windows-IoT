using Emlid.WindowsIoT.Hardware;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Emlid.WindowsIoT.Tests.HardwareTestApp.Models
{
    /// <summary>
    /// UI model for testing the <see cref="NavioPca9685Device"/> device.
    /// </summary>
    public class LedPwmTestUIModel : INotifyPropertyChanged, IDisposable
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        public LedPwmTestUIModel(TaskFactory uiThread)
        {
            // Initialize members
            _uiThread = uiThread;
            _output = new StringBuilder();

            // Initialize device
            Device = NavioPca9685Device.Initialize(NavioPca9685Device.ServoFrequencyDefault);
        }

        #region IDisposable

        /// <summary>
        /// Prevents duplicate calls to <see cref="Dispose()"/>.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Frees resources owned by this instance.
        /// </summary>
        /// <param name="disposing">
        /// True when called via <see cref="Dispose()"/>, false when called during finalization.
        /// </param>
        protected virtual void Dispose(bool disposing)
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

                // Release references
                Device = null;
            }
            finally
            {
                // Flag disposed
                IsDisposed = true;
            }
        }

        /// <summary>
        /// Calls <see cref="Dispose(bool)"/> during finalization, when not proactively disposed.
        /// </summary>
        ~LedPwmTestUIModel()
        {
            // Partial dispose
            Dispose(false);
        }

        /// <summary>
        /// Proactively frees resources owned by this object.
        /// </summary>
        public void Dispose()
        {
            try
            {
                // Full dispose
                Dispose(true);
            }
            finally
            {
                // Suppress finalizer
                GC.SuppressFinalize(this);
            }
        }

        #endregion

        #endregion

        #region Private Fields

        /// <summary>
        /// UI task factory.
        /// </summary>
        TaskFactory _uiThread;

        #endregion

        #region Properties

        /// <summary>
        /// Device.
        /// </summary>
        public NavioPca9685Device Device { get; private set; }

        /// <summary>
        /// Output text.
        /// </summary>
        public string Output { get { lock(_output) { return _output.ToString(); } } }
        private StringBuilder _output;

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

        #region Private Methods

        /// <summary>
        /// Writes text to the output.
        /// </summary>
        private void WriteOutput(string text, params object[] arguments)
        {
            // Add text to output with formatting when necessary
            string output;
            if (arguments.Length == 0)
                output = text;
            else
                output = String.Format(CultureInfo.CurrentCulture, text, arguments);

            // Add time stamp
            output = String.Format(CultureInfo.CurrentCulture, "{0} {1}", DateTime.Now, output);

            // Write to output and debugger
            lock (_output)
                _output.AppendLine(output);
            Debug.WriteLine(output);

            // Update view
            DoPropertyChanged(nameof(Output));
        }

        /// <summary>
        /// Runs a test method with status and error output.
        /// </summary>
        /// <param name="test">Test delegate to run.</param>
        /// <param name="name">Name to use in the output.</param>
        private void RunTest(Action test, [CallerMemberName] string name = "")
        {
            try
            {
                // Output start message
                WriteOutput("Starting {0}...", name);

                // Run test
                test();

                // Output successful end messsage
                WriteOutput("Finished {0}.", name);

                // Update properties
                DoPropertyChanged(nameof(Device));
            }
            catch (Exception error)
            {
                // Output error message
                WriteOutput(error.ToString());
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Fired when the model data has changed and the view should be refreshed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Fires the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="name">Name of the property which changed.</param>
        private void DoPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                _uiThread.StartNew(() =>
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
                }
                ).Wait();
            }
        }

        #endregion
    }
}
