﻿using System.Diagnostics.CodeAnalysis;

namespace Emlid.WindowsIot.Hardware.Boards.Navio
{
    /// <summary>
    /// Navio GPS device interface.
    /// </summary>
    /// <remarks>
    /// Navio models have different GPS chips. This interface provides a hardware
    /// model agnostic way to communicate with the GPS.
    /// </remarks>
    [SuppressMessage("Microsoft.Design", "CA1040", Justification = "Work in progress.")]
    public interface INavioGpsDevice
    {
        // TODO: Implement GPS
    }
}