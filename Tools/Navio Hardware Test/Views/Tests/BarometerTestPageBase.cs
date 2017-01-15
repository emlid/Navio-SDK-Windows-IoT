using Emlid.UniversalWindows.UI.Views;
using Emlid.WindowsIot.Tools.NavioHardwareTest.Models;
using Emlid.WindowsIot.Tools.NavioHardwareTest.Models.Tests;

namespace Emlid.WindowsIot.Tools.NavioHardwareTest.Views.Tests
{
    /// <summary>
    /// Wraps the generic XAML base class so that it can be used in the UWP and XAML designer,
    /// which does not support generic or self-references base class type arguments.
    /// </summary>
    public abstract class BarometerTestPageBase : UIModelPage<TestApplicationUIModel, BarometerTestUIModel>
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        protected BarometerTestPageBase()
        {
        }

        #endregion
    }
}
