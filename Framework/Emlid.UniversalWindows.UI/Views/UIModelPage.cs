using System;
using System.ComponentModel;
using Emlid.UniversalWindows.UI.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Emlid.UniversalWindows.UI.Views
{
    /// <summary>
    /// Base class for all XAML pages which support the UI model framework.
    /// </summary>
    public abstract partial class UIModelPage<TApplicationUIModel, TPageUIModel> : Page
        where TApplicationUIModel : ApplicationUIModel
        where TPageUIModel : PageUIModel<TApplicationUIModel>
    {
        #region Lifetime

        /// <summary>
        /// Initializes an instance.
        /// </summary>
        protected UIModelPage()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// <see cref="Model"/> <see cref="DependencyProperty"/>.
        /// </summary>
        public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(nameof(Model), typeof(TPageUIModel), typeof(UIModelPage<TApplicationUIModel, TPageUIModel>), null);

        /// <summary>
        /// Page UI model.
        /// </summary>
        public TPageUIModel Model
        {
            get { return (TPageUIModel)GetValue(ModelProperty); }
            private set { SetValue(ModelProperty, value); }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates the page model when it is displayed.
        /// </summary>
        protected abstract TPageUIModel CreateModel(TApplicationUIModel application);

        #endregion

        #region Events

        /// <summary>
        /// Initializes the page when it is loaded.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs arguments)
        {
            // Initialize model
            var application = (UIModelApplication<TApplicationUIModel>)Application.Current;
            DataContext = Model = CreateModel(application.Model);

            // Call base class method
            base.OnNavigatedTo(arguments);
        }

        /// <summary>
        /// Cleans-up when navigating away from the page.
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs arguments)
        {
            try
            {
                // Call base class method
                base.OnNavigatedFrom(arguments);
            }
            finally
            {
                // Dispose model
                Model?.Dispose();
            }
        }

        #endregion
    }
}
