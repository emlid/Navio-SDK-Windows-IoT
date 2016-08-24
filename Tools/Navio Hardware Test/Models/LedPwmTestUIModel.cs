using Emlid.WindowsIot.Hardware;
using Emlid.WindowsIot.Hardware.Boards.Navio;
using Emlid.WindowsIot.Hardware.Components.NxpPca9685;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Emlid.WindowsIot.Tests.NavioHardwareTestApp.Models
{
    /// <summary>
    /// UI model for testing the <see cref="NavioLedPwmDevice"/>.
    /// </summary>
    public sealed class LedPwmTestUIModel : TestUIModel
    {
        #region Constants

        /// <summary>
        /// Steps in which to cycle the LED.
        /// </summary>
        public const int LedCycleStep = 16;

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        public LedPwmTestUIModel(TaskFactory uiThread) : base(uiThread)
        {
            // Initialize device
            Device = NavioLedPwmDevice.Initialize(NavioLedPwmDevice.ServoFrequencyDefault);
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
            // Only managed resources to dispose
            if (!disposing)
                return;

            // Stop updates
            StopLedCycleTask();

            // Close device
            Device?.Dispose();
        }

        #endregion

        #endregion

        #region Fields

        /// <summary>
        /// LED cycle background task.
        /// </summary>
        Task _ledCycleTask;

        /// <summary>
        /// Cancellation source for the <see cref="_ledCycleTask"/>;
        /// </summary>
        /// <remarks>
        /// Used to stop the task.
        /// </remarks>
        CancellationTokenSource _ledCycleCancel;

        #endregion

        #region Properties

        /// <summary>
        /// Device.
        /// </summary>
        public NavioLedPwmDevice Device { get; private set; }

        /// <summary>
        /// Starts or stops the automatic-update background task.
        /// </summary>
        public bool LedCycle
        {
            get
            {
                // Return true when running
                lock (Device)
                {
                    return _ledCycleTask != null && _ledCycleTask.Status == TaskStatus.Running;
                }
            }
            set
            {
                // Start or stop task
                lock (Device)
                {
                    var running = (_ledCycleTask != null && _ledCycleTask.Status == TaskStatus.Running);
                    if (value && !running)
                    {
                        // Create new task
                        _ledCycleCancel = new CancellationTokenSource();
                        _ledCycleTask = Task.Factory.StartNew(() => LedCycleTask(_ledCycleCancel.Token));
                    }
                    else if (!value && running)
                    {
                        // Stop existing task
                        StopLedCycleTask();
                    }
                }

                // Fire event
                DoPropertyChanged(nameof(LedCycle));
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Tests the <see cref="NavioLedPwmDevice.ReadAll"/> function.
        /// </summary>
        public void Update()
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
        /// Tests the <see cref="NavioLedPwmDevice.Restart"/> function.
        /// </summary>
        public void Restart()
        {
            RunTest(delegate { Device.Restart(); });
        }

        /// <summary>
        /// Clears any existing output then tests the <see cref="NxpPca9685Device.Clear"/> function.
        /// </summary>
        public override void Clear()
        {
            // Call base class to clear output
            base.Clear();

            // Run device clear test
            RunTest(delegate { Device.Clear(); });
        }

        #endregion

        #region Non-Public Methods

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

        /// <summary>
        /// Enables output if necessary.
        /// </summary>
        private void EnsureOutputEnabled()
        {
            if (!Device.OutputEnabled)
            {
                WriteOutput("Enabling output.");
                Device.OutputEnabled = true;
                DoPropertyChanged(nameof(Device));
            }
        }

        /// <summary>
        /// Background task delegate which executes the LED Cycle.
        /// </summary>
        /// <param name="cancel">Cancellation token.</param>
        private void LedCycleTask(CancellationToken cancel)
        {
            try
            {
                // Initialize
                WriteOutput("LED cycle starting on background thread...");
                const int maximum = NxpPca9685ChannelValue.Maximum;
                var red = Device.LedRed;
                var green = Device.LedGreen;
                var blue = Device.LedBlue;
                Func<int, int> increment = (int value) => { value += LedCycleStep; return value < maximum ? value : maximum; };
                Func<int, int> decrement = (int value) => { value -= LedCycleStep; return value > 0 ? value : 0; };

                // Ensure output is enabled
                EnsureOutputEnabled();

                // Cycle red LED component down...
                WriteOutput("Cycling red down...");
                while (red > 0)
                {
                    red = decrement(red);
                    Device.SetLed(red, green, blue);
                    DoPropertyChanged(nameof(Device));

                    // Check for cancellation
                    cancel.ThrowIfCancellationRequested();
                }

                // Cycle green LED component down...
                WriteOutput("Cycling green down...");
                while (green > 0)
                {
                    green = decrement(green);
                    Device.SetLed(red, green, blue);
                    DoPropertyChanged(nameof(Device));

                    // Check for cancellation
                    cancel.ThrowIfCancellationRequested();
                }

                // Cycle blue LED component down...
                WriteOutput("Cycling blue down...");
                while (blue > 0)
                {
                    blue = decrement(blue);
                    Device.SetLed(red, green, blue);
                    DoPropertyChanged(nameof(Device));

                    // Check for cancellation
                    cancel.ThrowIfCancellationRequested();
                }

                // Cycle red LED component up...
                WriteOutput("Cycling red up...");
                while (red < maximum)
                {
                    red = increment(red);
                    Device.SetLed(red, green, blue);
                    DoPropertyChanged(nameof(Device));

                    // Check for cancellation
                    cancel.ThrowIfCancellationRequested();
                }

                // Cycle greed LED component up...
                WriteOutput("Cycling green up...");
                while (green < maximum)
                {
                    green = increment(green);
                    Device.SetLed(red, green, blue);
                    DoPropertyChanged(nameof(Device));

                    // Check for cancellation
                    cancel.ThrowIfCancellationRequested();
                }

                // Cycle blue LED component up...
                WriteOutput("Cycling blue up...");
                while (blue < maximum)
                {
                    blue = increment(blue);
                    Device.SetLed(red, green, blue);
                    DoPropertyChanged(nameof(Device));

                    // Check for cancellation
                    cancel.ThrowIfCancellationRequested();
                }

                // Report success
                WriteOutput("LED cycle complete.");
            }
            catch (OperationCanceledException)
            {
                // Report cancellation
                WriteOutput("LED cycle canceled.");
            }
            finally
            {
                // Update LED cycle button
                DoPropertyChanged(nameof(LedCycle));
            }
        }

        /// <summary>
        /// Stops the LED cycle task if running and frees related resources.
        /// </summary>
        private void StopLedCycleTask()
        {
            lock(this)
            {
                if (_ledCycleCancel != null)
                {
                    if (_ledCycleTask != null)
                    {
                        // Stop task when running
                        if (_ledCycleTask.Status == TaskStatus.Running)
                            _ledCycleCancel.Cancel();

                        // Clean-up task
                        _ledCycleTask = null;
                    }

                    // Clean-up cancellation token
                    _ledCycleCancel.Dispose();
                    _ledCycleCancel = null;
                }
            }
        }

        #endregion
    }
}
