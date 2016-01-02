using System;

namespace Emlid.WindowsIot.Common
{
    /// <summary>
    /// Provides an <see cref="IDisposable"/> base class.
    /// </summary>
    public abstract class DisposableObject : IDisposable
    {
        /// <summary>
        /// Indicates this instance has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Frees resources owned by this instance.
        /// </summary>
        /// <param name="disposing">
        /// True when called via <see cref="Dispose()"/>, false when called from the finalizer.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            // Flag disposed
            IsDisposed = true;
        }

        /// <summary>
        /// Finalizer which calls <see cref="Dispose(bool)"/> with false when it has not been disabled
        /// by a proactive call to <see cref="Dispose()"/>.
        /// </summary>
        ~DisposableObject()
        {
            // Partial dispose
            Dispose(false);
        }

        /// <summary>
        /// Pro-actively frees resources owned by this instance.
        /// </summary>
        public void Dispose()
        {
            try
            {
                // Full managed dispose
                Dispose(true);
            }
            finally
            {
                // Suppress finalizer (we already cleaned-up)
                GC.SuppressFinalize(this);
            }
        }
    }
}
