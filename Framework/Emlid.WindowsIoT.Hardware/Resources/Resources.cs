// PublicReswFileCodeGenerator for Windows 8.1 1.0.0
// (c) Sébastien Ollivier - https://github.com/sebastieno/PublicResWFileCodeGenerator/
// Updated by Tony Wall for Windows 10/IoT usage.

namespace Emlid.WindowsIot.Hardware.Resources  {
		
    /// <summary>
    /// Strings.
    /// </summary>
	public class Strings {
		
		private static Windows.ApplicationModel.Resources.ResourceLoader resourceLoader;
		internal static Windows.ApplicationModel.Resources.ResourceLoader ResourceLoader
		{
			get
			{
				if(resourceLoader == null)
				{
					resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForViewIndependentUse("Emlid.WindowsIot.Hardware\\Strings");
				}

				return resourceLoader;
			}
		}
			
		///<summary>
        ///0 = pin, 1 = controller index.
		///</summary>
		public static string GpioErrorDeviceNotFound 
		{ 
			get { return ResourceLoader.GetString("GpioErrorDeviceNotFound"); }
		}
 	
		///<summary>
        ///0 = address, 1 = controller index.
		///</summary>
		public static string I2cErrorDeviceNotFound 
		{ 
			get { return ResourceLoader.GetString("I2cErrorDeviceNotFound"); }
		}
 	
		///<summary>
        ///
		///</summary>
		public static string Mb85rc04vLowerAddressInvalidHasUpperBits 
		{ 
			get { return ResourceLoader.GetString("Mb85rc04vLowerAddressInvalidHasUpperBits"); }
		}
 	
		///<summary>
        ///
		///</summary>
		public static string Mb85rc04vUpperAddressInvalidNoMatchLower 
		{ 
			get { return ResourceLoader.GetString("Mb85rc04vUpperAddressInvalidNoMatchLower"); }
		}
 	
		///<summary>
        ///0 = pressure, 1 = temperature.
		///</summary>
		public static string MS5611MeasurementStringFormat 
		{ 
			get { return ResourceLoader.GetString("MS5611MeasurementStringFormat"); }
		}
 	
		///<summary>
        ///0 = # channels, 1 = maximum channels.
		///</summary>
		public static string NavioRCInputDecoderChannelOverflow 
		{ 
			get { return ResourceLoader.GetString("NavioRCInputDecoderChannelOverflow"); }
		}
 	
		///<summary>
        ///0 = low time, 1 = low length, 2 = high time, 3 = high length, 4 = total length.
		///</summary>
		public static string PwmCycleStringFormat 
		{ 
			get { return ResourceLoader.GetString("PwmCycleStringFormat"); }
		}
 	
		///<summary>
        ///0 = channel, 1 = value.
		///</summary>
		public static string PwmFrameStringFormatChannel 
		{ 
			get { return ResourceLoader.GetString("PwmFrameStringFormatChannel"); }
		}
 	
		///<summary>
        ///0 = time.
		///</summary>
		public static string PwmFrameStringFormatStart 
		{ 
			get { return ResourceLoader.GetString("PwmFrameStringFormatStart"); }
		}
 	
		///<summary>
        ///0 = level, 1 = time.
		///</summary>
		public static string PwmValueStringFormat 
		{ 
			get { return ResourceLoader.GetString("PwmValueStringFormat"); }
		}
 	}
}
