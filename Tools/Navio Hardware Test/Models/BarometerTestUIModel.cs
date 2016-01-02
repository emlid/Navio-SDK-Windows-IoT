using Emlid.WindowsIot.Hardware;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.CompilerServices;

namespace Emlid.WindowsIot.Tests.NavioHardwareTestApp.Models
{
    /// <summary>
    /// UI model for testing the <see cref="NavioBarometerDevice"/>.
    /// </summary>
    public class BarometerTestUIModel : TestUIModel
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        public BarometerTestUIModel(TaskFactory uiThread) : base(uiThread)
        {
            // Initialize members
            OsrList = new List<int>(Enum.GetValues(typeof(Ms5611Osr)).Cast<int>());
            Graph = new List<Ms5611Measurement>();

            // Initialize device
            Device = new NavioBarometerDevice();
            Device.MeasurementUpdated += OnMeasurementUpdated;
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
                    StopAutoUpdateTask();
                    if (Device != null)
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

        #region Fields

        /// <summary>
        /// Auto-update background task.
        /// </summary>
        Task _autoUpdateTask;

        /// <summary>
        /// Cancellation source for the <see cref="_autoUpdateTask"/>;
        /// </summary>
        /// <remarks>
        /// Used to stop the task.
        /// </remarks>
        CancellationTokenSource _autoUpdateCancel;

        #endregion

        #region Properties

        /// <summary>
        /// Device.
        /// </summary>
        public NavioBarometerDevice Device { get; private set; }

        /// <summary>
        /// OSR option list.
        /// </summary>
        public List<int> OsrList { get; private set; }

        /// <summary>
        /// History of measurements for display as a graph, with oldest items first.
        /// </summary>
        /// <remarks>
        /// For performance new entries are added to the end of the list, to avoid having to insert.
        /// When rendering the graph iterate backwards from <see cref="ICollection{T}.Count"/> to display the newest items first.
        /// </remarks>
        public List<Ms5611Measurement> Graph { get; private set; }

        /// <summary>
        /// Starts or stops the automatic-update background task.
        /// </summary>
        public bool AutoUpdate
        {
            get
            {
                // Return true when running
                lock (Device)
                {
                    return _autoUpdateTask != null && _autoUpdateTask.Status == TaskStatus.Running;
                }
            }
            set
            {
                // Start or stop task
                lock (Device)
                {
                    var running = (_autoUpdateTask != null && _autoUpdateTask.Status == TaskStatus.Running);
                    if (value && !running)
                    {
                        // Create new task
                        _autoUpdateCancel = new CancellationTokenSource();
                        _autoUpdateTask = Task.Factory.StartNew(() => AutoUpdateTask(_autoUpdateCancel.Token));
                    }
                    else if (!value && running)
                    {
                        // Stop existing task
                        StopAutoUpdateTask();
                    }
                }

                // Fire event
                DoPropertyChanged(nameof(AutoUpdate));
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Tests the <see cref="Ms5611Device.Reset"/> function.
        /// </summary>
        public void Reset()
        {
            RunTest(delegate { Device.Reset(); });
        }

        /// <summary>
        /// Tests the <see cref="Ms5611Device.Update"/> function.
        /// </summary>
        public void Update()
        {
            RunTest(delegate { Device.Update(); });
        }

        /// <summary>
        /// Clears all content.
        /// </summary>
        public override void Clear()
        {
            // Call base class to clear output
            base.Clear();

            // Clear graph
            Graph.Clear();

            // Update display
            DoPropertyChanged(nameof(Graph));
        }

        #endregion

        #region Events

        /// <summary>
        /// Updates the display when the <see cref="Device"/> channels change.
        /// </summary>
        private void OnMeasurementUpdated(object sender, Ms5611Measurement measurement)
        {
            // Dump statistics to output
            WriteOutput(measurement.ToString());

            // Add data point to graph
            Graph.Add(measurement);

            // Update display
            DoPropertyChanged(nameof(Device));
            DoPropertyChanged(nameof(Graph));
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
        /// Background task delegate which executes the auto-update.
        /// </summary>
        /// <param name="cancel">Cancellation token.</param>
        private void AutoUpdateTask(CancellationToken cancel)
        {
            try
            {
                // Initialize
                WriteOutput("Auto-update starting on background thread...");

                // Run until canceled
                while (!cancel.IsCancellationRequested)
                {
                    // Request hardware update
                    Device.Update();

                    // Update properties
                    DoPropertyChanged(nameof(Device));
                }
            }
            catch (OperationCanceledException) { }

            // Report cancellation
            WriteOutput("Auto-update stopped.");
        }

        /// <summary>
        /// Stops the auto-update task if running and frees related resources.
        /// </summary>
        private void StopAutoUpdateTask()
        {
            if (_autoUpdateCancel != null)
            {
                if (_autoUpdateTask != null)
                {
                    if (_autoUpdateTask.Status == TaskStatus.Running)
                    {
                        // Stop task when running
                        _autoUpdateCancel.Cancel();
                        try
                        {
                            _autoUpdateTask.Wait(_autoUpdateCancel.Token);
                        }
                        catch (OperationCanceledException) { /* Expected */ }
                    }

                    // Clean-up task
                    _autoUpdateTask = null;
                }

                // Clear-up cancellation token
                _autoUpdateCancel.Dispose();
                _autoUpdateCancel = null;
            }
        }

        #endregion
    }
}
