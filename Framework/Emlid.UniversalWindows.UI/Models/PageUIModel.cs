using System;

namespace Emlid.UniversalWindows.UI.Models
{
    /// <summary>
    /// Base class for all page UI models
    /// </summary>
    public abstract class PageUIModel<TApplicationUIModel> : UIModel
        where TApplicationUIModel : ApplicationUIModel
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        protected PageUIModel(TApplicationUIModel application)
        {
            // Validate
            if (application == null) throw new ArgumentNullException(nameof(application));

            // Initialize members
            Application = application;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Application model.
        /// </summary>
        public TApplicationUIModel Application { get; private set; }

        #endregion
    }
}
