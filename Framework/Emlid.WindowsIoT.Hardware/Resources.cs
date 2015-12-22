// PublicReswFileCodeGenerator for Windows 8.1 1.0.0
// (c) Sébastien Ollivier - https://github.com/sebastieno/PublicResWFileCodeGenerator/

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
					resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView("Strings");
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
        ///0 = # channels, 1 = maximum channels.
		///</summary>
		public string RCInputDecoderChannelOverflow 
		{ 
			get { return ResourceLoader.GetString("RCInputDecoderChannelOverflow"); }
		}
 	}
}
