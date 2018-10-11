using CodeForDotNet.WindowsUniversal.UI.Models;
using Emlid.WindowsIot.Tools.NavioHardwareTest.Models;

namespace Emlid.WindowsIot.Tools.NavioHardwareTest
{
    /// <summary>
    /// Wraps the generic XAML base class so that it can be used in the UWP and XAML designer,
    /// which does not support generic or self-references base class type arguments.
    /// </summary>
    public abstract class AppBase : UIModelApplication<TestApplicationUIModel>
    {
        #region Lifetime

        /// <summary>
        /// Creates an instance.
        /// </summary>
        protected AppBase()
        {
        }

        #endregion Lifetime
    }
}