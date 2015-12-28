namespace Emlid.WindowsIot.Hardware
{
    /// <summary>
    /// <see cref="MS5611Device"/> conversion Over-Sampling Rate (OSR).
    /// </summary>
    /// <remarks>
    /// These offsets are added to either the <see cref="MS5611Command.ConvertD1Pressure"/> or
    /// <see cref="MS5611Command.ConvertD2Temperature"/> commands to specify the sample rate
    /// at which to perform the calculation.
    /// </remarks>
    public enum MS5611Osr
    {
        /// <summary>
        /// 256 times.
        /// </summary>
        Osr256 = 256,

        /// <summary>
        /// 512 times.
        /// </summary>
        Osr512 = 512,

        /// <summary>
        /// 1024 times.
        /// </summary>
        Osr1024 = 1024,

        /// <summary>
        /// 2048 times.
        /// </summary>
        Osr2048 = 2048,

        /// <summary>
        /// 4096 times.
        /// </summary>
        Osr4096 = 4096
    }
}
