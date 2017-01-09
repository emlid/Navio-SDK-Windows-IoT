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
		public static string PpmCycleFormat 
		{ 
			get { return ResourceLoader.GetString("PpmCycleFormat"); }
		}
 	
		///<summary>
		///0 = channel, 1 = value.
		///</summary>
		public static string PpmFrameFormatChannel 
		{ 
			get { return ResourceLoader.GetString("PpmFrameFormatChannel"); }
		}
 	
		///<summary>
		///0 = time.
		///</summary>
		public static string PpmFrameFormatStart 
		{ 
			get { return ResourceLoader.GetString("PpmFrameFormatStart"); }
		}
 	
		///<summary>
		///0 = level, 1 = time.
		///</summary>
		public static string PpmPulseFormat 
		{ 
			get { return ResourceLoader.GetString("PpmPulseFormat"); }
		}
 	
		///<summary>
		///0 = pressure, 1 = temperature.
		///</summary>
		public static string BarometerMeasurementStringFormat 
		{ 
			get { return ResourceLoader.GetString("BarometerMeasurementStringFormat"); }
		}
 	
		///<summary>
		///0 = width, 1 = length, 2 = percent.
		///</summary>
		public static string PwmPulseFormat 
		{ 
			get { return ResourceLoader.GetString("PwmPulseFormat"); }
		}
 	}
}
