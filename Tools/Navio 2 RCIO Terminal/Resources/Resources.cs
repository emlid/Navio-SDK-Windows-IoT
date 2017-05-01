// PublicReswFileCodeGenerator for Windows 8.1 1.0.0
// (c) Sébastien Ollivier - https://github.com/sebastieno/PublicResWFileCodeGenerator/
// Updated by Tony Wall for Windows 10/IoT usage.

namespace Emlid.WindowsIot.Tools.Navio2RcioTerminal.Resources  {
		
	/// <summary>
	/// Strings.
	/// </summary>
	public static class Strings {
		
		private static Windows.ApplicationModel.Resources.ResourceLoader resourceLoader;
		internal static Windows.ApplicationModel.Resources.ResourceLoader ResourceLoader
		{
			get
			{
				if(resourceLoader == null)
				{
					resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForViewIndependentUse("Strings");
				}

				return resourceLoader;
			}
		}
			
		///<summary>
		///
		///</summary>
		public static string UnsupportedModelError 
		{ 
			get { return ResourceLoader.GetString("UnsupportedModelError"); }
		}
 	}
}
