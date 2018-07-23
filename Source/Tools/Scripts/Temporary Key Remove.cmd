@echo off
setlocal

echo Remove Visual Studio Temporary Key
echo ==================================
echo.
echo This solution uses temporary keys for development and testing.
echo.
echo This script removes the key from a private named store which
echo Visual Studio uses to locate private keys during build.
echo.
echo Every development machine has a different key (unfortunately) so
echo you will need to call this script once on each machine you wish
echo to clean-up and supply the correct parameter for your unique
echo key container name.
echo.

rem * Parse parameters
if "%~1" == "" goto Syntax
set VSKeyContainerName=%~1

rem * Initialize environment
echo Initializing environment...
call "%~dp0..\..\Dependencies\Variables.cmd"
if %errorlevel% neq 0 goto Error

rem * Remove key
sn -d %VSKeyContainerName%
if %errorlevel% neq 0 goto Error

rem * Exit successfully
echo Completed successfully.
endlocal
exit /b 0

rem * Syntax Error
:Syntax
echo Syntax^: "Temporary Key Install.cmd" VS_KEY_HHHHHHHHHHHHHHHH
echo.
echo VS_KEY_... is your unique Visual Studio key container, displayed in
echo the error message when the key is missing.
endlocal
exit /b 1

rem * Error Handler
:Error
set Result=%errorlevel%
echo.
echo Error %Result%
endlocal
exit /b 1
