using System.Diagnostics.CodeAnalysis;

namespace Emlid.WindowsIot.Hardware.Boards.Navio
{
    /// <summary>
    /// Navio ADC device interface.
    /// </summary>
    /// <remarks>
    /// Navio models have different ADC circuits and counts. This interface provides a hardware
    /// model agnostic way to communicate with each ADC connector.
    /// </remarks>
    [SuppressMessage("Microsoft.Design", "CA1040", Justification = "Work in progress.")]
    public interface INavioAdcDevice
    {
        // TODO: Implement ADC
    }
}