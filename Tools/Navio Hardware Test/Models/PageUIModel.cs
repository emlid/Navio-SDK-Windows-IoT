using System;

namespace Emlid.WindowsIot.Tests.NavioHardwareTestApp.Views
{
    /// <summary>
    /// Base class for all page UI models
    /// </summary>
    public abstract class PageUIModel : UIModel
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        protected PageUIModel(ApplicationUIModel application)
        {
            // Validate
            if (application == null) throw new ArgumentNullException(nameof(application));

            // Initialize members
            Application = application;
        }

        #endregion

        #region Properties

        /// <summary>
        /// UI task factory.
        /// </summary>
        public ApplicationUIModel Application { get; private set; }

        #endregion
    }
}
