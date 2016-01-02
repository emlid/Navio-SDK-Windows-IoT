# Windows 10 IoT Debugging Local Setup
# ------------------------------------

# Install test signing certificate and key
$wpdkPath = "C:\Program Files (x86)\Windows Kits\10\"
[Environment]::SetEnvironmentVariable("WPDKContentRoot", $wpdkPath, "Machine")
$command = "`"" + $wpdkPath + "InstallOEMCerts.cmd`""
Invoke-Expression -Command:$command
