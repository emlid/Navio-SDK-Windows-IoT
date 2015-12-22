call "%VS140COMNTOOLS%vsvars32"
cd /d "%~dp0"
"%WindowsSdkDir%Tools\x64\AcpiVerify\asl.exe" /Fo=ACPITABL.DAT Navio.asl
pause
