@echo off
call "%vs140comntools%vsvars32.bat"

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
if "%~1" == "" goto syntax
set VSKeyContainerName=%~1

rem * Remove key
sn -d %VSKeyContainerName%
if %errorlevel% neq 0 pause

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
echo Syntax^: "TemporaryKey Remove.cmd" VS_KEY_HHHHHHHHHHHHHHHH
echo.
echo VS_KEY_... is your unique Visual Studio key container, displayed in
echo the error message when the key is missing.
endlocal
exit /b 1
