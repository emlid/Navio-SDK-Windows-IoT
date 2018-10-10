# ==============================================================================
# Version Update Script
# ------------------------------------------------------------------------------
# Updates the solution version text file with the date based build number then
# applies the new version to all project files in the solution.
# ==============================================================================

# ==============================================================================
# Globals
# ------------------------------------------------------------------------------

# Options
Set-StrictMode -Version Latest    # Proactively avoid errors and inconsistency
$error.Clear()                    # Clear any errors from previous script runs
$ErrorActionPreference = "Stop"   # All unhandled errors stop program
$WarningPreference = "Stop"       # All warnings stop program

# ==============================================================================
# Modules
# ------------------------------------------------------------------------------

# Initialize module paths
$env:PSModulePath = [Environment]::GetEnvironmentVariable("PSModulePath", "Machine");
$env:PSModulePath = "$env:PSModulePath;$PSScriptRoot\Dependencies\Code for PowerShell\Modules";

# Import modules
Import-Module CodeForPowerShell.VisualStudio;

# ==============================================================================
# Main Program
# ------------------------------------------------------------------------------

# Display banner
Write-Output "Version Update Script";
Write-Output "=====================";
Write-Output "Increments the version number stored in the Version.txt file,";
Write-Output "then applies it to all relevant source files in the solution.";
Write-Output "Build is set to the UTC year and month in ""yyMM"" format.";
Write-Output "Revision is set to the UTC day * 1000 plus a three digit incrementing number.";
Write-Output "";

# Load current version file
$versionFilePath = "$PSScriptRoot\Version.txt";
$version = Get-VersionFile -File $versionFilePath;
Write-Host ("Old Version: " + $version.ToString());

# Update version and save
$newVersion = Update-Version -Version $version;
Write-Host ("New Version: " + $newVersion.ToString());
Set-VersionFile -File $versionFilePath -Version $newVersion;

# Set version in Visual Studio project and source files...
Set-VersionInAssemblyInfo -File "$PSScriptRoot\Common\AssemblyInfoGlobal.cs" -Version $newVersion;
Set-VersionInAppXManifest -File "$PSScriptRoot\Samples\CPP\Navio RC Input\Package.appxmanifest" -Version $newVersion;
Set-VersionInAppXManifest -File "$PSScriptRoot\Samples\CS\Navio Barometer\Package.appxmanifest" -Version $newVersion;
Set-VersionInAppXManifest -File "$PSScriptRoot\Samples\CS\Navio FRAM\Package.appxmanifest" -Version $newVersion;
Set-VersionInAppXManifest -File "$PSScriptRoot\Samples\CS\Navio LED\Package.appxmanifest" -Version $newVersion;
Set-VersionInAppXManifest -File "$PSScriptRoot\Samples\CS\Navio RC Input\Package.appxmanifest" -Version $newVersion;
Set-VersionInAppXManifest -File "$PSScriptRoot\Tools\Navio 2 RCIO Terminal\Package.appxmanifest" -Version $newVersion;
Set-VersionInAppXManifest -File "$PSScriptRoot\Tools\Navio Hardware Test\Package.appxmanifest" -Version $newVersion;

# Exit successful
Exit 0;