using Emlid.WindowsIot.Tests.NavioHardwareTestApp.Models;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Emlid.WindowsIot.Tests.NavioHardwareTestApp.Views.Tests
{
    /// <summary>
    /// Main page.
    /// </summary>
    public sealed partial class RCInputTestPage : Page
    {
        #region Lifetime

        /// <summary>
        /// Initializes the page.
        /// </summary>
        public RCInputTestPage()
        {
            // Initialize members
            _uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            _uiFactory = new TaskFactory(_uiScheduler);

            // Initialize view
            InitializeComponent();
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Task scheduler for the UI thread.
        /// </summary>
        private TaskScheduler _uiScheduler;

        /// <summary>
        /// Task factory for the UI thread.
        /// </summary>
        private TaskFactory _uiFactory;

        #endregion

        #region Properties

        /// <summary>
        /// UI model.
        /// </summary>
        public RCInputTestUIModel Model { get; private set; }

        #endregion

        #region Events

        /// <summary>
        /// Initializes the page when it is loaded.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs arguments)
        {
            // Initialize model and bind
            DataContext = Model = new RCInputTestUIModel(_uiFactory);
            Model.PropertyChanged += OnModelChanged;

            // Initial layout
            UpdateLayout();

            // Call base class method
            base.OnNavigatedTo(arguments);
        }

        /// <summary>
        /// Cleans-up when navigating away from the page.
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs arguments)
        {
            // Dispose model
            Model?.Dispose();

            // Call base class method
            base.OnNavigatedFrom(arguments);
        }

        /// <summary>
        /// Updates view elements when the model changes and no automatic
        /// method is currently available.
        /// </summary>
        private void OnModelChanged(object sender, PropertyChangedEventArgs arguments)
        {
            switch (arguments.PropertyName)
            {
                case nameof(Model.Output):
                    OutputScroller.UpdateLayout();
                    OutputScroller.ChangeView(null, OutputScroller.ScrollableHeight, null);
                    break;
            }
        }

        /// <summary>
        /// Executes the <see cref="TestUIModel.Clear"/> when the related button is clicked.
        /// </summary>
        private void OnClearButtonClick(object sender, RoutedEventArgs arguments)
        {
            Model.Clear();
        }

        /// <summary>
        /// Returns to the previous page when the close button is clicked.
        /// </summary>
        private void OnCloseButtonClick(object sender, RoutedEventArgs arguments)
        {
            Frame.GoBack();
        }

        #endregion
    }
}