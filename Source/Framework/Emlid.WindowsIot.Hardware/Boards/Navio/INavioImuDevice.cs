﻿using System.Diagnostics.CodeAnalysis;

namespace Emlid.WindowsIot.Hardware.Boards.Navio
{
    /// <summary>
    /// Navio GPS device interface.
    /// </summary>
    /// <remarks>
    /// Navio models have different IMU chips and counts. This interface provides a hardware
    /// model agnostic way to communicate with each IMU.
    /// </remarks>
    [SuppressMessage("Microsoft.Design", "CA1040", Justification = "Work in progress.")]
    public interface INavioImuDevice
    {
        // TODO: Implement IMU
    }
}