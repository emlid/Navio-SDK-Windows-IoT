using Windows.UI.Xaml.Controls;

namespace Emlid.WindowsIot.Tests.NavioHardwareTestApp.Views.Shared
{
    /// <summary>
    /// Message dialog box for UWP applications (the original XAML MessageDialog class is not available).
    /// </summary>
    public sealed partial class MessageDialog : ContentDialog
    {
        #region Lifetime

        /// <summary>
        /// Creates an empty instance.
        /// </summary>
        public MessageDialog()
        {
            // Initialize view
            InitializeComponent();
        }

        /// <summary>
        /// Creates an instance with specific settings.
        /// </summary>
        /// <param name="title">Dialog title.</param>
        /// <param name="message">Message text.</param>
        /// <param name="confirmButtonText">Optional confirmation button text override. Leave null or empty for the default "OK".</param>
        /// <param name="cancelButton">Set false to hide the cancel button. Default is true.</param>
        /// <param name="cancelButtonText">Optional cancel button text override. Leave null or empty for the default "Cancel".</param>
        public MessageDialog(string title, string message, string confirmButtonText = null, bool cancelButton = true, string cancelButtonText = null)
            : this()
        {
            // Initialize properties
            Title = title;
            Message.Text = message;
            PrimaryButtonText = !string.IsNullOrWhiteSpace(confirmButtonText) ? confirmButtonText : "OK";
            if (cancelButton)
                SecondaryButtonText = !string.IsNullOrWhiteSpace(cancelButtonText) ? confirmButtonText : "Cancel";
            else
                IsSecondaryButtonEnabled = false;
        }

        #endregion
    }
}
