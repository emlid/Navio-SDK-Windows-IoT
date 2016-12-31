# Windows 10 IoT Driver Deployment & Debugging Remote Setup
# ---------------------------------------------------------

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
	$certificatePath = Registry::HKLM\SOFTWARE\Microsoft\SystemCertificates\Root\Certificates\8a334aa8052dd244a647306a76b8178fa215f344
	if (!(Test-Path $certificatePath)) { New-Item -Path $certificatePath }

	# Configure Boot Manager for remote debugging
	C:\Windows\System32\bcdedit.exe /dbgsettings serial   # Baud rate is hard-coded on Raspberry Pi 2 to 912600
	C:\Windows\System32\bcdedit.exe /debug on
	C:\Windows\System32\bcdedit.exe /bootdebug yes
	C:\Windows\System32\bcdedit.exe
}
Wait-Job $job
$result = Receive-Job $job
$result
