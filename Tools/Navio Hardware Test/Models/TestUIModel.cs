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
    public abstract class TestUIModel : INotifyPropertyChanged, IDisposable
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        protected TestUIModel(TaskFactory uiThread)
        {
            // Initialize members
            UIThread = uiThread;
            _output = new StringBuilder();
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

            // Flag disposed
            IsDisposed = true;
        }

        /// <summary>
        /// Calls <see cref="Dispose(bool)"/> during finalization, when not proactively disposed.
        /// </summary>
        ~TestUIModel()
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

        #region Fields

        /// <summary>
        /// UI task factory.
        /// </summary>
        protected TaskFactory UIThread { get; private set; }

        #endregion

        #region Properties

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
            try
            {
                // Output start message
                WriteOutput("Starting {0}...", name);

                // Run test
                test();

                // Output successful end messsage
                WriteOutput("Finished {0}.", name);
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
        protected virtual void DoPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                UIThread.StartNew(() =>
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
                }
                ).Wait();
            }
        }

        #endregion
    }
}
