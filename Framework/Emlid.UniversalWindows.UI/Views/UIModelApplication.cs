using Emlid.UniversalWindows.UI.Models;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace Emlid.UniversalWindows.UI.Views
{
    /// <summary>
    /// Base class for all XAML applications which support the UI model framework.
    /// </summary>
    public abstract partial class UIModelApplication<TApplicationUIModel> : Application
        where TApplicationUIModel : ApplicationUIModel
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance with the specified model.
        /// </summary>
        protected UIModelApplication()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Application UI model.
        /// </summary>
        public TApplicationUIModel Model { get; private set; }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Starts the application.
        /// </summary>
        /// <param name="arguments">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs arguments)
        {
            // Create UI model
            Model = CreateModel();

            // Call base class method
            base.OnLaunched(arguments);
        }

        /// <summary>
        /// Creates the application model when it is started.
        /// </summary>
        protected virtual TApplicationUIModel CreateModel()
        {
            return default(TApplicationUIModel);
        }

        #endregion
    }
}
