<#
	.Synopsis
	Visual Studio Version PowerShell Module

	.Description
	Provides functions for versioning of Visual Studio projects and source files.
	The version logic is as follows:
	Versions are stored in text files which can be read and written with Get/Set-VersionFile methods.

	Versions are updated/calculated via the Update-Version method, which calculates the version as follows:
	* Major and minor parts of the version are static, i.e. controlled by the developer.
	* Build is set to the UTC year and month in ""yyMM"" format.
	* Revision is set to the UTC day * 1000 plus a three digit incrementing number.

	This version format has the following benefits:
	1) It is predictable, the date can be devised from any released module.
	2) Storing the version in text files allows them to be redistributed and used to update dependency references,
	   or other simple file based logic used to scan and identify source/runtime versions.
	3) Up to 999 builds per day are supported which should be enough for any environment, even continuous build.
	4) Can be called during a build script for fully automate incremental versioning on each test or release build.
	5) Unique versions solve limitations caused when updating code without changing the version number.
	6) When the update and reference updates are automated there is no longer any risk changing the version.,
	7) GAC deployment is no longer necessary just to consistently version dependencies, because binding redirects
	   are easy to maintain when they are updated automatically via this script.
#>

#region Globals.

# Options.
Set-StrictMode -Version Latest;   # Proactively avoid errors and inconsistency.
$Error.Clear();                   # Clear any errors from previous script runs.
$ErrorActionPreference = 'Stop';  # All unhandled errors stop program.
$WarningPreference = 'Stop';      # All warnings stop program.

#endregion

#region Functions.

<#
	.Synopsis
	Calculates a new version based on the major and minor parts of an existing version plus
	newly calculated build and revision parts based on the current date and incrementing
	revision number.
#>
[CmdletBinding]
function Update-Version([Version]$Version)
{
	# Get date.
	$date =(Get-Date).ToUniversalTime();

	# Calculate date relative parts of version.
	$newBuild = $date.ToString("yyMM");
	$dayRevisionMin = $date.Day * 1000;
	$dayRevisionMax = $dayRevisionMin + 999;

	# Increment revision.
	if($Version.Revision -ge $dayRevisionMin -and $Version.Revision -le $dayRevisionMax)
	{
		# Increment revision in range
		$newRevision = $Version.Revision + 1;
	} else
	{
		# Use minimum revision when out of range
		$newRevision = $dayRevisionMin;
	}

	# Return new version.
	New-Object -TypeName System.Version -ArgumentList $Version.Major, $Version.Minor, $newBuild, $newRevision;
}

<#
	.Synopsis
	Reads a version from a text file and returns a version object.
#>
[CmdletBinding]
function Get-VersionFile([String]$File)
{
	Write-Host("Reading version file " + $File);

	# Read file.
	$versionString = [System.IO.File]::ReadAllText($File).Trim();

	# Return version.
	New-Object -TypeName System.Version -ArgumentList $versionString;
}

<#
	.Synopsis
	Saves or overwrites a version text file.
#>
[CmdletBinding]
function Set-VersionFile([String]$File, [Version]$Version)
{
	Write-Host("Writing version file " + $File);

	# Write file.
	[System.IO.File]::WriteAllText($File, $Version.ToString());
}

<#
	.Synopsis
	Sets the version in a Windows Universal AppX package manifest file.
#>
[CmdletBinding]
function Set-VersionInAppXManifest([String]$File, [Version]$Version)
{
	Write-Host("Setting version in Windows Universal package manifest file " + $File);

	# Load file
	$contents = [xml][System.IO.File]::ReadAllText($File);

	# Find and replace manifest version
	$versionElement = $contents.SelectSingleNode("/node()[name() = 'Package']/node()[name() = 'Identity']/@Version");
	if ($versionElement -ne $null) { $versionElement.Value = $Version.ToString(); }

	# Save changes
	[System.IO.File]::WriteAllText($File, $contents.OuterXml);
}

<#
	.Synopsis
	Sets the version in a .NET assembly info file.
#>
[CmdletBinding]
function Set-VersionInAssemblyInfo([String]$File, [Version]$Version)
{
	Write-Host("Setting version in assembly info file " + $File);

	# Load file.
	$contents = [System.IO.File]::ReadAllText($File);

	# Find and replace version.
	$contents = [RegEx]::Replace($contents, "(AssemblyVersion(?:Attribute)?\(\"")(?:\d+\.\d+\.\d+\.\d+)(\""\))",("`${1}" + $Version.ToString() + "`${2}"));
	$contents = [RegEx]::Replace($contents, "(AssemblyFileVersion(?:Attribute)?\(\"")(?:\d+\.\d+\.\d+\.\d+)(\""\))",("`${1}" + $Version.ToString() + "`${2}"));

	# Save changes.
	[System.IO.File]::WriteAllText($File, $contents);
}

<#
	.Synopsis
	Sets the version in assembly references of a .NET configuration file.
#>
[CmdletBinding]
function Set-VersionInAssemblyReference([String]$File, [String]$AssemblyName, [Version]$Version)
{
	Write-Host("Setting version in assembly references of " + $File);

	# Load file.
	$contents = [System.IO.File]::ReadAllText($File);

	# Find and replace version.
	$contents = [RegEx]::Replace($contents, "([\"">](?:\S+,\s+){0,1}" + $AssemblyName + ",\s+Version=)(?:\d+\.\d+\.\d+\.\d+)([,\""<])",("`${1}" + $Version.ToString() + "`${2}"));

	# Save changes.
	[System.IO.File]::WriteAllText($File, $contents);
}

<#
	.Synopsis
	Sets the version in binding redirects of a .NET configuration file.
#>
[CmdletBinding]
function Set-VersionInBindingRedirect([String]$File, [String]$AssemblyName, [Version]$Version)
{
	Write-Host("Setting version in binding redirects of " + $File);

	# Load file.
	$contents = [System.IO.File]::ReadAllText($File);

	# Find and replace version.
	$oldVersionMax = New-Object -TypeName "System.Version" -ArgumentList $Version.Major, $Version.Minor, $Version.Build,($Version.Revision - 1);
	$pattern = "(<dependentAssembly>[\s\S]*?<assemblyIdentity\s+name=\""" + $AssemblyName + "\""[\s\S]+?/>[\s\S]*?<bindingRedirect\s+oldVersion=\""\d+\.\d+\.\d+\.\d+-)(?:\d+\.\d+\.\d+\.\d+)(\""\s+newVersion=\"")(?:\d+\.\d+\.\d+\.\d+)(\""[\s\S]*?/>)";
	$contents = [RegEx]::Replace($contents, $pattern,("`${1}" + $oldVersionMax.ToString() + "`${2}" + $Version.ToString() + "`${3}"));

	# Save changes.
	[System.IO.File]::WriteAllText($File, $contents);
}

<#
	.Synopsis
	Sets the version in a C++ module definition file.
#>
[CmdletBinding]
function Set-VersionInCppModuleDefinitionFile([String]$File, [Version]$Version)
{
	Write-Host("Setting version in C++ module definition file " + $File);

	# Load file.
	$contents = [System.IO.File]::ReadAllText($File);

	# Find and replace version.
	$contents = [RegEx]::Replace($contents, "(VERSION\s+)(?:\d+\,\d+\,\d+\,\d+)",("`${1}" + $Version.ToString(2)));

	# Save changes.
	[System.IO.File]::WriteAllText($File, $contents);
}

<#
	.Synopsis
	Sets the version in a C++ resource file.
#>
[CmdletBinding]
function Set-VersionInCppResourceFile([String]$File, [Version]$Version)
{
	Write-Host("Setting version in C++ resource file " + $File);

	# Load file.
	$contents = [System.IO.File]::ReadAllText($File);

	# Find and replace version.
	$contents = [RegEx]::Replace($contents, "(FILEVERSION\s+)(?:\d+\,\d+\,\d+\,\d+)",("`${1}" + $Version.Major.ToString() + "," + $Version.Minor.ToString() + "," + $Version.Build.ToString() + "," + $Version.Revision.ToString()));
	$contents = [RegEx]::Replace($contents, "(PRODUCTVERSION\s+)(?:\d+\,\d+\,\d+\,\d+)",("`${1}" + $Version.Major.ToString() + "," + $Version.Minor.ToString() + "," + $Version.Build.ToString() + "," + $Version.Revision.ToString()));
	$contents = [RegEx]::Replace($contents, "(VALUE\s+\""FileVersion\"",\s*\"")(?:\d+\.\d+\.\d+\.\d+)(\"")",("`${1}" + $Version.ToString() + "`${2}"));
	$contents = [RegEx]::Replace($contents, "(VALUE\s+\""ProductVersion\"",\s*\"")(?:\d+\.\d+\.\d+\.\d+)(\"")",("`${1}" + $Version.ToString() + "`${2}"));

	# Save changes.
	[System.IO.File]::WriteAllText($File, $contents);
}

<#
	.Synopsis
	Sets the version in a global variable of a PowerShell script file.
#>
[CmdletBinding]
function Set-VersionInPowerShellScript([String]$File, [String]$Variable, [Version]$Version)
{
	Write-Host("Setting version in PowerShell script variable `$$Variable in file `"$File`"");

	# Load file.
	$contents = [System.IO.File]::ReadAllText($File);

	# Find and replace version.
	$contents = [RegEx]::Replace($contents, "(\s*\`$$Variable\s*=\s*')(?:\d+(?:\.\d+)+)(')",("`${1}" + $Version.ToString() + "`${2}"));

	# Save changes.
	[System.IO.File]::WriteAllText($File, $contents);
}

<#
	.Synopsis
	Sets the version in a global variable of a PowerShell manifest file.
#>
[CmdletBinding]
function Set-VersionInPowerShellManifest([String]$File, [Version]$Version)
{
	Write-Host("Setting version in PowerShell manifest file " + $File);

	# Load file.
	$contents = [System.IO.File]::ReadAllText($File);

	# Find and replace version.
	$contents = [RegEx]::Replace($contents, "(\s*ModuleVersion\s*=\s*')(?:\d+(?:\.\d+)+)(')",("`${1}" + $Version.ToString() + "`${2}"));

	# Save changes.
	[System.IO.File]::WriteAllText($File, $contents);
}

<#
	.Synopsis
	Sets the version in a Visual Studio SQL Server database project file.
#>
[CmdletBinding]
function Set-VersionInSqlDatabaseProject([String]$File, [Version]$Version)
{
	Write-Host("Setting version in Database project file " + $File);

	# Load file.
	$contents = [System.IO.File]::ReadAllText($File);

	# Find and replace version.
	$contents = [RegEx]::Replace($contents, "(<DacVersion>)(?:\d+\.\d+\.\d+\.\d+)(</DacVersion>)",("`${1}" + $Version.ToString() + "`${2}"));

	# Save changes.
	[System.IO.File]::WriteAllText($File, $contents);
}

<#
	.Synopsis
	Sets the version in a Windows Installer XML global definition file.
#>
[CmdletBinding]
function Set-VersionInWixGlobal([String]$File, [Version]$Version)
{
	Write-Host("Setting version in WIX global file " + $File);

	# Load file.
	$contents = [System.IO.File]::ReadAllText($File);

	# Find and replace version.
	$contents = [RegEx]::Replace($contents, "(\<\?define\s*ProductVersion\s*=\s*\"")(?:\d+\.\d+\.\d+\.\d+)(\""\s*\?\>)",("`${1}" + $Version.ToString() + "`${2}"));

	# Save changes.
	[System.IO.File]::WriteAllText($File, $contents);
}

<#
	.Synopsis
	Sets the version in a Windows Installer XML project file.
#>
[CmdletBinding]
function Set-VersionInXmlProject([String]$File, [Version]$Version)
{
	Write-Host("Setting version in XML project file " + $File);

	# Load file.
	$contents = [xml][System.IO.File]::ReadAllText($File);

	# Find and replace manifest version.
	$versionElement = $contents.SelectSingleNode("/Project/PropertyGroup/Version");
	if ($versionElement -ne $null) { $versionElement.InnerText = $Version.ToString(); }

	# Find and replace assembly version.
	$versionElement = $contents.SelectSingleNode("/Project/PropertyGroup/AssemblyVersion");
	if ($versionElement -ne $null) { $versionElement.InnerText = $Version.ToString(); }

	# Find and replace file version.
	$versionElement = $contents.SelectSingleNode("/Project/PropertyGroup/FileVersion");
	if ($versionElement -ne $null) { $versionElement.InnerText = $Version.ToString(); }

	# Save changes.
	[System.IO.File]::WriteAllText($File, $contents.OuterXml);
}

#endregion
