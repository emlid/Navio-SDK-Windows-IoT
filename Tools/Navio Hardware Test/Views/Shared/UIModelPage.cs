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
        public UIModelPage()
        {
            var application = (ThisApp)App.Current;
            ApplicationUIModel = application.Model;
        }

        #endregion

        #region Public Properties

        ///// <summary>
        ///// XAML Application.
        ///// </summary>
        //public ThisApp Application { get; private set; }

        /// <summary>
        /// Application UI model.
        /// </summary>
        public ApplicationUIModel ApplicationUIModel { get; private set; }

        #endregion
    }
}
