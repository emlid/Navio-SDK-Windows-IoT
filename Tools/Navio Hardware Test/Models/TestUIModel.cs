using Emlid.WindowsIot.Common;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Emlid.WindowsIot.Tests.NavioHardwareTestApp.Models
{
    /// <summary>
    /// Base class for all test UI models.
    /// </summary>
    public abstract class TestUIModel : DisposableObject, INotifyPropertyChanged
    {
        #region Constants

        /// <summary>
        /// Maximum time (in milliseconds) allowed for the UI to process updates before continuing with other updates.
        /// </summary>
        /// <remarks>
        /// When too short and events are generated too quickly, the UI has no chance to refresh.
        /// When too long and processor intensive operations are triggered, the UI could appear to hang.
        /// </remarks>
        public const int UpdateTimeout = 500;

        #endregion

        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        protected TestUIModel(TaskFactory uiThread)
        {
            // Initialize members
            UIThread = uiThread;
            InputEnabled = true;
            _output = new StringBuilder();
        }

        #endregion

        #region Fields

        /// <summary>
        /// UI task factory.
        /// </summary>
        protected TaskFactory UIThread { get; private set; }

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether new user input is enabled.
        /// </summary>
        public bool InputEnabled { get; private set; }

        /// <summary>
        /// Output text.
        /// </summary>
        public string Output { get { lock(_output) { return _output.ToString(); } } }
        private StringBuilder _output;

        #endregion

        #region Public Methods

        /// <summary>
        /// Clears all content.
        /// </summary>
        /// <remarks>
        /// Should be overridden to clear any other generated content.
        /// </remarks>
        public virtual void Clear()
        {
            ClearOutput();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Clears all content from <see cref="Output"/>.
        /// </summary>
        protected void ClearOutput()
        {
            // Clear content
            lock(_output)
               _output.Length = 0;

            // Update view
            DoPropertyChanged(nameof(Output));
        }

        /// <summary>
        /// Writes text to the output.
        /// </summary>
        protected virtual void WriteOutput(string text, params object[] arguments)
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
        protected virtual void RunTest(Action test, [CallerMemberName] string name = "")
        {
            // Do nothing when input is disabled
            if (!InputEnabled)
                return;

            // Run test on background thread...
            Task.Factory.StartNew(() =>
            {
                try
                {
                    // Disable tests
                    InputEnabled = false;
                    DoPropertyChanged(nameof(InputEnabled));

                    // Output start message
                    WriteOutput("Starting {0}...", name);

                    // Run test
                    test();

                    // Output successful end message
                    WriteOutput("Finished {0}.", name);
                }
                catch (Exception error)
                {
                    // Output error message
                    WriteOutput(error.ToString());
                }
                finally
                {
                    // Re-enable tests
                    InputEnabled = true;
                    DoPropertyChanged(nameof(InputEnabled));
                }
            });
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
        protected virtual void DoPropertyChanged(string name)
        {
            // Do nothing when disposed
            if (IsDisposed) return;

            // Run event handler on UI thread
            if (PropertyChanged != null)
            {
                UIThread.StartNew(() =>
                {
                    // Do nothing when disposed (may occur whilst scheduling call to UI thread)
                    if (IsDisposed) return;

                    // Fire event causing UI to update
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
                })
                .Wait(UpdateTimeout);
            }
        }

        #endregion
    }
}
