using Emlid.WindowsIot.Tools.Navio2RcioTerminal.Models;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace Emlid.WindowsIot.Tools.Navio2RcioTerminal.Views
{
    /// <summary>
    /// Start page of the application.
    /// </summary>
    public sealed partial class StartPage : StartPageBase
    {
        #region Lifetime

        /// <summary>
        /// Creates the page.
        /// </summary>
        public StartPage()
        {
            // Initialize view
            InitializeComponent();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates the page model when it is displayed.
        /// </summary>
        protected override StartUIModel CreateModel(RcioTerminalApplicationUIModel application)
        {
            return new StartUIModel(application);
        }

        #endregion

        #region Events

        /// <summary>
        /// Initializes the page when it is loaded.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs arguments)
        {
            // Call base class method
            base.OnNavigatedTo(arguments);

            // Hook events
            Model.PropertyChanged += OnModelChanged;

            // Update bindings
            //Bindings.Update();

            // Initial layout
            UpdateLayout();
        }

        /// <summary>
        /// Updates view elements when the model changes and no automatic
        /// method is currently available.
        /// </summary>
        private void OnModelChanged(object sender, PropertyChangedEventArgs arguments)
        {
            switch (arguments.PropertyName)
            {
                case nameof(Model):
                    //Bindings.Update();
                    break;
            }
        }

        /// <summary>
        /// Exits the application when the corresponding button is clicked.
        /// </summary>
        private void OnExitButtonClick(object sender, RoutedEventArgs arguments)
        {
            Application.Current.Exit();
        }

        #endregion
    }
}
