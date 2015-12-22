# Windows 10 IoT Driver Deployment & Debugging Setup
# --------------------------------------------------

$session = New-PSSession -ComputerName NavioIot1 -Credential NavioIot1\Administrator
$job = Invoke-Command -Session $session -AsJob `
{
	# Remote Setup
	# ------------

	# Stop and disable SSH service
	Set-Service SshSvc -StartupType Disabled
	Stop-Service SshSvc

	# Start and enable test signing service
	Set-Service TestSirepSvc -StartupType Automatic
	Start-Service TestSirepSvc

	# Add test signature to trusted root certificates
	$key = Get-Item -Path Registry::HKLM\SOFTWARE\Microsoft\SystemCertificates\Root\Certificates\8a334aa8052dd244a647306a76b8178fa215f344 -ErrorAction Ignore
	if ($key -eq $null) {
			New-Item -Path Registry::HKLM\SOFTWARE\Microsoft\SystemCertificates\Root\Certificates\8a334aa8052dd244a647306a76b8178fa215f344
	}
	
	# Configure Boot Manager for remote debugging
	C:\Windows\System32\bcdedit.exe /dbgsettings serial   # Baud rate is hard-coded on Raspberry Pi 2 to 912600
	C:\Windows\System32\bcdedit.exe /debug on
	C:\Windows\System32\bcdedit.exe /bootdebug yes
	C:\Windows\System32\bcdedit.exe
}
Wait-Job $job
$result = Receive-Job $job
$result

# Local Setup
# -----------

# Install test signing certificate and key
$wpdkPath = "C:\Program Files (x86)\Windows Kits\10\"
[Environment]::SetEnvironmentVariable("WPDKContentRoot", $wpdkPath, "Machine")
$command = "`"" + $wpdkPath + "InstallOEMCerts.cmd`""
Invoke-Expression -Command:$command
