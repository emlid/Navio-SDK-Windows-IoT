@echo off
echo ==========================================================================
echo VSWhereDev.cmd
echo --------------------------------------------------------------------------
echo Initializes Visual Studio environment variables (VSDevCmd) with VSWhere
echo to locate the installation path, which is no longer set automatically
echo during installation.
echo --------------------------------------------------------------------------
echo.

echo * Calling VSWhere to locate Visual Studio 2017 (version 15.x).
for /f "usebackq tokens=1* delims=: " %%i in (`"%~dp0..\..\Dependencies\VSWhere\vswhere.exe" -version 15.0^,16.0`) do (
	if /i "%%~i"=="installationPath" set VS150InstallDir=%%j
)
if %errorlevel% neq 0 goto error
echo.
echo Visual Studio 15 Installation Path: "%VS150InstallDir%".
set VS150ComnTools=%VS150InstallDir%\Common7\Tools
echo Visual Studio 15 Tools: "%VS150ComnTools%".
echo.

echo * Calling VSDevCmd.bat to initialize environment.
set VSCMD_START_DIR=%CD%
call "%VS150ComnTools%\VSDevCmd.bat"
if %errorlevel% neq 0 goto error

echo.
echo --------------------------------------------------------------------------
echo * VSWhereDev.cmd completed sucessfully
echo ==========================================================================
exit /b 0

:error
echo.
echo --------------------------------------------------------------------------
echo ! VSWhereDev.cmd error %errorlevel%
echo ==========================================================================
echo.
exit /b 1
