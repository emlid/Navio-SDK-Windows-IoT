# Windows 10 IoT Debugging Local Setup
# ------------------------------------

# Initialize
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# Install test signing certificate and key
#Write-Host "Installing WDK Test Certificates"
#$wpdkPath = "C:\Program Files (x86)\Windows Kits\10\"
#[Environment]::SetEnvironmentVariable("WPDKContentRoot", $wpdkPath, "Machine")
#$command = "`"" + $wpdkPath + "Tools\Bin\I386\InstallOEMCerts.cmd`""
#Invoke-Expression -Command:$command

# Ensure .NET binding failure logging is enabled (warning from WDK PKGGEN when not and useful anyway)
New-ItemProperty -Path 'HKLM:\Software\Microsoft\Fusion' -Name 'EnableLog' -Value 1 -PropertyType DWORD -Force
