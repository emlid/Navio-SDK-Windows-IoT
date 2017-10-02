@echo off
setlocal
echo Developer Workstation Setup 
echo ===========================
echo This must be run on every developer PC to before the solution will build.
echo Workstation specific dependencies are installed or updated such as
echo digital certificates and global packages.
echo.
echo It may also need to be ran after any major changes to the project,
echo new projects and technologies, or certificates have expired/regenerated.
echo.
pause

rem * Parse parameters
if "%~1" == "" goto Syntax

rem * Install build signature key
call "%~dp0Tools\Scripts\Temporary Key Install.cmd" %~1
if %errorlevel% neq 0 goto Error

rem * Exit successfully
echo Completed successfully.
endlocal
exit /b 0

rem * Syntax Error
:Syntax
echo Syntax^: "Setup.cmd" VS_KEY_HHHHHHHHHHHHHHHH
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
