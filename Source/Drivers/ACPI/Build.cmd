@echo off
setlocal

rem * Initialize environment
echo Initializing Visual Studio tools...
pushd "%~dp0"
call "%VS150ComnTools%VsDevCmd.bat"
if %errorlevel% neq 0 goto error
popd

rem * Compile ACPI ASL
cd /d "%~dp0"
"%WindowsSdkDir%Tools\x64\AcpiVerify\asl.exe" /Fo=ACPITABL.DAT Navio.asl
if %errorlevel% neq 0 goto error
"%WindowsSdkDir%Tools\x64\AcpiVerify\asl.exe" /Fo=ACPITABL2.DAT Navio2.asl
if %errorlevel% neq 0 goto error

rem * Exit successfully
echo Completed successfully.
endlocal
exit /b 0

:error
set result=%errorlevel%
echo.
echo Error %result%
endlocal
exit /b %result%