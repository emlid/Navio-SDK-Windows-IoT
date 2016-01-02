using Emlid.WindowsIot.Tests.NavioHardwareTestApp.Models;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.ComponentModel;

namespace Emlid.WindowsIot.Tests.NavioHardwareTestApp.Views.Tests
{
    /// <summary>
    /// Main page.
    /// </summary>
    public sealed partial class FramTestPage : Page
    {
        #region Lifetime

        /// <summary>
        /// Initializes the page.
        /// </summary>
        public FramTestPage()
        {
            // Initialize members
            _uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            _uiFactory = new TaskFactory(_uiScheduler);

            // Initialize view
            InitializeComponent();
        }

        #endregion

        #region Fields

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
        public FramTestUIModel Model { get; private set; }

        #endregion

        #region Events

        /// <summary>
        /// Initializes the page when it is loaded.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs arguments)
        {
            // Initialize model and bind
            DataContext = Model = new FramTestUIModel(_uiFactory);
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
            Model.Dispose();

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
                    OutputScroller.ScrollToVerticalOffset(OutputScroller.ScrollableHeight);
                    break;
            }
        }

        /// <summary>
        /// Executes the <see cref="FramTestUIModel.Read"/> action when the related button is clicked.
        /// </summary>
        private void OnReadButtonClick(object sender, RoutedEventArgs arguments)
        {
            Model.Read();
        }

        /// <summary>
        /// Executes the <see cref="FramTestUIModel.Erase"/> action when the related button is clicked.
        /// </summary>
        private void OnEraseButtonClick(object sender, RoutedEventArgs arguments)
        {
            Model.Erase();
        }

        /// <summary>
        /// Executes the <see cref="FramTestUIModel.Fill"/> action when the related button is clicked.
        /// </summary>
        private void OnFillButtonClick(object sender, RoutedEventArgs arguments)
        {
            Model.Fill();
        }

        /// <summary>
        /// Executes the <see cref="FramTestUIModel.Sequence"/> action when the related button is clicked.
        /// </summary>
        private void OnSequenceButtonClick(object sender, RoutedEventArgs arguments)
        {
            Model.Sequence();
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