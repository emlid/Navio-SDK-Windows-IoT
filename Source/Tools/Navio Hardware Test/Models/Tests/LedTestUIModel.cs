using Emlid.WindowsIot.Hardware.Boards.Navio;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Emlid.WindowsIot.Tools.NavioHardwareTest.Models.Tests
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
        public const int LedCycleStep = 100;

        #endregion Constants

        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        public LedTestUIModel(TestApplicationUIModel application) : base(application)
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

        #endregion IDisposable

        #endregion Lifetime

        #region Fields

        /// <summary>
        /// LED cycle background task.
        /// </summary>
        private Task _cycleTask;

        /// <summary>
        /// Cancellation source for the <see cref="_cycleTask"/>;
        /// </summary>
        /// <remarks>
        /// Used to stop the task.
        /// </remarks>
        private CancellationTokenSource _cycleCancel;

        #endregion Fields

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
                        _cycleTask = Task.Factory.StartNew(() => CycleTask(_cycleCancel.Token),
                            CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
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

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Tests the <see cref="INavioLedDevice.Reset"/> function.
        /// </summary>
        public void Reset()
        {
            lock (Device)
            {
                // Run test
                RunTest(delegate { Device.Reset(); });

                // Update view
                DoPropertyChanged(nameof(Device));
            }
        }

        /// <summary>
        /// Tests the <see cref="INavioLedDevice.Read"/> function.
        /// </summary>
        public void Read()
        {
            lock (Device)
            {
                // Run test
                RunTest(delegate { Device.Read(); });

                // Update view
                DoPropertyChanged(nameof(Device));
            }
        }

        #endregion Public Methods

        #region Non-Public Methods

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

                // Break LED range into steps
                var step = (int)Math.Round((float)maximum / LedCycleStep);
                if (step < 1) step = 1;
                int increment(int value) { value += LedCycleStep; return value < maximum ? value : maximum; }
                int decrement(int value) { value -= LedCycleStep; return value > 0 ? value : 0; }

                // Ensure output is enabled
                if (!Device.Enabled)
                {
                    // Enable output
                    WriteOutput("Enabling output...");
                    Device.Enabled = true;

                    // Update view
                    DoPropertyChanged(nameof(Device));
                }

                // Cycle until stopped
                while (!cancel.IsCancellationRequested)
                {
                    // Cycle red LED component up via property...
                    WriteOutput("Cycling red up via property...");
                    while (red < maximum)
                    {
                        // Decrement red channel
                        red = increment(red);
                        Device.Red = red;

                        // Update view
                        DoPropertyChanged(nameof(Device));

                        // Check for cancellation
                        cancel.ThrowIfCancellationRequested();
                    }

                    // Wait a bit...
                    Task.Delay(TimeSpan.FromSeconds(1)).Wait();

                    // Cycle blue LED component down via property...
                    WriteOutput("Cycling blue down via property...");
                    while (blue > 0)
                    {
                        // Decrement blue channel
                        blue = decrement(blue);
                        Device.Blue = blue;

                        // Update view
                        DoPropertyChanged(nameof(Device));

                        // Check for cancellation
                        cancel.ThrowIfCancellationRequested();
                    }

                    // Wait a bit...
                    Task.Delay(TimeSpan.FromSeconds(1)).Wait();

                    // Cycle green LED component up via property...
                    WriteOutput("Cycling green up via property...");
                    while (green < maximum)
                    {
                        // Decrement green channel
                        green = increment(green);
                        Device.Green = green;

                        // Update view
                        DoPropertyChanged(nameof(Device));

                        // Check for cancellation
                        cancel.ThrowIfCancellationRequested();
                    }

                    // Wait a bit...
                    Task.Delay(TimeSpan.FromSeconds(1)).Wait();

                    // Cycle red LED component down via RGB...
                    WriteOutput("Cycling red down via RGB...");
                    while (red > 0)
                    {
                        // Increment red channel
                        red = decrement(red);
                        Device.SetRgb(red, green, blue);

                        // Update view
                        DoPropertyChanged(nameof(Device));

                        // Check for cancellation
                        cancel.ThrowIfCancellationRequested();
                    }

                    // Wait a bit...
                    Task.Delay(TimeSpan.FromSeconds(1)).Wait();

                    // Cycle blue LED component up...
                    WriteOutput("Cycling blue up via RGB...");
                    while (blue < maximum)
                    {
                        // Increment blue
                        blue = increment(blue);
                        Device.SetRgb(red, green, blue);

                        // Update view
                        DoPropertyChanged(nameof(Device));

                        // Check for cancellation
                        cancel.ThrowIfCancellationRequested();
                    }

                    // Wait a bit...
                    Task.Delay(TimeSpan.FromSeconds(1)).Wait();

                    // Cycle greed LED component down...
                    WriteOutput("Cycling green down via RGB...");
                    while (green > 0)
                    {
                        // Increment green channel
                        green = decrement(green);
                        Device.SetRgb(red, green, blue);

                        // Update view
                        DoPropertyChanged(nameof(Device));

                        // Check for cancellation
                        cancel.ThrowIfCancellationRequested();
                    }

                    // Wait a bit...
                    Task.Delay(TimeSpan.FromSeconds(1)).Wait();
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
            lock (Device)
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

        #endregion Non-Public Methods
    }
}