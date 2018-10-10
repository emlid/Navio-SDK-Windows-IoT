# Code for .NET - Release Notes

## Version 4.72.1809.13### (2018.09.13)
* Fixed Variables.cmd script dependency to ensure dependency update and build scripts initialize from any command prompt.
* KeyboardShortcut enumeration added, a platform independent copy of System.Windows.Forms.Shortcut.
* Logical drawing styles renamed to clean "logical" word and replace with names like "data", which are closer to their purpose.

## Version 4.72.1807.24### (2018.07.24)
* Migrated almost all code from .NET Full to Standard libraries.
* Disambiguate Drawing.StringAlignment by renaming to LogicalStringAlignment.

## Version 4.72.1807.21### (2018.07.21)
* Upgraded to .NET Standard 2.0 and v4.72.
* Added new components for .NET Core, standard, full and Universal Windows.
* Removed version/current subdirectory from source and build to conform to Git branch model.
* Documentation converted from RTF to MarkDown.

## Version 4.52.1505.12### (2015.05.12)
* Windows Image Acquisition wrappers updated.
* Framework updated to 4.5.2.
* Unit tests converted to XUnit 2.0.

## Version 4.51.1310.12### (2013.10.30)
* Portable library re-targeted to reference 4.5/Windows/Phone 8.0 framework and other assemblies targeting 4.5.1. Silverlight  is now excluded as it is end of life.

## Version 4.51.1310.12### (2013.10.12)
* First public release.