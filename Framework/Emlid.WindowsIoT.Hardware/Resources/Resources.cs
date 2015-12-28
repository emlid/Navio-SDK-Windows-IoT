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
        ///0 = address, 1 = controller.
		///</summary>
		public string I2cErrorDeviceCannotOpen 
		{ 
			get { return ResourceLoader.GetString("I2cErrorDeviceCannotOpen"); }
		}
 	
		///<summary>
        ///0 = selector.
		///</summary>
		public string I2cErrorDeviceNotFound 
		{ 
			get { return ResourceLoader.GetString("I2cErrorDeviceNotFound"); }
		}
 	
		///<summary>
        ///0 = pressure, 1 = temperature.
		///</summary>
		public string MS5611MeasurementStringFormat 
		{ 
			get { return ResourceLoader.GetString("MS5611MeasurementStringFormat"); }
		}
 	
		///<summary>
        ///0 = # channels, 1 = maximum channels.
		///</summary>
		public string RCInputDecoderChannelOverflow 
		{ 
			get { return ResourceLoader.GetString("RCInputDecoderChannelOverflow"); }
		}
 	}
}
