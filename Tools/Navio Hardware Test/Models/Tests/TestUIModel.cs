using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Emlid.WindowsIot.Tests.NavioHardwareTestApp.Views.Tests
{
    /// <summary>
    /// Base class for all test UI models.
    /// </summary>
    public abstract class TestUIModel : PageUIModel
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        protected TestUIModel(ApplicationUIModel application) : base(application)
        {
            // Initialize members
            InputEnabled = true;
            _output = new StringBuilder();
        }

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
            // Validate
            if (text == null) throw new ArgumentNullException(nameof(text));

            // Add text to output with formatting when necessary
            string output;
            if (arguments?.Length == 0)
                output = text;
            else
                output = string.Format(CultureInfo.CurrentCulture, text, arguments);

            // Add time stamp
            output = string.Format(CultureInfo.CurrentCulture, "{0} {1}", DateTime.Now, output);

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
    }
}
