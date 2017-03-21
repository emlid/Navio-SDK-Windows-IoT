@echo off
setlocal

echo Install Visual Studio Temporary Key
echo ===================================
echo.
echo This solution uses temporary keys for development and testing.
echo.
echo This script installs the key into a private named store which
echo Visual Studio uses to locate private keys during build.
echo.
echo Every development machine has a different key (unfortunately) so
echo you will need to call this script once on each machine you wish
echo to build the source and supply the correct parameter for your unique
echo key container name.
echo.
echo A privately signed official copy of the SDK will be made available once
echo the features are complete and must never be checked-in to open source.
echo.
echo ***************************************************************************
echo *** ACTION: When prompted for a password, just hit enter (no password)! ***
echo ***************************************************************************
echo.

rem * Parse parameters
if "%~1" == "" goto syntax
set VSKeyContainerName=%~1

rem * Initialize environment
echo Initializing Visual Studio tools...
pushd "%~dp0"
call "%VS150ComnTools%VsDevCmd.bat"
if %errorlevel% neq 0 goto error
popd

rem * Install key
echo Source: %~dp0
sn -i "%~dp0..\..\Common\TemporaryKey.pfx" %VSKeyContainerName%
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

:syntax
echo Syntax^: "TemporaryKey Install.cmd" VS_KEY_HHHHHHHHHHHHHHHH
echo.
echo VS_KEY_... is your unique Visual Studio key container, displayed in
echo the error message when the key is missing.
endlocal
exit /b 1
