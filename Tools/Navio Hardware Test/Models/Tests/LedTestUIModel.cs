using Emlid.WindowsIot.Hardware.Boards.Navio;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Emlid.WindowsIot.Tests.NavioHardwareTestApp.Views.Tests
{
    /// <summary>
    /// UI model for testing the <see cref="INavioLedDevice"/>.
    /// </summary>
    public sealed class LedTestUIModel : TestUIModel
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
        public LedTestUIModel(ApplicationUIModel application) : base(application)
        {
            // Initialize members
            Device = application.Board.Led;
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
            try
            {
                // Dispose resources when possible
                if (disposing)
                {
                    // Stop updates
                    StopCycleTask();

                    // Disable device if required
                    if (Device?.CanDisable == true)
                        Device.Enabled = false;
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

        #region Fields

        /// <summary>
        /// LED cycle background task.
        /// </summary>
        Task _cycleTask;

        /// <summary>
        /// Cancellation source for the <see cref="_cycleTask"/>;
        /// </summary>
        /// <remarks>
        /// Used to stop the task.
        /// </remarks>
        CancellationTokenSource _cycleCancel;

        #endregion

        #region Public Properties

        /// <summary>
        /// Device.
        /// </summary>
        public INavioLedDevice Device { get; private set; }

        /// <summary>
        /// Starts or stops the automatic-update background task.
        /// </summary>
        public bool Cycle
        {
            get
            {
                // Return true when running
                lock (Device)
                {
                    return _cycleTask != null && _cycleTask.Status == TaskStatus.Running;
                }
            }
            set
            {
                // Start or stop task
                lock (Device)
                {
                    var running = (_cycleTask != null && _cycleTask.Status == TaskStatus.Running);
                    if (value && !running)
                    {
                        // Create new task
                        _cycleCancel = new CancellationTokenSource();
                        _cycleTask = Task.Factory.StartNew(() => CycleTask(_cycleCancel.Token));
                    }
                    else if (!value && running)
                    {
                        // Stop existing task
                        StopCycleTask();
                    }
                }

                // Fire event
                DoPropertyChanged(nameof(Cycle));
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Clears any existing output then tests the <see cref="INavioLedDevice.Clear"/> function.
        /// </summary>
        public override void Clear()
        {
            // Call base class to clear output
            base.Clear();

            // Run device clear test
            RunTest(delegate { Device.Clear(); });
        }

        /// <summary>
        /// Tests the <see cref="INavioLedDevice.Read"/> function.
        /// </summary>
        public void Read()
        {
            RunTest(delegate { Device.Read(); });
        }

        /// <summary>
        /// Tests the <see cref="INavioLedDevice.Sleep"/> function.
        /// </summary>
        public void Sleep()
        {
            RunTest(delegate { Device.Sleep(); });
        }

        /// <summary>
        /// Tests the <see cref="INavioLedDevice.Wake"/> function.
        /// </summary>
        public void Wake()
        {
            RunTest(delegate { Device.Wake(); });
        }

        /// <summary>
        /// Tests the <see cref="INavioLedDevice.Restart"/> function.
        /// </summary>
        public void Restart()
        {
            RunTest(delegate { Device.Restart(); });
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
            if (!Device.Enabled)
            {
                WriteOutput("Enabling output.");
                Device.Enabled = true;
                DoPropertyChanged(nameof(Device));
            }
        }

        /// <summary>
        /// Background task delegate which executes the LED Cycle.
        /// </summary>
        /// <param name="cancel">Cancellation token.</param>
        private void CycleTask(CancellationToken cancel)
        {
            try
            {
                // Initialize
                WriteOutput("LED cycle starting on background thread...");
                var maximum = Device.MaximumValue;
                var red = Device.Red;
                var green = Device.Green;
                var blue = Device.Blue;
                Func<int, int> increment = (int value) => { value += LedCycleStep; return value < maximum ? value : maximum; };
                Func<int, int> decrement = (int value) => { value -= LedCycleStep; return value > 0 ? value : 0; };

                // Ensure output is enabled
                EnsureOutputEnabled();

                // Cycle until stopped
                while (!cancel.IsCancellationRequested)
                {
                    // Cycle red LED component down...
                    WriteOutput("Cycling red down...");
                    while (red > 0)
                    {
                        red = decrement(red);
                        Device.SetRgb(red, green, blue);
                        DoPropertyChanged(nameof(Device));

                        // Check for cancellation
                        cancel.ThrowIfCancellationRequested();
                    }

                    // Cycle green LED component down...
                    WriteOutput("Cycling green down...");
                    while (green > 0)
                    {
                        green = decrement(green);
                        Device.SetRgb(red, green, blue);
                        DoPropertyChanged(nameof(Device));

                        // Check for cancellation
                        cancel.ThrowIfCancellationRequested();
                    }

                    // Cycle blue LED component down...
                    WriteOutput("Cycling blue down...");
                    while (blue > 0)
                    {
                        blue = decrement(blue);
                        Device.SetRgb(red, green, blue);
                        DoPropertyChanged(nameof(Device));

                        // Check for cancellation
                        cancel.ThrowIfCancellationRequested();
                    }

                    // Cycle red LED component up...
                    WriteOutput("Cycling red up...");
                    while (red < maximum)
                    {
                        red = increment(red);
                        Device.SetRgb(red, green, blue);
                        DoPropertyChanged(nameof(Device));

                        // Check for cancellation
                        cancel.ThrowIfCancellationRequested();
                    }

                    // Cycle greed LED component up...
                    WriteOutput("Cycling green up...");
                    while (green < maximum)
                    {
                        green = increment(green);
                        Device.SetRgb(red, green, blue);
                        DoPropertyChanged(nameof(Device));

                        // Check for cancellation
                        cancel.ThrowIfCancellationRequested();
                    }

                    // Cycle blue LED component up...
                    WriteOutput("Cycling blue up...");
                    while (blue < maximum)
                    {
                        blue = increment(blue);
                        Device.SetRgb(red, green, blue);
                        DoPropertyChanged(nameof(Device));

                        // Check for cancellation
                        cancel.ThrowIfCancellationRequested();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Report cancellation
                WriteOutput("LED cycle canceled.");
            }
            finally
            {
                // Update LED cycle button
                DoPropertyChanged(nameof(Cycle));
            }
        }

        /// <summary>
        /// Stops the LED cycle task if running and frees related resources.
        /// </summary>
        private void StopCycleTask()
        {
            lock(this)
            {
                if (_cycleCancel != null)
                {
                    if (_cycleTask != null)
                    {
                        // Stop task when running
                        if (_cycleTask.Status == TaskStatus.Running)
                            _cycleCancel.Cancel();

                        // Clean-up task
                        _cycleTask = null;
                    }

                    // Clean-up cancellation token
                    _cycleCancel.Dispose();
                    _cycleCancel = null;
                }
            }
        }

        #endregion
    }
}
