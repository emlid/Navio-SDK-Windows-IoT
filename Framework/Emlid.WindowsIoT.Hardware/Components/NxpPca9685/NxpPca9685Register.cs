namespace Emlid.WindowsIot.Hardware.Components.NxpPca9685
{
    /// <summary>
    /// Defines the I2C registers of the <see cref="NxpPca9685Device"/>.
    /// </summary>
    /// <remarks>
    /// Auto Increment past register 69 will point to mode 1 register (register 0).
    /// Auto Increment also works from register 250 to register 254, then rolls over to register 0.
    /// </remarks>
    public enum NxpPca9685Register : byte
    {
        #region Control

        /// <summary>
        /// Mode register 1, controls sleep, restart, auto-increment, external clocking, addressing.
        /// </summary>
        Mode1 = 0x00,

        /// <summary>
        /// Mode register 2, controls inversion, output condition, drive type.
        /// </summary>
        Mode2 = 0x01,

        /// <summary>
        /// Sub-address 1.
        /// </summary>
        SubAddress1 = 0x02,

        /// <summary>
        /// Sub-address 2.
        /// </summary>
        SubAddress2 = 0x03,

        /// <summary>
        /// Sub-address 2.
        /// </summary>
        SubAddress3 = 0x04,

        /// <summary>
        /// Enables or disables calling all LED channels.
        /// </summary>
        AllCall = 0x05,

        /// <summary>
        /// Prescaler for PWM output frequency.
        /// </summary>
        /// <remarks>
        /// Minimum value is 3.
        /// Writes to prescale register are blocked when sleep bit is 0 (mode 1).
        /// </remarks>
        Prescale = 0xfe,

        /// <summary>
        /// Test mode.
        /// </summary>
        /// <remarks>
        /// Reserved. Writes to this register may cause unpredictable results.
        /// </remarks>
        ModeTest = 0xff,

        #endregion

        #region LED Channels

        #region LED Channel 0

        /// <summary>
        /// Low byte of 12-bit word for rising edge of LED channel 0.
        /// </summary>
        Channel0OnLow = 0x06,

        /// <summary>
        /// High byte of 12-bit word for rising edge of LED channel 0.
        /// </summary>
        Channel0OnHigh = 0x07,

        /// <summary>
        /// Low byte of 12-bit word for falling edge of LED channel 0.
        /// </summary>
        Channel0OffLow = 0x08,

        /// <summary>
        /// High byte of 12-bit word for falling edge of LED channel 0.
        /// </summary>
        Channel0OffHigh = 0x09,

        #endregion

        #region LED Channel 1

        /// <summary>
        /// Low byte of 12-bit word for rising edge of LED channel 1.
        /// </summary>
        Channel1OnLow = 0x0a,

        /// <summary>
        /// High byte of 12-bit word for rising edge of LED channel 1.
        /// </summary>
        Channel1OnHigh = 0x0b,

        /// <summary>
        /// Low byte of 12-bit word for falling edge of LED channel 1.
        /// </summary>
        Channel1OffLow = 0x0c,

        /// <summary>
        /// High byte of 12-bit word for falling edge of LED channel 1.
        /// </summary>
        Channel1OffHigh = 0x0d,

        #endregion

        #region LED Channel 2

        /// <summary>
        /// Low byte of 12-bit word for rising edge of LED channel 2.
        /// </summary>
        Channel2OnLow = 0x0e,

        /// <summary>
        /// High byte of 12-bit word for rising edge of LED channel 2.
        /// </summary>
        Channel2OnHigh = 0x0f,

        /// <summary>
        /// Low byte of 12-bit word for falling edge of LED channel 2.
        /// </summary>
        Channel2OffLow = 0x10,

        /// <summary>
        /// High byte of 12-bit word for falling edge of LED channel 2.
        /// </summary>
        Channel2OffHigh = 0x11,

        #endregion

        #region LED Channel 3

        /// <summary>
        /// Low byte of 12-bit word for rising edge of LED channel 3.
        /// </summary>
        Channel3OnLow = 0x12,

        /// <summary>
        /// High byte of 12-bit word for rising edge of LED channel 3.
        /// </summary>
        Channel3OnHigh = 0x13,

        /// <summary>
        /// Low byte of 12-bit word for falling edge of LED channel 3.
        /// </summary>
        Channel3OffLow = 0x14,

        /// <summary>
        /// High byte of 12-bit word for falling edge of LED channel 3.
        /// </summary>
        Channel3OffHigh = 0x15,

        #endregion

        #region LED Channel 4

        /// <summary>
        /// Low byte of 12-bit word for rising edge of LED channel 4.
        /// </summary>
        Channel4OnLow = 0x16,

        /// <summary>
        /// High byte of 12-bit word for rising edge of LED channel 4.
        /// </summary>
        Channel4OnHigh = 0x17,

        /// <summary>
        /// Low byte of 12-bit word for falling edge of LED channel 4.
        /// </summary>
        Channel4OffLow = 0x18,

        /// <summary>
        /// High byte of 12-bit word for falling edge of LED channel 4.
        /// </summary>
        Channel4OffHigh = 0x19,

        #endregion

        #region LED Channel 5

        /// <summary>
        /// Low byte of 12-bit word for rising edge of LED channel 5.
        /// </summary>
        Channel5OnLow = 0x1a,

        /// <summary>
        /// High byte of 12-bit word for rising edge of LED channel 5.
        /// </summary>
        Channel5OnHigh = 0x1b,

        /// <summary>
        /// Low byte of 12-bit word for falling edge of LED channel 5.
        /// </summary>
        Channel5OffLow = 0x1c,

        /// <summary>
        /// High byte of 12-bit word for falling edge of LED channel 5.
        /// </summary>
        Channel5OffHigh = 0x1d,

        #endregion

        #region LED Channel 6

        /// <summary>
        /// Low byte of 12-bit word for rising edge of LED channel 6.
        /// </summary>
        Channel6OnLow = 0x1e,

        /// <summary>
        /// High byte of 12-bit word for rising edge of LED channel 6.
        /// </summary>
        Channel6OnHigh = 0x1f,

        /// <summary>
        /// Low byte of 12-bit word for falling edge of LED channel 6.
        /// </summary>
        Channel6OffLow = 0x20,

        /// <summary>
        /// High byte of 12-bit word for falling edge of LED channel 6.
        /// </summary>
        Channel6OffHigh = 0x21,

        #endregion

        #region LED Channel 7

        /// <summary>
        /// Low byte of 12-bit word for rising edge of LED channel 7.
        /// </summary>
        Channel7OnLow = 0x22,

        /// <summary>
        /// High byte of 12-bit word for rising edge of LED channel 7.
        /// </summary>
        Channel7OnHigh = 0x23,

        /// <summary>
        /// Low byte of 12-bit word for falling edge of LED channel 7.
        /// </summary>
        Channel7OffLow = 0x24,

        /// <summary>
        /// High byte of 12-bit word for falling edge of LED channel 7.
        /// </summary>
        Channel7OffHigh = 0x25,

        #endregion

        #region LED Channel 8

        /// <summary>
        /// Low byte of 12-bit word for rising edge of LED channel 8.
        /// </summary>
        Channel8OnLow = 0x26,

        /// <summary>
        /// High byte of 12-bit word for rising edge of LED channel 8.
        /// </summary>
        Channel8OnHigh = 0x27,

        /// <summary>
        /// Low byte of 12-bit word for falling edge of LED channel 8.
        /// </summary>
        Channel8OffLow = 0x28,

        /// <summary>
        /// High byte of 12-bit word for falling edge of LED channel 8.
        /// </summary>
        Channel8OffHigh = 0x29,

        #endregion

        #region LED Channel 9

        /// <summary>
        /// Low byte of 12-bit word for rising edge of LED channel 9.
        /// </summary>
        Channel9OnLow = 0x2a,

        /// <summary>
        /// High byte of 12-bit word for rising edge of LED channel 9.
        /// </summary>
        Channel9OnHigh = 0x2b,

        /// <summary>
        /// Low byte of 12-bit word for falling edge of LED channel 9.
        /// </summary>
        Channel9OffLow = 0x2c,

        /// <summary>
        /// High byte of 12-bit word for falling edge of LED channel 9.
        /// </summary>
        Channel9OffHigh = 0x2d,

        #endregion

        #region LED Channel 10

        /// <summary>
        /// Low byte of 12-bit word for rising edge of LED channel 10.
        /// </summary>
        Channel10OnLow = 0x2e,

        /// <summary>
        /// High byte of 12-bit word for rising edge of LED channel 10.
        /// </summary>
        Channel10OnHigh = 0x2f,

        /// <summary>
        /// Low byte of 12-bit word for falling edge of LED channel 10.
        /// </summary>
        Channel10OffLow = 0x30,

        /// <summary>
        /// High byte of 12-bit word for falling edge of LED channel 10.
        /// </summary>
        Channel10OffHigh = 0x31,

        #endregion

        #region LED Channel 11

        /// <summary>
        /// Low byte of 12-bit word for rising edge of LED channel 11.
        /// </summary>
        Channel11OnLow = 0x32,

        /// <summary>
        /// High byte of 12-bit word for rising edge of LED channel 11.
        /// </summary>
        Channel11OnHigh = 0x33,

        /// <summary>
        /// Low byte of 12-bit word for falling edge of LED channel 11.
        /// </summary>
        Channel11OffLow = 0x34,

        /// <summary>
        /// High byte of 12-bit word for falling edge of LED channel 11.
        /// </summary>
        Channel11OffHigh = 0x35,

        #endregion

        #region LED Channel 12

        /// <summary>
        /// Low byte of 12-bit word for rising edge of LED channel 12.
        /// </summary>
        Channel12OnLow = 0x36,

        /// <summary>
        /// High byte of 12-bit word for rising edge of LED channel 12.
        /// </summary>
        Channel12OnHigh = 0x37,

        /// <summary>
        /// Low byte of 12-bit word for falling edge of LED channel 12.
        /// </summary>
        Channel12OffLow = 0x38,

        /// <summary>
        /// High byte of 12-bit word for falling edge of LED channel 12.
        /// </summary>
        Channel12OffHigh = 0x39,

        #endregion

        #region LED Channel 13

        /// <summary>
        /// Low byte of 12-bit word for rising edge of LED channel 13.
        /// </summary>
        Channel13OnLow = 0x3a,

        /// <summary>
        /// High byte of 12-bit word for rising edge of LED channel 13.
        /// </summary>
        Channel13OnHigh = 0x3b,

        /// <summary>
        /// Low byte of 12-bit word for falling edge of LED channel 13.
        /// </summary>
        Channel13OffLow = 0x3c,

        /// <summary>
        /// High byte of 12-bit word for falling edge of LED channel 13.
        /// </summary>
        Channel13OffHigh = 0x3d,

        #endregion

        #region LED Channel 14

        /// <summary>
        /// Low byte of 12-bit word for rising edge of LED channel 14.
        /// </summary>
        Channel14OnLow = 0x3e,

        /// <summary>
        /// High byte of 12-bit word for rising edge of LED channel 14.
        /// </summary>
        Channel14OnHigh = 0x3f,

        /// <summary>
        /// Low byte of 12-bit word for falling edge of LED channel 14.
        /// </summary>
        Channel14OffLow = 0x40,

        /// <summary>
        /// High byte of 12-bit word for falling edge of LED channel 14.
        /// </summary>
        Channel14OffHigh = 0x41,

        #endregion

        #region LED Channel 15

        /// <summary>
        /// Low byte of 12-bit word for rising edge of LED channel 15.
        /// </summary>
        Channel15OnLow = 0x42,

        /// <summary>
        /// High byte of 12-bit word for rising edge of LED channel 15.
        /// </summary>
        Channel15OnHigh = 0x43,

        /// <summary>
        /// Low byte of 12-bit word for falling edge of LED channel 15.
        /// </summary>
        Channel15OffLow = 0x44,

        /// <summary>
        /// High byte of 12-bit word for falling edge of LED channel 15.
        /// </summary>
        Channel15OffHigh = 0x45,

        #endregion

        #region All LED Channels

        /// <summary>
        /// Low byte of 12-bit word for rising edge of all channels.
        /// </summary>
        AllChannelsOnLow = 0xfa,

        /// <summary>
        /// High byte of 12-bit word for rising edge of all channels.
        /// </summary>
        AllChannelsOnHigh = 0xfb,

        /// <summary>
        /// Low byte of 12-bit word for falling edge of all channels.
        /// </summary>
        AllChannelsOffLow = 0xfc,

        /// <summary>
        /// High byte of 12-bit word for falling edge of all channels.
        /// </summary>
        AllChannelsOffHigh = 0xfd

        #endregion

        #endregion
    }
}
