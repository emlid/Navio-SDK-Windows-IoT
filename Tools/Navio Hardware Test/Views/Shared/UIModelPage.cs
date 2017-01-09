using Windows.UI.Xaml.Controls;
using ThisApp = Emlid.WindowsIot.Tests.NavioHardwareTestApp.App;

namespace Emlid.WindowsIot.Tests.NavioHardwareTestApp.Views.Shared
{
    /// <summary>
    /// Base class for all XAML pages which support the UI model framework.
    /// </summary>
    public partial class UIModelPage : Page
    {
        #region Lifetime

        /// <summary>
        /// Initializes an instance.
        /// </summary>
        protected UIModelPage()
        {
            var application = (ThisApp)App.Current;
            ApplicationModel = application.Model;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Application UI model.
        /// </summary>
        public ApplicationUIModel ApplicationModel { get; private set; }

        #endregion
    }
}
