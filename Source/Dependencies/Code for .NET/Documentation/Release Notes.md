# Code for .NET - Release Notes

## Version 4.8.1911.13### (2019.11.13)
* Renamed Vector2 to AngleVector2 to avoid conflict with new Vector2 system class.
* Moved AngleVector2 and Quadrant to Numerics namespace to follow similar system class grouping.
* Enhanced AngleVector2 unit test to visually test both angle to point and point to angle (rount-trip).
* Optimized AngleVector2 code to improve accuracy and ensure round-trip works 100% (visually).

## Version 4.8.1910.09### (2019.10.09)
* Visual Studio 2019 16.3 new code analysis fixes and refactoring including C# 8 nullable.
* Full framework version updated to v4.8.
* Preparation for .NET Core 3 although components must remain on .NET Standard 2.0 until full framework supports 2.1.

## Version 4.72.1904.10### (2019.04.10)
* Visual Studio 2019 update with new code analysis fixes and refactoring.
* Fixed UWP tests.
 
## Version 4.72.1903.11### (2019.03.11)
* Remove DictionaryChangedEventHandler in preference of using the newer EventHandler&lt;T&gt; generic delegate definition.
* Add missing WPF/XAML build dependency to Windows Universal component build.
* Update packages.

## Version 4.72.1902.28### (2019.02.28)
* Restore ObservableDictionary as still not available in .NET Standard, this time using similar event argument types and naming as the Microsoft ObservableCollection.
* Add paint and mouse event arguments to UI assembly.
* Move keyboard and command input classes from WindowsUniversal to UI assembly as they have no strict dependency on XAML and could be used generically.
* Added StringCollection.Contains() extension.
* Add support for disposing generic dictionaries to DictionaryExtensions.

## Version 4.72.1811.25### (2018.11.25)
* Remove ObservableDictionary and ObservableCollection in favour of built-in types.

## Version 4.72.1810.16### (2018.10.16)
* Remove all global code analysis suppressions, re-evaluate all CA errors and correct or suppress with attribute on the class or member affected.
* Avoid overflow exceptions in Number casting down to byte during internal manipulation (not a bug but an un-necessary error when compiling with check for overflow, now set as standard in this solution for debug builds).

## Version 4.72.1810.11### (2018.10.10)
* Added UI library and components.
* Switch to new Code Analysis analyzers (NuGet package based).
* Updated extensions.
* Updated dependencies.
* Updated unit test project references.

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
