using Emlid.WindowsIot.Hardware.Boards.Navio;
using Emlid.WindowsIot.Hardware.Components.Ms5611;
using Emlid.WindowsIot.Hardware.Protocols.Barometer;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Emlid.WindowsIot.Tests.NavioHardwareTestApp.Views.Tests
{
    /// <summary>
    /// UI model for testing the <see cref="INavioBarometerDevice"/>.
    /// </summary>
    public sealed class BarometerTestUIModel : TestUIModel
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        public BarometerTestUIModel(ApplicationUIModel application) : base(application)
        {
            // Initialize members
            Graph = new List<BarometerMeasurement>();

            // Initialize device
            Device = Application.Board.Barometer;
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
            try
            {
                // Dispose resources when possible
                if (disposing)
                {
                    // Stop updates
                    StopAutoUpdateTask();

                    // Unhook events
                    Device.MeasurementUpdated -= OnMeasurementUpdated;
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
        public INavioBarometerDevice Device { get; private set; }

        /// <summary>
        /// History of measurements for display as a graph, with oldest items first.
        /// </summary>
        /// <remarks>
        /// For performance new entries are added to the end of the list, to avoid having to insert.
        /// When rendering the graph iterate backwards from <see cref="ICollection{T}.Count"/> to display the newest items first.
        /// </remarks>
        public List<BarometerMeasurement> Graph { get; private set; }

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
            // Run test
            RunTest(delegate { Device.Reset(); });

            // Update view
            DoPropertyChanged(nameof(Device));
        }

        /// <summary>
        /// Tests the <see cref="Ms5611Device.Update"/> function.
        /// </summary>
        public void Update()
        {
            // Run test
            RunTest(delegate { Device.Update(); });

            // Update view
            DoPropertyChanged(nameof(Device));
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
        private void OnMeasurementUpdated(object sender, BarometerMeasurement measurement)
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
        /// Background task delegate which executes the auto-update.
        /// </summary>
        /// <param name="cancel">Cancellation token.</param>
        private void AutoUpdateTask(CancellationToken cancel)
        {
            try
            {
                // Initialize
                WriteOutput("Auto-update starting on background thread...");

                // Run as fast as possible until canceled
                while (!cancel.IsCancellationRequested)
                    Device.Update();
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

                // Clean-up cancellation token
                _autoUpdateCancel?.Dispose();
                _autoUpdateCancel = null;
            }
        }

        #endregion
    }
}
